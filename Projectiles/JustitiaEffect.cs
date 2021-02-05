using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class JustitiaEffect : ModProjectile
    {
        public static int ArmLength = 135;
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
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 1;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }
        public override void AI()
        {
            if (Twilight.FindHead() == -1 || ApoBirdSky.CurrentState != 2)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = Main.npc[Twilight.FindHead()].Center + new Vector2(0, -100);
            }
            projectile.ai[1]++;
            if (projectile.ai[1] < 30)   //0-30显形，30-60静止，60-70旋转，70-120惯性，120-160静止，160-190消失
            {
                projectile.Opacity = projectile.ai[1] / 30;
            }
            else if (projectile.ai[1] < 60)
            {
                projectile.Opacity = 1;
            }
            else if (projectile.ai[1] < 70)
            {
                projectile.rotation += MathHelper.Pi / 50;
                ArmRot = MathHelper.Pi / 6 * (projectile.ai[1] - 60) / 10;
            }
            else if (projectile.ai[1] < 120)
            {
                float k = (120f - projectile.ai[1]) / 50 * MathHelper.Pi / 6;
                float a = (projectile.ai[1] - 70f) / 10f * MathHelper.Pi;
                ArmRot = k * (float)Math.Cos(a);
            }
            else if (projectile.ai[1] < 160)
            {
                ArmRot = 0;
            }
            else
            {
                projectile.Opacity = (190f - projectile.ai[1]) / 30;
                if (projectile.ai[1] >= 190)
                {
                    projectile.Kill();
                    return;
                }
            }
            if (projectile.ai[1] == 70)
            {
                int dmg = (int)(Twilight.JusticeDamage * TwilightPlayer.GetDamageBonus(Main.player[projectile.owner]));
                Projectile.NewProjectile(Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<JusticeDamage>(), dmg, 0, projectile.owner);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/JusticeOn"),Main.player[projectile.owner].Center);
            }
            if (projectile.ai[1] == 60)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect2>(), 0, 0, projectile.owner);
            }
            //projectile.rotation += 0.05f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex1 = mod.GetTexture("Projectiles/Justice1");
            Texture2D tex2 = mod.GetTexture("Projectiles/Justice2");
            Texture2D tex3 = mod.GetTexture("Projectiles/Justice3");
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex1.Size() / 2, projectile.scale * 3f, SpriteEffects.None, 0);
            Vector2 DrawPos = projectile.Center + projectile.rotation.ToRotationVector2() * ArmLength * projectile.scale - Main.screenPosition;
            spriteBatch.Draw(tex2, DrawPos, null, Color.White * projectile.Opacity, ArmRot, new Vector2(tex2.Width / 2, 0), projectile.scale * 3f, SpriteEffects.None, 0);

            DrawPos = projectile.Center - projectile.rotation.ToRotationVector2() * ArmLength * projectile.scale - Main.screenPosition;
            spriteBatch.Draw(tex2, DrawPos, null, Color.White * projectile.Opacity, -ArmRot, new Vector2(tex2.Width / 2, 0), projectile.scale * 3f, SpriteEffects.None, 0);

            spriteBatch.Draw(tex3, projectile.Center + new Vector2(0, -10) - Main.screenPosition, null, Color.White * projectile.Opacity, 0, tex3.Size() / 2, projectile.scale / 2, SpriteEffects.None, 0);
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