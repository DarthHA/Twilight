using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class JustitiaEffect2 : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/Justice4";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.Opacity = Projectile.ai[1] / 10;
            }
            else
            {
                Projectile.Opacity = 1;
            }
            if (Projectile.ai[1] < 15)
            {
                Projectile.scale = Projectile.ai[1] / 15;
            }
            else
            {
                Projectile.scale = 1;
            }

            if (Projectile.ai[1] < 12)
            {
                Projectile.frame = 0;
            }
            else
            {
                Projectile.frame = (int)((Projectile.ai[1] - 12) / 3) - 1;
                if (Projectile.frame > 4) Projectile.frame = 4;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice4").Value;
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Rectangle rectangle = new Rectangle(0, 256 * Projectile.frame, 256, 256);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rectangle, Color.White * Projectile.Opacity, 0, rectangle.Size() / 2, Projectile.scale * 4f, SpriteEffects.None, 0);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }

}