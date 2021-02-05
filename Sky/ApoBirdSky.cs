using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Twilight.Items;
using Twilight.NPCs;

namespace Twilight.Sky
{
    public class ApoBirdSky : CustomSky
    {
        private bool isActive = false;
        private static float intensity = 0;
        public static int CurrentState = 0;   //0为待命，1为开启，2为全开.3为结束
        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                switch (CurrentState)
                {
                    case 0:

                        break;
                    case 1:
                        intensity += 0.01f;
                        if (intensity >= 1)
                        {
                            intensity = 1;
                            CurrentState = 2;
                        }
                        break;
                    case 2:
                        if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<TwilightItem>())
                        {
                            CurrentState = 3;
                        }
                        if (!Main.LocalPlayer.active || Main.LocalPlayer.dead || Main.LocalPlayer.ghost)
                        {
                            CurrentState = 3;
                        }
                        break;
                    case 3:
                        intensity -= 0.01f;
                        if (intensity <= 0)
                        {
                            intensity = 0;
                            CurrentState = 0;
                            foreach (NPC npc in Main.npc)
                            {
                                if (npc.active && (npc.type == ModContent.NPCType<ApoBirdBody>() ||
                                    npc.type == ModContent.NPCType<ApoBirdClaw>() ||
                                    npc.type == ModContent.NPCType<ApoBirdHead>()))
                                {
                                    npc.active = false;
                                }
                            }
                            Main.LocalPlayer.GetModPlayer<TwilightPlayer>().ApoBirdCD = 1800;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                CurrentState = 3;
                intensity -= 0.01f;
                if (intensity < 0)
                {
                    intensity = 0;
                }
            }


        }



        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {

                DrawCurtain(spriteBatch, 0, intensity, Color.White);
                if (intensity > 0 && intensity < 1)
                {
                    for (float i = 0; i <= 10f; i++)
                    {
                        float k = i / 300f;
                        float a = (10f - i) / 10f;
                        DrawCurtain(spriteBatch, intensity + k, intensity + k + 0.01f, Color.White * a);
                    }
                }
                if (isActive && Twilight.FindBody() != -1 && Twilight.FindHead() != -1)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    DrawWings();
                    DrawWingEyes();
                    DrawNeck();
                    DrawArmExtras();
                    DrawBody();

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                    DrawFloor(Main.spriteBatch, 0, intensity, Color.White);
                    if (intensity > 0 && intensity < 1)
                    {
                        for (float i = 0; i <= 10f; i++)
                        {
                            float k = i / 300f;
                            float a = (10f - i) / 10f;
                            DrawFloor(Main.spriteBatch, intensity + k, intensity + k + 0.01f, Color.White * a);
                        }
                    }


                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                    DrawHead();
                    DrawArms();
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            }
        }

        private void DrawCurtain(SpriteBatch sb, float d1, float d2, Color color)
        {
            Texture2D tex = Twilight.Instance.GetTexture("Sky/ApoBirdSky");
            sb.Draw(tex, new Rectangle((int)(Main.screenWidth * d1), 0, (int)(Main.screenWidth * d2), Main.screenHeight), new Rectangle((int)(tex.Width * d1), 0, (int)(tex.Width * d2), tex.Height), color);
        }

        private void DrawFloor(SpriteBatch sb, float d1, float d2, Color color)
        {
            Texture2D tex = Twilight.Instance.GetTexture("Sky/ApoBirdSky2");
            sb.Draw(tex, new Rectangle((int)(Main.screenWidth * d1), 0, (int)(Main.screenWidth * d2), Main.screenHeight), new Rectangle((int)(tex.Width * d1), 0, (int)(tex.Width * d2), tex.Height), color);
        }

        public override float GetCloudAlpha()
        {
            return 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive()
        {
            return isActive || intensity > 0f;
        }



        public void DrawNeck()
        {
            if (Twilight.FindBody() == -1) return;
            if (Twilight.FindHead() == -1) return;

            Texture2D NeckTex = Twilight.Instance.GetTexture("NPCs/ApoBirdNeck");
            NPC head = Main.npc[Twilight.FindHead()];
            NPC body = Main.npc[Twilight.FindBody()];
            Vector2 NeckTop = new Vector2((head.Center.X + body.Center.X) / 2, body.Center.Y - 600);
            float NeckRot = (NeckTop - body.Center).ToRotation();
            Vector2 Scale = new Vector2(Vector2.Distance(NeckTop, body.Center) / 240, 1f);
            Main.spriteBatch.Draw(NeckTex, (NeckTop + body.Center) / 2 - Main.screenPosition, null, Color.White * intensity, NeckRot, NeckTex.Size() / 2, Scale, SpriteEffects.None, 0);

            NeckRot = (head.Center - NeckTop).ToRotation() + MathHelper.Pi;
            Scale = new Vector2(Vector2.Distance(NeckTop, head.Center) / 240, 0.75f);
            Main.spriteBatch.Draw(NeckTex, (NeckTop + head.Center) / 2 - Main.screenPosition, null, Color.White * intensity, NeckRot, NeckTex.Size() / 2, Scale, SpriteEffects.None, 0);
        }

        public void DrawWings()
        {
            if (Twilight.FindBody() == -1) return;
            NPC body = Main.npc[Twilight.FindBody()];
            ApoBirdBody aopbody = body.modNPC as ApoBirdBody;
            Texture2D WingTex = Twilight.Instance.GetTexture("NPCs/ApoBirdWings");
            Main.spriteBatch.Draw(WingTex, body.Center + new Vector2(5, 0) - Main.screenPosition, null, Color.White * intensity, -aopbody.WingRot, WingTex.Size(), body.scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(WingTex, body.Center + new Vector2(-5, 0) - Main.screenPosition, null, Color.White * intensity, aopbody.WingRot, new Vector2(0, WingTex.Height), body.scale * 0.9f, SpriteEffects.FlipHorizontally, 0);
        }

        public void DrawWingEyes()
        {
            if (Twilight.FindBody() == -1) return;
            NPC body = Main.npc[Twilight.FindBody()];
            if (CurrentState != 2) return;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex = Twilight.Instance.GetTexture("NPCs/EyeLight");
            //左
            Vector2 DrawPos = body.Center + new Vector2(5, 0) - Main.screenPosition;
            float ops = (float)Math.Sin((body.modNPC as ApoBirdBody).WingLight / 100f * MathHelper.Pi);
            float WingRot = (body.modNPC as ApoBirdBody).WingRot;
            for (int i = 0; i < ApoBirdBody.WingEyePos.Length; i++)
            {
                Vector2 EyePos = ApoBirdBody.WingEyePos[i] * 0.9f;
                EyePos.X = -EyePos.X;
                float EyeRot = ApoBirdBody.WingEyeRot[i];
                Main.spriteBatch.Draw(tex, DrawPos + Twilight.RotateVector2(EyePos, -WingRot), null, Color.White * ops, EyeRot, tex.Size() / 2, ApoBirdBody.WingEyeScale[i] * body.scale, SpriteEffects.None, 0);
            }
            //右
            DrawPos = body.Center + new Vector2(-5, 0) - Main.screenPosition;
            for (int i = 0; i < ApoBirdBody.WingEyePos.Length; i++)
            {
                Vector2 EyePos = ApoBirdBody.WingEyePos[i] * 0.9f;
                float EyeRot = -ApoBirdBody.WingEyeRot[i];
                Main.spriteBatch.Draw(tex, DrawPos + Twilight.RotateVector2(EyePos, WingRot), null, Color.White * ops, EyeRot, tex.Size() / 2, ApoBirdBody.WingEyeScale[i] * body.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
        }

        public void DrawHead()
        {
            if (Twilight.FindHead() == -1) return;
            NPC head = Main.npc[Twilight.FindHead()];
            Texture2D tex = Twilight.Instance.GetTexture("NPCs/ApoBirdHead");//Main.npcTexture[ModContent.NPCType<ApoBirdHead>()];
            Texture2D tex2 = Twilight.Instance.GetTexture("NPCs/EyeBall");
            Main.spriteBatch.Draw(tex, head.Center - Main.screenPosition, null, Color.White * intensity, head.rotation, tex.Size() / 2, head.scale, SpriteEffects.None, 0);
            float rot = (Main.MouseWorld - head.Center).ToRotation();
            Vector2 EyePos = head.Center + new Vector2(12, 50) - Main.screenPosition + rot.ToRotationVector2() * 9;
            Main.spriteBatch.Draw(tex2, EyePos, null, Color.White * intensity, 0, tex2.Size() / 2, head.scale, SpriteEffects.None, 0);
        }

        public void DrawBody()
        {
            if (Twilight.FindBody() == -1) return;
            NPC body = Main.npc[Twilight.FindBody()];
            Texture2D tex = Twilight.Instance.GetTexture("NPCs/ApoBirdBody"); //Main.npcTexture[ModContent.NPCType<ApoBirdBody>()];
            Main.spriteBatch.Draw(tex, body.Center - Main.screenPosition, null, Color.White * intensity, body.rotation, new Vector2(tex.Width / 2, 834), body.scale * 0.85f, SpriteEffects.None, 0);
        }

        public void DrawArms()
        {
            if (Twilight.FindArmLeft() == -1) return;
            Texture2D tex = Twilight.Instance.GetTexture("NPCs/ApoBirdClaw");// Main.npcTexture[ModContent.NPCType<ApoBirdClaw>()];
            NPC Arm = Main.npc[Twilight.FindArmLeft()];
            SpriteEffects SP = SpriteEffects.None;
            Main.spriteBatch.Draw(tex, Arm.Center - Main.screenPosition, null, Color.White * intensity, Arm.rotation, tex.Size() / 2, Arm.scale * 1.5f, SP, 0);

            if (Twilight.FindArmRight() == -1) return;
            Arm = Main.npc[Twilight.FindArmRight()];
            SP = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(tex, Arm.Center - Main.screenPosition, null, Color.White * intensity, Arm.rotation, tex.Size() / 2, Arm.scale * 1.5f, SP, 0);
        }

        public void DrawArmExtras()
        {
            if (Twilight.FindBody() == -1) return;
            NPC body = Main.npc[Twilight.FindBody()];
            if (Twilight.FindArmLeft() == -1 || Twilight.FindArmRight() == -1) return;
            DrawArmExtra(body.Center + DrawNPCRot(body, new Vector2(-600, -660)), Main.npc[Twilight.FindArmLeft()].Center + new Vector2(-220, -640));  //532 1024
            DrawArmExtra(body.Center + DrawNPCRot(body, new Vector2(635, -660)), Main.npc[Twilight.FindArmRight()].Center + new Vector2(220, -640));
        }

        public void DrawArmExtra(Vector2 Start, Vector2 End)
        {
            Vector2 DrawPos = (Start + End) / 2 - Main.screenPosition;
            Vector2 Scale = new Vector2(Vector2.Distance(Start, End) / 320, 1f);
            float Rot = (End - Start).ToRotation();
            Texture2D tex = Twilight.Instance.GetTexture("NPCs/ApoBirdClawExtra");
            Main.spriteBatch.Draw(tex, DrawPos, null, Color.White * intensity, Rot, tex.Size() / 2, Scale, SpriteEffects.None, 0);
        }

        public Vector2 DrawNPCRot(NPC npc, Vector2 vec)
        {
            return Twilight.RotateVector2(vec, npc.rotation);
        }

        public static float GetIntensity()
        {
            return intensity;
        }

        public static void SummonTheBird()
        {
            if (CurrentState != 0) return;
            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
            {
                CurrentState = 1;
                NPC.NewNPC((int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<ApoBirdBody>(), 0, 0, 0, 0, Main.myPlayer);
            }
        }
    }
}