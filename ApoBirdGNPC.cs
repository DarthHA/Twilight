using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Twilight.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Twilight.Sky;
using Twilight.Buffs;
using Twilight.Items;

namespace Twilight
{
    public class ApoBirdGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public int Sins = 0;
        public Vector2 OldVel = Vector2.Zero;
        public bool Stunned = false;
        public int Bleed = 0;
        public override bool PreAI(NPC npc)
        {
            if (npc.HasBuff(ModContent.BuffType<TwilightStunnedBuff>()))
            {
                if (!Stunned)
                {
                    OldVel = npc.velocity;
                }
                Stunned = true;
                npc.velocity = Vector2.Zero;
                return false;
            }
            else
            {
                if (Stunned)
                {
                    npc.velocity = OldVel;
                }
                Stunned = false;
            }
            return true;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!npc.HasBuff(ModContent.BuffType<TwilightBleedingBuff>()))
            {
                Bleed = 0;
            }
            if (Bleed > 0)
            {
                if (npc.lifeRegen > 0)
                {
                    npc.lifeRegen = 0;
                }
                npc.lifeRegen -= Bleed * Twilight.BleedingDamage;
                damage = Bleed * Twilight.BleedingDamage / 4;
                if (Main.rand.Next(10) == 1)
                {
                    Bleed--;
                }
            }
        }
        public override void AI(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<SinBuff>()))
            {
                Sins = 0;
            }
            if (FakeModPlayer.Initialised)
            {
                if (Main.player[EyeLantern.FakePlayer].active)
                {
                    npc.target = EyeLantern.FakePlayer;
                }
            }
        }
        public override void FindFrame(NPC npc, int frameHeight)
        {
            float VelocityScale = 1;
            if (Twilight.FindBody() != -1 && ApoBirdSky.CurrentState == 2)
            {
                if (npc.CanBeChasedBy())
                {
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<SmallPeakBuff>()))
                    {
                        VelocityScale -= 0.25f;
                    }
                    if (Main.npc[Twilight.FindBody()].ai[1] == 3)
                    {
                        VelocityScale -= 0.25f;
                    }
                    if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()))
                    {
                        if (Sins > 0)
                        {
                            if (FakeModPlayer.AnyLanterns2() != -1)
                            {
                                if (Main.player[EyeLantern.FakePlayer].Distance(npc.Center) < Twilight.SalvationCritRange)
                                {
                                    VelocityScale -= 0.05f * Sins;
                                }
                            }
                        }
                    }
                }
            }
            if (Stunned)
            {
                VelocityScale = 1;
            }
            npc.position -= npc.velocity * (1 - VelocityScale);
        }

        public override void ModifyHitNPC(NPC npc, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (target.HasBuff(ModContent.BuffType<TwilightParadisedBuff>()))
            {
                damage /= 2;
            }
        }

        public override void ModifyHitPlayer(NPC npc, Player target, ref int damage, ref bool crit)
        {
            if (target.HasBuff(ModContent.BuffType<TwilightParadisedBuff>()))
            {
                damage /= 2;
            }
        }

        public override bool StrikeNPC(NPC npc, ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (Sins > 0)
            {
                if (FakeModPlayer.AnyLanterns2() != -1)
                {
                    if (Main.player[EyeLantern.FakePlayer].Distance(npc.Center) < 600)
                    {
                        damage += defense / 2;
                    }
                }
            }
            return true;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            //Utils.DrawBorderStringFourWay(Main.spriteBatch, Main.fontMouseText, Sins.ToString(), npc.Center.X - Main.screenPosition.X, npc.Bottom.Y + 10 - Main.screenPosition.Y, Color.Cyan, Color.Black, Vector2.Zero);
            float EnchantedOps = 0;
            float SinOps = 0;
            Texture2D SinTex = mod.GetTexture("Buffs/SinMark");
            Texture2D CharmTex = mod.GetTexture("Buffs/Enchanted");
            if (FakeModPlayer.AnyLanterns2() != -1 && npc.CanBeChasedBy())
            {
                EnchantedOps = Utils.Clamp(Twilight.SalvationCritRange - npc.Distance(Main.player[EyeLantern.FakePlayer].Center), 0, Twilight.SalvationCritRange) / Twilight.SalvationCritRange;
                EnchantedOps = Utils.Clamp(EnchantedOps * 2, 0, 1);
            }

            if (Sins > 0)
            {
                SinOps = Sins / 5f;
            }
            if (EnchantedOps > 0 && SinOps > 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(-15, 30) - Main.screenPosition;
                spriteBatch.Draw(CharmTex, DrawPos, null, Color.White * EnchantedOps, 0, CharmTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
                DrawPos = npc.Bottom + new Vector2(15, 30) - Main.screenPosition;
                spriteBatch.Draw(SinTex, DrawPos, null, Color.White * SinOps, 0, SinTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            if (EnchantedOps > 0 && SinOps == 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(0, 30) - Main.screenPosition;
                spriteBatch.Draw(CharmTex, DrawPos, null, Color.White * EnchantedOps, 0, CharmTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            if (EnchantedOps == 0 && SinOps > 0)
            {
                Vector2 DrawPos = npc.Bottom + new Vector2(0, 30) - Main.screenPosition;
                spriteBatch.Draw(SinTex, DrawPos, null, Color.White * SinOps, 0, SinTex.Size() / 2, 0.167f, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
        }

    }
}