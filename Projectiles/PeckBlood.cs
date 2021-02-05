using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class PeckBlood : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/PeckBlood1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Peck Blood");
            DisplayName.AddTranslation(GameCulture.Chinese, "小喙血溅");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1.0f;
            projectile.friendly = false;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 0;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (projectile.ai[0] == 0)
            {
                projectile.ai[0] = Main.rand.Next(3) + 1;
            }
            projectile.ai[1]++;
            if (projectile.ai[1] < 10)
            {
                projectile.Opacity = projectile.ai[1] / 10;
            }
            else
            {
                projectile.Opacity = (60f - projectile.ai[1]) / 50;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] > 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                Texture2D tex = mod.GetTexture("Projectiles/PeckBlood" + projectile.ai[0].ToString());
                spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, 0, tex.Size() / 2, projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}