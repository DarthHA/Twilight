using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.Items;
namespace Twilight.Projectiles
{
    public class TwilightStrike : ModProjectile
    {
        //public static readonly float OffSet = 0;
        public float BladeRot = 0;
        public float BladeAlpha = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight Strike");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄瞑纵劈");

        }

        public override void SetDefaults()  //512  512   16  0.75
        {
            projectile.melee = true;
            projectile.ranged = true;
            projectile.magic = true;
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.timeLeft = 100;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 100;
        }
        public override void AI()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.penetrate = -1;
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
            //projectile.rotation = projectile.velocity.ToRotation();
            owner.itemTime = 15;
            owner.itemAnimation = 15;
            owner.direction = Math.Sign(projectile.velocity.X + 0.01f);
            owner.heldProj = projectile.whoAmI;
            //int dir = owner.direction;
            //owner.itemRotation = (float)Math.Atan2(projectile.rotation.ToRotationVector2().Y * dir, projectile.rotation.ToRotationVector2().X * dir);
            projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                projectile.Center = owner.MountedCenter;
            }
            if (projectile.ai[0] == 0)
            {
                projectile.extraUpdates = 1;
                projectile.ai[1]++;
                if (owner.direction > 0)
                {
                    BladeRot = -MathHelper.Pi / 4 * 3 + MathHelper.Pi * projectile.ai[1] / 8f;
                    
                }
                else
                {
                    BladeRot = -MathHelper.Pi / 4 - MathHelper.Pi * projectile.ai[1] / 8f;
                }
                BladeAlpha = projectile.ai[1] / 16f;
                projectile.Opacity = projectile.ai[1] / 8f;
                if (projectile.ai[1] >= 8)
                {
                    projectile.ai[0] = 1;
                    projectile.ai[1] = 0;
                }
            }
            else if (projectile.ai[0] == 1)
            {
                projectile.extraUpdates = 0;
                BladeAlpha = 1;
                projectile.ai[1]++;
                if (owner.direction > 0)
                {
                    BladeRot = MathHelper.Pi / 4;

                }
                else
                {
                    BladeRot = MathHelper.Pi / 4 * 3;
                }

                projectile.Opacity = 1 - projectile.ai[1] / 30;
                if (projectile.ai[1] >= 30)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                    return;
                }
            }
            owner.itemRotation = (float)Math.Atan2(BladeRot.ToRotationVector2().Y * owner.direction, BladeRot.ToRotationVector2().X * owner.direction);

        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightStunnedBuff>(), 65);
            float distance = target.Distance(projectile.Center);
            if (distance > 290) distance = 290;
            Vector2 Pos = projectile.Center + Vector2.Normalize(target.Center - projectile.Center) * distance;
            Projectile.NewProjectile(Pos, Vector2.Zero, ModContent.ProjectileType<TwilightBlood>(), 0, 0);
        
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {

            Player owner = Main.player[projectile.owner];

            Texture2D tex = Main.projectileTexture[projectile.type];
            Texture2D BladeTex = mod.GetTexture("Projectiles/TwilightEnd");
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects SP2 = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (owner.direction < 0)
            {
                Vector2 origin = new Vector2(90, 52);
                spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, BladeRot, origin, projectile.scale * 0.4f, SP2, 0);
            }
            else
            {
                Vector2 origin = new Vector2(90, 52);
                spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, BladeRot, origin, projectile.scale * 0.4f, SP2, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, 0, tex.Size() / 2, projectile.scale * 0.55f, SP, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int dir = Math.Sign(projectile.velocity.X + 0.01f);
            if (targetHitbox.Distance(projectile.Center) <= 275 * projectile.scale && Math.Sign(targetHitbox.Center.X - projHitbox.Center.X) == dir)
            {
                return true;
            }
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void CutTiles()
        {
            int dir = Math.Sign(projectile.velocity.X + 0.01f);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center, projectile.Center + new Vector2(275 * dir, 0), 550 * projectile.scale, DelegateMethods.CutTiles);
        }
    }
}