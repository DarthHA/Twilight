using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.NPCs;
using Twilight.Projectiles;
using Twilight.Sky;

namespace Twilight
{
    public class ApoBirdGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public int Sins = 0;
        public int StunBuffTime = 0;
        public int ParadiseBuffTime = 0;
        public int BleedingBuffTime = 0;

        public Vector2? OldVel = null;
        public int Bleed = 0;

        public static Vector2? OldPlayerPos = null;
        public static Vector2? OldPlayerVel = null;
        public static bool FuckingInvincible = false;
        public override bool PreAI(NPC npc)
        {
            if (SomeUtils.HasStun(npc))
            {
                if (OldVel == null)
                {
                    OldVel = npc.velocity;
                }
                npc.velocity = Vector2.Zero;
                return false;
            }
            else
            {
                if (OldVel != null)
                {
                    npc.velocity = (Vector2)OldVel;
                    OldVel = null;
                }
            }
            if (BirdUtils.IsNotBird(npc) && !npc.friendly && npc.target == Main.myPlayer)
            {
                int lantern2 = EyeLantern2.FindLantern2();
                if (lantern2 != -1)
                {
                    FuckingInvincible = true;
                    OldPlayerPos = Main.LocalPlayer.position;
                    OldPlayerVel = Main.LocalPlayer.velocity;
                    Main.LocalPlayer.velocity = Vector2.Zero;
                    Main.LocalPlayer.Center = Main.projectile[lantern2].Center;
                }
            }

            return true;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!SomeUtils.HasBleeding(npc))
            {
                Bleed = 0;
            }

            if (Bleed > 0)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= Bleed * TwilightData.BleedingDamage;
                if (damage < Bleed * TwilightData.BleedingDamage / 4)
                {
                    damage = Bleed * TwilightData.BleedingDamage / 4;
                }
                if (Main.rand.NextBool(15))
                {
                    Bleed--;
                }
            }
        }

        public override void PostAI(NPC npc)
        {
            if (Sins > 0)
            {
                npc.buffImmune[ModContent.BuffType<SinBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<SinBuff>(), 2);
            }

            if (StunBuffTime > 0)
            {
                StunBuffTime--;
                npc.buffImmune[ModContent.BuffType<TwilightStunnedBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<TwilightStunnedBuff>(), StunBuffTime);
            }

            if (ParadiseBuffTime > 0)
            {
                ParadiseBuffTime--;
                npc.buffImmune[ModContent.BuffType<TwilightParadisedBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<TwilightParadisedBuff>(), ParadiseBuffTime);
            }

            if (BleedingBuffTime > 0)
            {
                BleedingBuffTime--;
                npc.buffImmune[ModContent.BuffType<TwilightBleedingBuff>()] = false;
                npc.AddBuff(ModContent.BuffType<TwilightBleedingBuff>(), BleedingBuffTime);
            }

            FuckingInvincible = false;
            if (OldPlayerPos.HasValue)
            {
                Main.LocalPlayer.position = OldPlayerPos.Value;
                OldPlayerPos = null;
            }

            if (OldPlayerVel.HasValue)
            {
                Main.LocalPlayer.velocity = OldPlayerVel.Value;
                OldPlayerVel = null;
            }
        }


        public override void FindFrame(NPC npc, int frameHeight)
        {
            float VelocityScale = 1;
            if (BirdUtils.FindBody() != -1 && ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                if (npc.CanBeChasedBy())
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
                    {
                        VelocityScale -= 0.25f;
                    }
                    if (Main.npc[BirdUtils.FindBody()].ai[1] == (int)ApoBirdBody.State.Beak)
                    {
                        VelocityScale -= 0.5f;
                    }
                }
            }

            if (SomeUtils.HasStun(npc))
            {
                VelocityScale = 1;

                if (npc.frameCounter > 0)
                {
                    npc.frameCounter--;
                }
            }

            npc.position -= npc.velocity * (1 - VelocityScale);
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (SomeUtils.HasParadise(npc))
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref Player.HurtModifiers modifiers)
        {
            if (SomeUtils.HasParadise(npc))
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }

        public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (Sins > 0)
            {
                int lantern2 = EyeLantern2.FindLantern2();
                if (lantern2 != -1)
                {
                    if (npc.Distance(Main.projectile[lantern2].Center) < TwilightData.SalvationCritRange)
                    {
                        modifiers.ScalingArmorPenetration += 1f;
                    }
                }
            }
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (!BirdUtils.IsNotBird(npc)) return;
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, Sins.ToString(), npc.Center.X - Main.screenPosition.X, npc.Bottom.Y + 10 - Main.screenPosition.Y, Color.Cyan, Color.Black, Vector2.Zero);
            float EnchantedOps = 0;
            float SinOps = 0;
            Texture2D SinTex = ModContent.Request<Texture2D>("Twilight/Buffs/SinMark").Value;
            Texture2D CharmTex = ModContent.Request<Texture2D>("Twilight/Buffs/Enchanted").Value;

            if (Sins > 0)
            {
                SinOps = Sins / 5f;
            }

            int lantern2 = EyeLantern2.FindLantern2();
            if (lantern2 != -1)
            {
                float dist = npc.Distance(Main.projectile[lantern2].Center);
                if (dist < TwilightData.SalvationCritRange)
                {
                    EnchantedOps = 1 - dist / TwilightData.SalvationCritRange;
                }
            }

            if (EnchantedOps > 0 && SinOps > 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(-15, 30) - screenPos;
                spriteBatch.Draw(CharmTex, DrawPos, null, Color.White * EnchantedOps, 0, CharmTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
                DrawPos = npc.Bottom + new Vector2(15, 30) - screenPos;
                spriteBatch.Draw(SinTex, DrawPos, null, Color.White * SinOps, 0, SinTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            if (EnchantedOps > 0 && SinOps == 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(0, 30) - screenPos;
                spriteBatch.Draw(CharmTex, DrawPos, null, Color.White * EnchantedOps, 0, CharmTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            if (EnchantedOps == 0 && SinOps > 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(0, 30) - screenPos;
                spriteBatch.Draw(SinTex, DrawPos, null, Color.White * SinOps, 0, SinTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
        }


    }
}