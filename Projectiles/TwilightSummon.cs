using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Items;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class TwilightSummon : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Summon The Bird!");
            DisplayName.AddTranslation(GameCulture.Chinese, "召唤鸟！");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 255;
        }

        public override void AI()
        {
            Player owner = Main.player[projectile.owner];
            if (!owner.active || owner.dead || owner.ghost)
            {
                projectile.Kill();
                return;
            }
            if (owner.HeldItem.type != ModContent.ItemType<TwilightItem>())
            {
                projectile.Kill();
                return;
            }
            projectile.rotation = -MathHelper.Pi / 2;
            owner.itemTime = 15;
            owner.itemAnimation = 15;
            owner.direction = Math.Sign(projectile.velocity.X + 0.01f);
            owner.heldProj = projectile.whoAmI;
            projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                projectile.Center = owner.MountedCenter;
            }

            owner.itemRotation = (float)Math.Atan2(projectile.rotation.ToRotationVector2().Y * owner.direction, projectile.rotation.ToRotationVector2().X * owner.direction);

            projectile.ai[1]++;
            if (projectile.ai[1] < 10)
            {
                projectile.Opacity = projectile.ai[1] / 10;
            }
            else
            {
                projectile.Opacity = 1;
            }
            
            if (projectile.ai[1] >= 40)
            {
                ApoBirdSky.SummonTheBird();
                projectile.Kill();
                return;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player owner = Main.player[projectile.owner];
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 origin = new Vector2(90, 52);
            if (projectile.ai[1] > 10)
            {
                float k = (float)Math.Sin((projectile.ai[1] - 10) / 30 * MathHelper.Pi) / 4;
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity * 0.3f, projectile.rotation, origin, projectile.scale * (0.4f + k), SP, 0);
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity * 0.3f, projectile.rotation, origin, projectile.scale * (0.4f + k / 2), SP, 0);
            }
            spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, origin, projectile.scale * 0.4f, SP, 0);
            
            return false;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}