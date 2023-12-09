using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class TwilightSummon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 10;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || owner.ghost)
            {
                Projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<TwilightItem>())
            {
                Projectile.Kill();
                return;
            }
            Projectile.rotation = -MathHelper.Pi / 2;
            owner.itemTime = 15;
            owner.itemAnimation = 15;
            owner.direction = Math.Sign(Projectile.velocity.X + 0.01f);
            owner.heldProj = Projectile.whoAmI;
            Projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                Projectile.Center = owner.MountedCenter;
            }

            owner.itemRotation = (float)Math.Atan2(Projectile.rotation.ToRotationVector2().Y * owner.direction, Projectile.rotation.ToRotationVector2().X * owner.direction);

            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Projectile.Opacity = Projectile.ai[1] / 10;
            }
            else
            {
                Projectile.Opacity = 1;
            }

            if (Projectile.ai[1] >= 40)
            {
                ApoBirdSky.SummonTheBird();
                Projectile.Kill();
                return;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 origin = new(90, 52);
            if (Projectile.ai[1] > 10)
            {
                float k = (float)Math.Sin((Projectile.ai[1] - 10) / 30 * MathHelper.Pi) / 4;
                Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.3f, Projectile.rotation, origin, Projectile.scale * (0.4f + k), SP, 0);
                Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity * 0.3f, Projectile.rotation, origin, Projectile.scale * (0.4f + k / 2), SP, 0);
            }
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.4f, SP, 0);

            return false;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}