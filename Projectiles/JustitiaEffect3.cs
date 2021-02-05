using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class JustitiaEffect3 : ModProjectile
    {
        public override string Texture => "Twilight/Buffs/SinMark";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sin Mark");
            DisplayName.AddTranslation(GameCulture.Chinese, "罪痕");
        }

        public override void SetDefaults()
        {
            projectile.CloneDefaults(ProjectileID.BrainOfConfusion);
            projectile.scale = 0.5f;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.BrainOfConfusion;
        }

        public override void AI()
        {
            projectile.frame = 0;
            projectile.frameCounter = 0;
        }
        public override void PostAI()
        {
            projectile.position -= projectile.velocity / 2;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            Texture2D texture2D13 = Main.projectileTexture[projectile.type];
            int num156 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Rectangle?(rectangle), Color.White * projectile.Opacity, projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }
    }
}