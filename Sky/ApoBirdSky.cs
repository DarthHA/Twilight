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

        public enum State
        {
            NoSky,
            Revealing,
            ApoSky,
            Ending
        }

        public static State CurrentState = State.NoSky;   //0为待命，1为开启，2为全开.3为结束
        public override void Update(GameTime gameTime)
        {
            if (isActive)
            {
                switch (CurrentState)
                {
                    case State.NoSky:

                        break;
                    case State.Revealing:
                        intensity += 0.01f;
                        if (intensity >= 1)
                        {
                            intensity = 1;
                            CurrentState = State.ApoSky;
                        }
                        break;
                    case State.ApoSky:
                        if (BirdUtils.FindBody() == -1)
                        {
                            CurrentState = State.Ending;
                        }
                        else
                        {
                            if (Main.npc[BirdUtils.FindBody()].ai[1] == 0)
                            {
                                if (Main.LocalPlayer.HeldItem.type != ModContent.ItemType<TwilightItem>())
                                {
                                    CurrentState = State.Ending;
                                }
                                if (!Main.LocalPlayer.active || Main.LocalPlayer.dead || Main.LocalPlayer.ghost)
                                {
                                    CurrentState = State.Ending;
                                }
                            }
                        }
                        break;
                    case State.Ending:
                        intensity -= 0.01f;
                        if (intensity <= 0)
                        {
                            intensity = 0;
                            CurrentState = State.NoSky;
                            foreach (NPC npc in Main.npc)
                            {
                                if (npc.active && (npc.type == ModContent.NPCType<ApoBirdBody>() ||
                                    npc.type == ModContent.NPCType<ApoBirdClaw>() ||
                                    npc.type == ModContent.NPCType<ApoBirdHead>()))
                                {
                                    npc.active = false;
                                }
                            }
                            Main.LocalPlayer.GetModPlayer<TwilightPlayer>().NextNormalAttack = 0;
                            Main.LocalPlayer.GetModPlayer<TwilightPlayer>().ApoBirdCD = 1800;
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                CurrentState = State.Ending;
                intensity -= 0.01f;
                if (intensity < 0)
                {
                    intensity = 0;
                }
            }
        }

        public void DrawInCurtain(SpriteBatch spriteBatch)
        {
            Texture2D texBg = ModContent.Request<Texture2D>("Twilight/Sky/ApoBirdSky").Value;
            float t = (Main.LocalPlayer.Center.X - (int)(Main.LocalPlayer.Center.X / (Main.screenWidth * 3)) * Main.screenWidth * 3) / (Main.screenWidth * 3);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied, Main.BackgroundViewMatrix.ZoomMatrix);
            DrawBgs(spriteBatch, texBg, 1 - t);
            if (isActive && BirdUtils.FindBody() != -1 && BirdUtils.FindHead() != -1)
            {
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied, Main.GameViewMatrix.ZoomMatrix);
                DrawWings();
                DrawWingEyes();
                DrawNeck();
                DrawArmExtras();
                DrawBody();

                Texture2D texFloor = ModContent.Request<Texture2D>("Twilight/Sky/ApoBirdSky2").Value;
                t = (Main.LocalPlayer.Center.X - (int)(Main.LocalPlayer.Center.X / (Main.screenWidth * 1.5f)) * Main.screenWidth * 1.5f) / (Main.screenWidth * 1.5f);
                EasyDraw.AnotherDraw(BlendState.NonPremultiplied, Main.BackgroundViewMatrix.ZoomMatrix);
                DrawBgs(spriteBatch, texFloor, 1 - t);

                EasyDraw.AnotherDraw(BlendState.NonPremultiplied, Main.GameViewMatrix.ZoomMatrix);

                DrawHead();
                DrawArms();
            }
        }

        public void DrawOutOfCurtain(SpriteBatch spriteBatch, float progress)
        {
            if (!Main.drawToScreen && !Main.gameMenu)
            {
                GraphicsDevice gd = Main.graphics.GraphicsDevice;
                spriteBatch.End();
                //Save Screen
                gd.SetRenderTarget(Main.screenTargetSwap);
                gd.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                spriteBatch.End();

                //Save sky
                gd.SetRenderTarget(SkyRenderHandler.skyRender);
                gd.Clear(Color.Transparent);
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                //draw here
                DrawInCurtain(spriteBatch);
                spriteBatch.End();

                //Draw Them
                gd.SetRenderTarget(Main.screenTarget);
                gd.Clear(Color.Transparent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                spriteBatch.End();

                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                Twilight.CurtainEffect.Parameters["progress"].SetValue(progress);
                Twilight.CurtainEffect.CurrentTechnique.Passes["CurtainEffect"].Apply();
                spriteBatch.Draw(SkyRenderHandler.skyRender, Vector2.Zero, Color.White);
                spriteBatch.End();

            }
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (maxDepth >= 0 && minDepth < 0)
            {
                if (intensity > 0)
                {
                    DrawOutOfCurtain(spriteBatch, 1 - (intensity * 1.05f));

                    EasyDraw.AnotherDraw(BlendState.AlphaBlend, Main.GameViewMatrix.TransformationMatrix);
                }
            }
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



        internal void DrawNeck()
        {
            if (BirdUtils.FindBody() == -1) return;
            if (BirdUtils.FindHead() == -1) return;

            Texture2D NeckTex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdNeck").Value;
            NPC head = Main.npc[BirdUtils.FindHead()];
            NPC body = Main.npc[BirdUtils.FindBody()];
            Vector2 NeckTop = new((head.Center.X + body.Center.X) / 2, body.Center.Y - 600);
            float NeckRot = (NeckTop - body.Center).ToRotation();
            Vector2 Scale = new(Vector2.Distance(NeckTop, body.Center) / 240, 1f);
            Main.spriteBatch.Draw(NeckTex, (NeckTop + body.Center) / 2 - Main.screenPosition, null, Color.White, NeckRot, NeckTex.Size() / 2, Scale, SpriteEffects.None, 0);

            NeckRot = (head.Center - NeckTop).ToRotation() + MathHelper.Pi;
            Scale = new Vector2(Vector2.Distance(NeckTop, head.Center) / 240, 0.75f);
            Main.spriteBatch.Draw(NeckTex, (NeckTop + head.Center) / 2 - Main.screenPosition, null, Color.White, NeckRot, NeckTex.Size() / 2, Scale, SpriteEffects.None, 0);
        }

        internal void DrawWings()
        {
            if (BirdUtils.FindBody() == -1) return;
            NPC body = Main.npc[BirdUtils.FindBody()];
            ApoBirdBody aopbody = body.ModNPC as ApoBirdBody;
            Texture2D WingTex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdWings").Value;
            Main.spriteBatch.Draw(WingTex, body.Center + new Vector2(5, 0) - Main.screenPosition, null, Color.White, -aopbody.WingRot, WingTex.Size(), body.scale * 0.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(WingTex, body.Center + new Vector2(-5, 0) - Main.screenPosition, null, Color.White, aopbody.WingRot, new Vector2(0, WingTex.Height), body.scale * 0.9f, SpriteEffects.FlipHorizontally, 0);
        }

        internal void DrawWingEyes()
        {
            if (BirdUtils.FindBody() == -1) return;
            NPC body = Main.npc[BirdUtils.FindBody()];
            if (CurrentState != State.ApoSky) return;
            EasyDraw.AnotherDraw(BlendState.Additive, Main.GameViewMatrix.ZoomMatrix);
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/NPCs/EyeLight").Value;
            //左
            Vector2 DrawPos = body.Center + new Vector2(5, 0) - Main.screenPosition;
            float ops = (float)Math.Sin((body.ModNPC as ApoBirdBody).WingLight / 100f * MathHelper.Pi);
            float WingRot = (body.ModNPC as ApoBirdBody).WingRot;
            for (int i = 0; i < ApoBirdBody.WingEyePos.Length; i++)
            {
                Vector2 EyePos = ApoBirdBody.WingEyePos[i] * 0.9f;
                EyePos.X = -EyePos.X;
                float EyeRot = ApoBirdBody.WingEyeRot[i];
                Main.spriteBatch.Draw(tex, DrawPos + BirdUtils.RotateVector2(EyePos, -WingRot), null, Color.White * ops, EyeRot, tex.Size() / 2, ApoBirdBody.WingEyeScale[i] * body.scale, SpriteEffects.None, 0);
            }
            //右
            DrawPos = body.Center + new Vector2(-5, 0) - Main.screenPosition;
            for (int i = 0; i < ApoBirdBody.WingEyePos.Length; i++)
            {
                Vector2 EyePos = ApoBirdBody.WingEyePos[i] * 0.9f;
                float EyeRot = -ApoBirdBody.WingEyeRot[i];
                Main.spriteBatch.Draw(tex, DrawPos + BirdUtils.RotateVector2(EyePos, WingRot), null, Color.White * ops, EyeRot, tex.Size() / 2, ApoBirdBody.WingEyeScale[i] * body.scale, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied, Main.GameViewMatrix.ZoomMatrix);
        }

        internal void DrawHead()
        {
            if (BirdUtils.FindHead() == -1) return;
            NPC head = Main.npc[BirdUtils.FindHead()];
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdHead").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/NPCs/EyeBall").Value;
            Main.spriteBatch.Draw(tex, head.Center - Main.screenPosition, null, Color.White, head.rotation, tex.Size() / 2, head.scale, SpriteEffects.None, 0);

            float TargetRot;             //眼球摆动
            if ((Main.npc[BirdUtils.FindBody()].ModNPC as ApoBirdBody).CurrentState == ApoBirdBody.State.ClawAttack)
            {
                TargetRot = MathHelper.Pi / 2 - ApoBirdClaw.IsStrike() * MathHelper.Pi / 4;
            }
            else if ((Main.npc[BirdUtils.FindBody()].ModNPC as ApoBirdBody).CurrentState == ApoBirdBody.State.Beak)
            {
                TargetRot = (Main.LocalPlayer.Center + (Main.npc[BirdUtils.FindBody()].ModNPC as ApoBirdBody).TargetPos - head.Center).ToRotation();
            }
            else if ((Main.npc[BirdUtils.FindBody()].ModNPC as ApoBirdBody).CurrentState == ApoBirdBody.State.Justice)
            {
                TargetRot = -MathHelper.Pi / 2;
            }
            else if ((Main.npc[BirdUtils.FindBody()].ModNPC as ApoBirdBody).CurrentState == ApoBirdBody.State.Lantern)
            {
                TargetRot = (head.ModNPC as ApoBirdHead).EyeRotation + 0.01f;
            }
            else
            {
                TargetRot = (Main.MouseWorld - head.Center).ToRotation();
            }
            (head.ModNPC as ApoBirdHead).EyeRotation = BirdUtils.Rotating((head.ModNPC as ApoBirdHead).EyeRotation, TargetRot, 0.1f);
            Vector2 EyeRelaPos = (head.ModNPC as ApoBirdHead).EyeRotation.ToRotationVector2() * 9;
            Vector2 EyePos = head.Center + BirdUtils.RotateVector2(new Vector2(12, 50), head.rotation) - Main.screenPosition + EyeRelaPos;
            Main.spriteBatch.Draw(tex2, EyePos, null, Color.White, 0, tex2.Size() / 2, head.scale, SpriteEffects.None, 0);
        }

        internal void DrawBody()
        {
            if (BirdUtils.FindBody() == -1) return;
            NPC body = Main.npc[BirdUtils.FindBody()];
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdBody").Value;
            Main.spriteBatch.Draw(tex, body.Center - Main.screenPosition, null, Color.White, body.rotation, new Vector2(tex.Width / 2, 834), body.scale * 0.85f, SpriteEffects.None, 0);
        }

        internal void DrawArms()
        {
            if (BirdUtils.FindArmLeft() == -1) return;
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdClaw").Value;
            NPC Arm = Main.npc[BirdUtils.FindArmLeft()];
            SpriteEffects SP = SpriteEffects.None;
            Main.spriteBatch.Draw(tex, Arm.Center - Main.screenPosition, null, Color.White, Arm.rotation, tex.Size() / 2, Arm.scale * 1.5f, SP, 0);
            if (BirdUtils.FindArmRight() == -1) return;
            Arm = Main.npc[BirdUtils.FindArmRight()];
            SP = SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(tex, Arm.Center - Main.screenPosition, null, Color.White, Arm.rotation, tex.Size() / 2, Arm.scale * 1.5f, SP, 0);
        }

        internal void DrawArmExtras()
        {
            if (BirdUtils.FindBody() == -1) return;
            NPC body = Main.npc[BirdUtils.FindBody()];
            if (BirdUtils.FindArmLeft() == -1 || BirdUtils.FindArmRight() == -1) return;
            DrawArmExtra(body.Center + BirdUtils.RotateVector2(new Vector2(-600, -660), body.rotation), Main.npc[BirdUtils.FindArmLeft()].Center + new Vector2(-220, -640));  //532 1024
            DrawArmExtra(body.Center + BirdUtils.RotateVector2(new Vector2(635, -660), body.rotation), Main.npc[BirdUtils.FindArmRight()].Center + new Vector2(220, -640));
        }

        internal void DrawArmExtra(Vector2 Start, Vector2 End)
        {
            Vector2 DrawPos = (Start + End) / 2 - Main.screenPosition;
            Vector2 Scale = new(Vector2.Distance(Start, End) / 320, 1f);
            float Rot = (End - Start).ToRotation();
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/NPCs/ApoBirdClawExtra").Value;
            Main.spriteBatch.Draw(tex, DrawPos, null, Color.White, Rot, tex.Size() / 2, Scale, SpriteEffects.None, 0);
        }

        internal void DrawBgs(SpriteBatch spriteBatch, Texture2D texBg, float t)
        {
            float BeginDrawX = t * Main.screenWidth;
            float TexSliceX = (1 - t) * texBg.Width;
            spriteBatch.Draw(texBg, new Rectangle((int)BeginDrawX, 0, Main.screenWidth - (int)BeginDrawX, Main.screenHeight), new Rectangle(0, 0, (int)TexSliceX, texBg.Height), Color.White);
            spriteBatch.Draw(texBg, new Rectangle(0, 0, (int)BeginDrawX, Main.screenHeight), new Rectangle((int)TexSliceX, 0, texBg.Width - (int)TexSliceX, texBg.Height), Color.White);
        }

        public static void SummonTheBird()
        {
            if (CurrentState != State.NoSky) return;
            if (Main.LocalPlayer.active && !Main.LocalPlayer.dead && !Main.LocalPlayer.ghost)
            {
                CurrentState = State.Revealing;
                NPC.NewNPC(Main.LocalPlayer.GetSource_ItemUse_WithPotentialAmmo(Main.LocalPlayer.HeldItem, 0), (int)Main.LocalPlayer.Center.X, (int)Main.LocalPlayer.Center.Y, ModContent.NPCType<ApoBirdBody>(), 0, 0, 0, 0, Main.myPlayer);
            }
        }

    }
}