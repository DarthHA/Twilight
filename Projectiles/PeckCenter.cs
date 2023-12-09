using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;
using Twilight.UI;

namespace Twilight.Projectiles
{
    public class PeckCenter : ModProjectile
    {
        bool OnHit = false;
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.scale = 1.0f;
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.timeLeft = 6000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 999999;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            Projectile.friendly = true;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;   //40

            if (BirdUtils.FindBody() == -1)
            {
                Projectile.Kill();
                return;
            }
            Projectile.Center = Main.npc[BirdUtils.FindBody()].Center + new Vector2(0, 138);

            Projectile.ai[0]++;

            if (Projectile.ai[0] == 30)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 20);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + Vector2.Normalize(Vel) * Main.rand.Next(1, 100), Vel, ModContent.ProjectileType<PeckSplit>(), 0, 0, Projectile.owner);
                }
            }

            if (Projectile.ai[0] < 10)
            {
                Projectile.Opacity = Projectile.ai[0] / 10;
            }
            else if (Projectile.ai[0] < 30)
            {

            }
            else if (Projectile.ai[0] < 35)
            {
                Projectile.Opacity = 1;
            }
            else
            {
                Projectile.Opacity = (55 - Projectile.ai[0]) / 20;
                if (Projectile.ai[0] >= 55)
                {
                    Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                    Projectile.Kill();
                    return;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            Texture2D Tex1 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D[] Tex2 = new Texture2D[6];
            for (int i = 0; i < Tex2.Length; i++)
            {
                Tex2[i] = ModContent.Request<Texture2D>("Twilight/Projectiles/Peck" + (i + 1).ToString()).Value;
            }
            float r;
            if (Projectile.ai[0] < 10)
            {
                r = 100 + 400f * Projectile.ai[0] / 10;
            }
            else if (Projectile.ai[0] < 30)
            {
                r = 500;
            }
            else if (Projectile.ai[0] < 35)
            {
                r = 100f + 400f * (35f - Projectile.ai[0]) / 5;
            }
            else
            {
                r = 100;
            }

            for (int i = 0; i < Tex2.Length; i++)
            {
                for (int j = 20; j < r; j += 20)
                {
                    float scale = (float)Math.Pow(j / (r * 2), 2) * 2 + 0.5f;
                    float rot = MathHelper.TwoPi / 6 * i - MathHelper.Pi / 2;
                    Main.spriteBatch.Draw(Tex2[i], Projectile.Center + rot.ToRotationVector2() * j - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, Tex2[i].Size() / 2, Projectile.scale * scale * 0.8f, SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Draw(Tex1, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, Tex1.Size() / 2, Projectile.scale * 0.6f, SpriteEffects.None, 0);
            for (int i = 0; i < Tex2.Length; i++)
            {
                float rot = MathHelper.TwoPi / 6 * i - MathHelper.Pi / 2;
                Main.spriteBatch.Draw(Tex2[i], Projectile.Center + rot.ToRotationVector2() * r - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, Tex2[i].Size() / 2, Projectile.scale * 0.8f, SpriteEffects.None, 0);
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] >= 30) return true;
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool CanHitPlayer(Player target) => false;
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Distance(target.Center) < 150)
            {
                Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 200, 0, Main.player[Projectile.owner].statLifeMax2);
                Main.player[Projectile.owner].HealEffect(200);
                modifiers.FinalDamage *= 10f;
                modifiers.SetCrit();
                modifiers.ScalingArmorPenetration += 1f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.Distance(target.Center) < 150)
            {
                if (!OnHit)
                {
                    OnHit = true;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PeckBlood>(), 0, 0);
                    Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(15, 1);
                    for (int i = 0; i < 200; i++)
                    {
                        SomeUtils.ApplyBleeding(target, 600);
                        //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightBleedingBuff>(), 600, true);
                    }

                }

            }

        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (Projectile.Distance(targetHitbox.Center.ToVector2()) < 100) return true;
            float r;
            if (Projectile.ai[0] < 10)
            {
                r = 100 + 400f * Projectile.ai[0] / 10;
            }
            else if (Projectile.ai[0] < 30)
            {
                r = 500;
            }
            else if (Projectile.ai[0] < 35)
            {
                r = 100f + 400f * (35f - Projectile.ai[0]) / 5;
            }
            else
            {
                r = 100;
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 Center = Projectile.Center + (MathHelper.TwoPi / 6 * i - MathHelper.Pi / 2).ToRotationVector2() * r;
                if (Vector2.Distance(Center, targetHitbox.Center.ToVector2()) <= 80)
                {
                    return true;
                }
            }
            return false;
        }
    }
}