using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Items;
namespace Twilight.Projectiles
{
    public class TwilightSwing : ModProjectile
    {
        //public static readonly float OffSet = 0;
        public float BladeFrame = 0;
        public float BladeAlpha = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight Swing");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄瞑直挥");

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
            projectile.localNPCHitCooldown = 10;
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
            projectile.rotation = projectile.velocity.ToRotation();
            owner.itemTime = 10;
            owner.itemAnimation = 10;
            owner.direction = Math.Sign(projectile.velocity.X + 0.01f);
            owner.heldProj = projectile.whoAmI;
            int dir = owner.direction;
            owner.itemRotation = (float)Math.Atan2(projectile.rotation.ToRotationVector2().Y * dir, projectile.rotation.ToRotationVector2().X * dir);
            projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                projectile.Center = owner.MountedCenter;
            }
            if (projectile.ai[0] == 0)
            {
                //projectile.extraUpdates = 1;
                projectile.ai[1]++;
                BladeFrame = projectile.ai[1];
                BladeAlpha = projectile.ai[1] / 7f;
                projectile.Opacity = projectile.ai[1] / 7f;

                if (projectile.ai[1] >= 7)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/TwilightAtk3"), owner.Center);
                    projectile.ai[0] = 1;
                    projectile.ai[1] = 0;
                }
            }
            else if (projectile.ai[0] == 1)
            {
                //projectile.extraUpdates = 0;
                BladeAlpha = 1;
                projectile.ai[1]++;
                BladeFrame = 8;

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
            if (owner.direction >= 0)//725 214
            {
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation, tex.Size() / 2, projectile.scale * 0.4f, SP, 0);
            }
            else
            {
                spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * projectile.Opacity, projectile.rotation + MathHelper.Pi, tex.Size() / 2, projectile.scale * 0.4f, SP, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), projectile.Center - projectile.rotation.ToRotationVector2() * 363, projectile.Center + projectile.rotation.ToRotationVector2() * 363, 214,ref point);
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            float percent = Twilight.SwingPaleDamage * TwilightPlayer.GetDamageBonus(Main.player[projectile.owner]);
            if (target.realLife == -1)
            {
                int dmg = (int)(target.lifeMax * percent);

                if (dmg < target.life)
                {
                    target.life -= dmg;
                }
                else
                {
                    target.life = 0;
                    target.checkDead();
                }
                CombatText.NewText(target.Hitbox, Color.Cyan, dmg);
            }
            else
            {
                NPC owner = Main.npc[target.realLife];
                if (owner.active)
                {
                    int count = SegmentCounts(owner);
                    if (count == 0) count = 1;
                    int dmg = (int)(target.lifeMax * percent) / (int)Math.Pow(count, 0.666);
                    damage /= (int)Math.Pow(count, 0.666);
                    if (dmg < owner.life)
                    {
                        owner.life -= dmg;
                    }
                    else
                    {
                        owner.life = 0;
                        owner.checkDead();
                    }
                    CombatText.NewText(target.Hitbox, Color.Cyan, dmg);
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(projectile.Center - projectile.rotation.ToRotationVector2() * 363, projectile.Center + projectile.rotation.ToRotationVector2() * 363, 214 * projectile.scale, DelegateMethods.CutTiles);
        }

        public int SegmentCounts(NPC npc)
        {
            int result = 0;
            foreach (NPC segs in Main.npc)
            {
                if (segs.realLife != -1 && segs.realLife == npc.whoAmI)
                {
                    result++;
                }
            }
            return result;
        }
    }
}