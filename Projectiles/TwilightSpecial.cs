using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;
namespace Twilight.Projectiles
{
    public class TwilightSpecial : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.friendly = false;
            Projectile.hostile = false;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.itemTime = 10;
            owner.itemAnimation = 10;
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
                Projectile.Kill();
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Player owner = Main.player[Projectile.owner];
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 origin = new Vector2(90, 52);
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, origin, Projectile.scale * 0.4f, SP, 0);
            return false;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}