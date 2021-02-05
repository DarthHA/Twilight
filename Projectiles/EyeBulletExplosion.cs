using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.UI;

namespace Twilight.Projectiles
{
    public class EyeBulletExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄暝");
        }
        public override void SetDefaults()
        {
            projectile.width = 400;
            projectile.height = 200;
            projectile.scale = 1.2f;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 100;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 6;
            projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 99999;   //40

            projectile.ai[1]++;
            if (projectile.ai[1] == 1 && Main.LocalPlayer.Distance(projectile.Center) < 1100)
            {
                Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(5, 0.5f);
            }

            if (projectile.ai[1] > 30)
            {
                projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                projectile.Kill();
                return;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 1, 0, Main.player[projectile.owner].statLifeMax2);
            Main.player[projectile.owner].HealEffect(1);
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 480);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            if (projectile.ai[1] > 0)
            {
                Texture2D tex2 = Main.projectileTexture[projectile.type];
                Texture2D tex3 = mod.GetTexture("Projectiles/MeleeEffect3");
                float ops;
                if (projectile.ai[1] < 10)
                {
                    ops = projectile.ai[1] / 10;
                }
                else if (projectile.ai[1] < 25)
                {
                    ops = (25 - projectile.ai[1]) / 15;
                }
                else
                {
                    ops = 0;
                }
                spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * ops, 0, tex2.Size() / 2, projectile.scale * 0.9f, SpriteEffects.None, 0);

                if (projectile.ai[1] > 5)
                {
                    float ops2 = 1;
                    float scale = projectile.scale * (projectile.ai[1] - 5) / 15f * 0.4f;
                    if (projectile.ai[1] >= 20)
                    {
                        ops2 = (30f - projectile.ai[1]) / 10f;
                    }

                    spriteBatch.Draw(tex3, projectile.Center - Main.screenPosition, null, Color.White * ops2, 0, tex3.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }




    }
}