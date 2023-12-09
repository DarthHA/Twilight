using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class JustitiaEffect3 : ModProjectile
    {
        public override string Texture => "Twilight/Buffs/SinMark";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BrainOfConfusion);
            Projectile.scale = 0.5f;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.BrainOfConfusion;
        }

        public override void AI()
        {
            Projectile.frame = 0;
            Projectile.frameCounter = 0;
        }
        public override void PostAI()
        {
            Projectile.position -= Projectile.velocity / 2;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = texture2D13.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }
    }
}