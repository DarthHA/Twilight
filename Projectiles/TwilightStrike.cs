using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Enums;
using Terraria.ModLoader;
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

        }

        public override void SetDefaults()  //512  512   16  0.75
        {
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 10;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 100;
        }
        public override void AI()
        {
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
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
            //Projectile.rotation = Projectile.velocity.ToRotation();
            owner.itemTime = 15;
            owner.itemAnimation = 15;
            owner.direction = Math.Sign(Projectile.velocity.X + 0.01f);
            owner.heldProj = Projectile.whoAmI;
            //int dir = owner.direction;
            //owner.itemRotation = (float)Math.Atan2(Projectile.rotation.ToRotationVector2().Y * dir, Projectile.rotation.ToRotationVector2().X * dir);
            Projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                Projectile.Center = owner.MountedCenter;
            }
            if (Projectile.ai[0] == 0)
            {
                Projectile.extraUpdates = 1;
                Projectile.ai[1]++;
                if (owner.direction > 0)
                {
                    BladeRot = -MathHelper.Pi / 4 * 3 + MathHelper.Pi * Projectile.ai[1] / 8f;

                }
                else
                {
                    BladeRot = -MathHelper.Pi / 4 - MathHelper.Pi * Projectile.ai[1] / 8f;
                }
                BladeAlpha = Projectile.ai[1] / 16f;
                Projectile.Opacity = Projectile.ai[1] / 8f;
                if (Projectile.ai[1] >= 8)
                {
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                Projectile.extraUpdates = 0;
                BladeAlpha = 1;
                Projectile.ai[1]++;
                if (owner.direction > 0)
                {
                    BladeRot = MathHelper.Pi / 4;

                }
                else
                {
                    BladeRot = MathHelper.Pi / 4 * 3;
                }

                Projectile.Opacity = 1 - Projectile.ai[1] / 30;
                if (Projectile.ai[1] >= 30)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }
            owner.itemRotation = (float)Math.Atan2(BladeRot.ToRotationVector2().Y * owner.direction, BladeRot.ToRotationVector2().X * owner.direction);

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.Next(4) <= 2)
            {
                modifiers.SetCrit();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightStunnedBuff>(), 65);
            SomeUtils.DeepApplyStun(target, 65);
            float distance = target.Distance(Projectile.Center);
            if (distance > 290) distance = 290;
            Vector2 Pos = Projectile.Center + Vector2.Normalize(target.Center - Projectile.Center) * distance;
            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Pos, Vector2.Zero, ModContent.ProjectileType<TwilightBlood>(), 0, 0);

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Player owner = Main.player[Projectile.owner];

            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D BladeTex = ModContent.Request<Texture2D>("Twilight/Projectiles/TwilightEnd").Value;
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects SP2 = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (owner.direction < 0)
            {
                Vector2 origin = new(90, 52);
                Main.spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, BladeRot, origin, Projectile.scale * 0.4f, SP2, 0);
            }
            else
            {
                Vector2 origin = new(90, 52);
                Main.spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, BladeRot, origin, Projectile.scale * 0.4f, SP2, 0);
            }
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale * 0.55f, SP, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            int dir = Math.Sign(Projectile.velocity.X + 0.01f);
            if (targetHitbox.Distance(Projectile.Center) <= 275 * Projectile.scale && Math.Sign(targetHitbox.Center.X - projHitbox.Center.X) == dir)
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
            int dir = Math.Sign(Projectile.velocity.X + 0.01f);
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + new Vector2(275 * dir, 0), 550 * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}