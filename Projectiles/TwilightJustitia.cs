using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class TwilightJustitia : ModProjectile
    {
        public static int ArmLength = 45;
        public float ArmRot = 0;
        public override string Texture => "Twilight/Projectiles/Justice1";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Justitia");
            DisplayName.AddTranslation(GameCulture.Chinese, "正义裁决");
        }
        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.timeLeft = 150;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 1;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            Player owner = Main.player[projectile.owner];
            if (!owner.active && owner.dead && owner.ghost)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = owner.Center + new Vector2(0, -35) + new Vector2(70, 0) * owner.direction;
            }
            projectile.ai[1]++;
            if (projectile.ai[1] < 10) 
            {
                projectile.Opacity = projectile.ai[1] / 10;
            }
            else if (projectile.ai[1] < 35)
            {
                projectile.Opacity = 1;
            }
            else if (projectile.ai[1] < 45)
            {
                projectile.rotation += MathHelper.Pi / 50;
                ArmRot = MathHelper.Pi / 6 * (projectile.ai[1] - 35) / 10;
            }
            else if (projectile.ai[1] < 70)
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
                projectile.Opacity = (80f - projectile.ai[1]) / 10;
                if (projectile.ai[1] >= 90)
                {
                    projectile.Kill();
                    return;
                }
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex1 = mod.GetTexture("Projectiles/Justice1");
            Texture2D tex2 = mod.GetTexture("Projectiles/Justice2");
            Texture2D tex3 = mod.GetTexture("Projectiles/Justice3");
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex1.Size() / 2, projectile.scale, SpriteEffects.None, 0);
            Vector2 DrawPos = projectile.Center + projectile.rotation.ToRotationVector2() * ArmLength * projectile.scale - Main.screenPosition;
            spriteBatch.Draw(tex2, DrawPos, null, Color.White * projectile.Opacity, ArmRot, new Vector2(tex2.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);

            DrawPos = projectile.Center - projectile.rotation.ToRotationVector2() * ArmLength * projectile.scale - Main.screenPosition;
            spriteBatch.Draw(tex2, DrawPos, null, Color.White * projectile.Opacity, -ArmRot, new Vector2(tex2.Width / 2, 0), projectile.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(tex3, projectile.Center + new Vector2(0, -10) - Main.screenPosition, null, Color.White * projectile.Opacity, 0, tex3.Size() / 2, projectile.scale / 6, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }

}