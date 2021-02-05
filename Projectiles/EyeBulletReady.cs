using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace Twilight.Projectiles
{
    public class EyeBulletReady : ModProjectile
    {
        public Vector2 Pos = Vector2.Zero;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄暝");
        }
        public override void SetDefaults()          //280 400  scale = 0.4
        {
            projectile.width = 112;
            projectile.height = 112;
            projectile.scale = 1f;
            projectile.timeLeft = 6000;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 100;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
        }
        public override void AI()
        {

            if (Twilight.FindBody() == -1)
            {
                projectile.Kill();
                return;
            }
            if (Pos == Vector2.Zero)
            {
                Pos = projectile.Center - Main.npc[Twilight.FindBody()].Center;
            }
            else
            {
                projectile.Center = Main.npc[Twilight.FindBody()].Center + Pos;
            }

            projectile.ai[0]++;
            if (projectile.ai[0] > 60)
            {
                projectile.Kill();
                return;
            }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Texture2D tex1 = mod.GetTexture("Projectiles/EyeBullet1");
            Texture2D tex2 = mod.GetTexture("Projectiles/EyeBullet2");
            Texture2D tex3 = mod.GetTexture("Projectiles/EyeBullet3");
            float ops1, ops2;
            if (projectile.ai[0] < 0)
            {
                ops1 = 0;
                ops2 = 0;
            }
            else if (projectile.ai[0] < 30)
            {
                ops1 = projectile.ai[0] / 30;
                ops2 = 0;
                if (projectile.ai[0] == 0)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EyeBulletStart"), Main.player[projectile.owner].Center);
                }
            }
            else
            {
                ops1 = (60 - projectile.ai[0]) / 30;
                ops2 = (projectile.ai[0] - 30) / 30;
            }
            spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.White * ops1, projectile.rotation, tex1.Size() / 2, projectile.scale * 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * ops2, projectile.rotation, tex2.Size() / 2, projectile.scale * 0.3f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(tex3, projectile.Center - Main.screenPosition, null, Color.White * ops2 * 0.6f, 0, tex3.Size() / 2, projectile.scale * 0.45f, SpriteEffects.None, 0);


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void Kill(int timeLeft)
        {
            int protmp = Projectile.NewProjectile(projectile.Center, (MathHelper.TwoPi * Main.rand.NextFloat()).ToRotationVector2() * 15, ModContent.ProjectileType<EyeBulletShot>(), projectile.damage, 0, projectile.owner);
            Main.projectile[protmp].localAI[0] = Pos.X;
            Main.projectile[protmp].localAI[1] = Pos.Y;
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EyeBulletFire"), Main.player[projectile.owner].Center);
        }

        public override bool CanDamage()
        {
            return false;
        }

        public static bool AnyBulletReady()
        {
            foreach (Projectile bullet in Main.projectile)
            {
                if (bullet.active && bullet.type == ModContent.ProjectileType<EyeBulletReady>())
                {
                    return true;
                }
            }
            return false;
        }
    }
}