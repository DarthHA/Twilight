using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;

namespace Twilight.Projectiles
{
    public class TwilightBulletExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.scale = 1f;
            Projectile.timeLeft = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
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
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SomeUtils.DeepApplyParadise(target, 300);
            //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Rectangle rectangle = new Rectangle(0, 256 * Projectile.frame, 256, 256);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rectangle, Color.White * Projectile.Opacity, 0, rectangle.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return targetHitbox.Distance(Projectile.Center) < 154 * Projectile.scale;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }

}