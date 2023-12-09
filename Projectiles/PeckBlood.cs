using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class PeckBlood : ModProjectile
    {
        public override string Texture => "Twilight/Projectiles/PeckBlood1";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1.0f;
            Projectile.friendly = false;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 0;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.ai[0] = Main.rand.Next(3) + 1;
                for (int i = 0; i < 10; i++)
                {
                    Vector2 Vel = (Main.rand.NextFloat() * MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(10, 20);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + Vector2.Normalize(Vel) * Main.rand.Next(1, 100), Vel, ModContent.ProjectileType<PeckSplit>(), 0, 0, Projectile.owner);
                }
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.Opacity = Projectile.ai[1] / 10;
            }
            else
            {
                Projectile.Opacity = (60f - Projectile.ai[1]) / 50;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] > 0)
            {
                EasyDraw.AnotherDraw(BlendState.Additive);
                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

                Texture2D tex = ModContent.Request<Texture2D>("Twilight/Projectiles/PeckBlood" + Projectile.ai[0].ToString()).Value;
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

                EasyDraw.AnotherDraw(BlendState.AlphaBlend);
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}