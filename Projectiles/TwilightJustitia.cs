using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class TwilightJustitia : ModProjectile
    {
        public static int ArmLength = 45;
        public float ArmRot = 0;
        public override string Texture => "Twilight/Projectiles/Justice1";
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 1;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active && owner.dead && owner.ghost)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = owner.Center + new Vector2(0, -35) + new Vector2(70, 0) * owner.direction;
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.Opacity = Projectile.ai[1] / 10;
            }
            else if (Projectile.ai[1] < 35)
            {
                Projectile.Opacity = 1;
            }
            else if (Projectile.ai[1] < 45)
            {
                Projectile.rotation += MathHelper.Pi / 50;
                ArmRot = MathHelper.Pi / 6 * (Projectile.ai[1] - 35) / 10;
            }
            else if (Projectile.ai[1] < 70)
            {
                if (ArmRot > 0)
                {
                    ArmRot -= MathHelper.Pi / 60;
                }
                else
                {
                    ArmRot = 0;
                }
            }
            else
            {
                Projectile.Opacity = (80f - Projectile.ai[1]) / 10;
                if (Projectile.ai[1] >= 90)
                {
                    Projectile.Kill();
                    return;
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice1").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice2").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice3").Value;
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(tex1, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex1.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
            Vector2 DrawPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * ArmLength * Projectile.scale - Main.screenPosition;
            Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White * Projectile.Opacity, ArmRot, new Vector2(tex2.Width / 2, 0), Projectile.scale, SpriteEffects.None, 0);

            DrawPos = Projectile.Center - Projectile.rotation.ToRotationVector2() * ArmLength * Projectile.scale - Main.screenPosition;
            Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White * Projectile.Opacity, -ArmRot, new Vector2(tex2.Width / 2, 0), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(tex3, Projectile.Center + new Vector2(0, -10) - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex3.Size() / 2, Projectile.scale / 6, SpriteEffects.None, 0);
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