using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.UI;

namespace Twilight.Projectiles
{
    public class PeckCenter : ModProjectile
    {
        bool OnHit = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Small Peck");
            DisplayName.AddTranslation(GameCulture.Chinese, "小喙");
        }
        public override void SetDefaults()
        {
            projectile.width = 300;
            projectile.height = 300;
            projectile.scale = 1.0f;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.timeLeft = 6000;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 999999;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 60;
            projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            projectile.friendly = true;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 99999;   //40

            if (Twilight.FindBody() == -1)
            {
                projectile.Kill();
                return;
            }
            projectile.Center = Main.npc[Twilight.FindBody()].Center + new Vector2(0, 138);

            projectile.ai[0]++;
            if (projectile.ai[0] < 10)
            {
                projectile.Opacity = projectile.ai[0] / 10;
            }
            else if (projectile.ai[0] < 30)
            {

            }
            else if (projectile.ai[0] < 35)
            {
                projectile.Opacity = 1;
            }
            else
            {
                projectile.Opacity = (55 - projectile.ai[0]) / 20;
                if (projectile.ai[0] >= 55)
                {
                    projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                    projectile.Kill();
                    return;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            Texture2D Tex1 = Main.projectileTexture[projectile.type];
            Texture2D[] Tex2 = new Texture2D[6];
            for (int i = 0; i < Tex2.Length; i++)
            {
                Tex2[i] = mod.GetTexture("Projectiles/Peck" + (i + 1).ToString());
            }
            float r;
            if (projectile.ai[0] < 10)
            {
                r = 100 + 400f * projectile.ai[0] / 10;
            }
            else if (projectile.ai[0] < 30)
            {
                r = 500;
            }
            else if (projectile.ai[0] < 35)
            {
                r = 100f + 400f * (35f - projectile.ai[0]) / 5;
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
                    spriteBatch.Draw(Tex2[i], projectile.Center + rot.ToRotationVector2() * j - Main.screenPosition, null, Color.White * projectile.Opacity, 0, Tex2[i].Size() / 2, projectile.scale * scale * 0.8f, SpriteEffects.None, 0);
                }
            }
            spriteBatch.Draw(Tex1, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, 0, Tex1.Size() / 2, projectile.scale * 0.6f, SpriteEffects.None, 0);
            for (int i = 0; i < Tex2.Length; i++)
            {
                float rot = MathHelper.TwoPi / 6 * i - MathHelper.Pi / 2;
                spriteBatch.Draw(Tex2[i], projectile.Center + rot.ToRotationVector2() * r - Main.screenPosition, null, Color.White * projectile.Opacity, 0, Tex2[i].Size() / 2, projectile.scale * 0.8f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }
        public override bool CanDamage()
        {
            if (projectile.ai[0] >= 30) return true;
            return false;
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool CanHitPlayer(Player target) => false;
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.Distance(target.Center) < 100)
            {
                Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 200, 0, Main.player[projectile.owner].statLifeMax2);
                Main.player[projectile.owner].HealEffect(200);
                damage *= 10;

            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (projectile.Distance(target.Center) < 100)
            {
                if (!OnHit)
                {
                    OnHit = true;
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PeckBlood>(), 0, 0);
                    Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(15, 1);
                    for (int i = 0; i < 150; i++)
                    {
                        Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightBleedingBuff>(), 600, true);
                    }
                    
                }

            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projectile.Distance(targetHitbox.Center.ToVector2()) < 100) return true;
            float r;
            if (projectile.ai[0] < 10)
            {
                r = 100 + 400f * projectile.ai[0] / 10;
            }
            else if (projectile.ai[0] < 30)
            {
                r = 500;
            }
            else if (projectile.ai[0] < 35)
            {
                r = 100f + 400f * (35f - projectile.ai[0]) / 5;
            }
            else
            {
                r = 100;
            }
            for (int i = 0; i < 6; i++)
            {
                Vector2 Center = projectile.Center + (MathHelper.TwoPi / 6 * i - MathHelper.Pi / 2).ToRotationVector2() * r;
                if (Vector2.Distance(Center, targetHitbox.Center.ToVector2()) <= 80)
                {
                    return true;
                }
            }
            return false;
        }
    }
}