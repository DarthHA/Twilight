using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;

namespace Twilight.Projectiles
{
    public class TwilightBulletExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Light Illuminating the Forest");
            DisplayName.AddTranslation(GameCulture.Chinese, "照亮森林的光芒");
            Main.projFrames[projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.melee = true;
            projectile.ranged = true;
            projectile.magic = true;
            projectile.scale = 1f;
            projectile.timeLeft = 30;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 5;
        }
        public override void AI()
        {
            projectile.ai[1]++;
            if (projectile.ai[1] < 10)
            {
                projectile.Opacity = projectile.ai[1] / 10;
            }
            else
            {
                projectile.Opacity = 1;
            }
            if (projectile.ai[1] < 15)
            {
                projectile.scale = projectile.ai[1] / 15;
            }
            else
            {
                projectile.scale = 1;
            }

            if (projectile.ai[1] < 12)
            {
                projectile.frame = 0;
            }
            else
            {
                projectile.frame = (int)((projectile.ai[1] - 12) / 3) - 1;
                if (projectile.frame > 4) projectile.frame = 4;
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 300);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Rectangle rectangle = new Rectangle(0, 256 * projectile.frame, 256, 256);
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, rectangle, Color.White * projectile.Opacity, 0, rectangle.Size() / 2, projectile.scale * 1.2f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(projectile.Center) < 154 * projectile.scale;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }

}