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
    public class TwilightSlash : ModProjectile
    {
        //public static readonly float OffSet = 0;
        public float BladeFrame = 0;
        public float BladeAlpha = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight Slash");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄瞑横斩");
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
            projectile.timeLeft = 60;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 10;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 1;
        }
        public override void AI()
        {
            projectile.penetrate = -1;
            projectile.hostile = false;
            projectile.friendly = true;
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
            projectile.rotation = projectile.velocity.ToRotation();
            owner.itemTime = 10;
            owner.itemAnimation = 10;
            owner.direction = Math.Sign(projectile.velocity.X + 0.01f);
            owner.heldProj = projectile.whoAmI;
            int dir = owner.direction;
            owner.itemRotation = (float)Math.Atan2(projectile.rotation.ToRotationVector2().Y * dir, projectile.rotation.ToRotationVector2().X * dir);
            projectile.Center = owner.Center + new Vector2(0, -5);
            if (owner.mount.Active)
            {
                projectile.Center = owner.MountedCenter;
            }
            if (projectile.ai[0] == 0)
            {
                //projectile.extraUpdates = 1;
                projectile.ai[1]++;
                BladeFrame = projectile.ai[1] - 1;
                if (BladeFrame == 0) BladeFrame = 10;
                BladeAlpha = projectile.ai[1] / 6f;
                projectile.Opacity = projectile.ai[1] / 6f;
                if (projectile.ai[1] >= 6)
                {
                    projectile.ai[0] = 1;
                    projectile.ai[1] = 0;
                }
            }
            else if (projectile.ai[0] == 1)
            {
                //projectile.extraUpdates = 0;
                BladeAlpha = 1;
                projectile.ai[1]++;
                BladeFrame = 6;

                projectile.Opacity = 1 - projectile.ai[1] / 30;
                if (projectile.ai[1] >= 30)
                {
                    projectile.alpha = 255;
                    projectile.Kill();
                    return;
                }
            }
            //owner.itemRotation = (float)Math.Atan2(BladeRot.ToRotationVector2().Y * owner.direction, BladeRot.ToRotationVector2().X * owner.direction);

        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player owner = Main.player[projectile.owner];

            Texture2D tex = Main.projectileTexture[projectile.type];
            Texture2D BladeTex = mod.GetTexture("Projectiles/TwilightSwing" + BladeFrame.ToString());
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (owner.direction >= 0)
            {
                spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, projectile.rotation, BladeTex.Size() / 2, projectile.scale * 0.5f, SP, 0);
            }
            else
            {
                spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, projectile.rotation + MathHelper.Pi, BladeTex.Size() / 2, projectile.scale * 0.5f, SP, 0);
            }
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            if (owner.direction >= 0)//1026 593
            {
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale * 0.55f, SP, 0);
            }
            else
            {
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi, tex.Size() / 2, projectile.scale * 0.55f, SP, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - projectile.rotation.ToRotationVector2() * 290, projectile.Center + projectile.rotation.ToRotationVector2() * 290, 326, ref point);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightBleedingBuff>(), 300, true);
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center - projectile.rotation.ToRotationVector2() * 290, projectile.Center + projectile.rotation.ToRotationVector2() * 290, 326 * projectile.scale, DelegateMethods.CutTiles);
        }
    }
}