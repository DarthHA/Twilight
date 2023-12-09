using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
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

        }

        public override void SetDefaults()  //512  512   16  0.75
        {
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 10;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
            Projectile.rotation = Projectile.velocity.ToRotation();
            owner.itemTime = 10;
            owner.itemAnimation = 10;
            owner.direction = Math.Sign(Projectile.velocity.X + 0.01f);
            owner.heldProj = Projectile.whoAmI;
            int dir = owner.direction;
            owner.itemRotation = (float)Math.Atan2(Projectile.rotation.ToRotationVector2().Y * dir, Projectile.rotation.ToRotationVector2().X * dir);
            Projectile.Center = owner.Center;
            if (owner.mount.Active)
            {
                Projectile.Center = owner.MountedCenter;
            }
            if (Projectile.ai[0] == 0)
            {
                //Projectile.extraUpdates = 1;
                Projectile.ai[1]++;
                BladeFrame = Projectile.ai[1];
                BladeAlpha = Projectile.ai[1] / 7f;
                Projectile.Opacity = Projectile.ai[1] / 7f;

                if (Projectile.ai[1] >= 7)
                {
                    SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/TwilightAtk3") { Volume = 0.5f }, owner.Center);
                    Projectile.ai[0] = 1;
                    Projectile.ai[1] = 0;
                }
            }
            else if (Projectile.ai[0] == 1)
            {
                //Projectile.extraUpdates = 0;
                BladeAlpha = 1;
                Projectile.ai[1]++;
                BladeFrame = 8;

                Projectile.Opacity = 1 - Projectile.ai[1] / 30;
                if (Projectile.ai[1] >= 30)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                    return;
                }
            }
            //owner.itemRotation = (float)Math.Atan2(BladeRot.ToRotationVector2().Y * owner.direction, BladeRot.ToRotationVector2().X * owner.direction);

        }


        public override bool PreDraw(ref Color lightColor)
        {

            Player owner = Main.player[Projectile.owner];

            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D BladeTex = ModContent.Request<Texture2D>("Twilight/Projectiles/TwilightSwing" + BladeFrame.ToString()).Value;
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (owner.direction >= 0)
            {
                Main.spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, Projectile.rotation, BladeTex.Size() / 2, Projectile.scale * 0.5f, SP, 0);
            }
            else
            {
                Main.spriteBatch.Draw(BladeTex, owner.Center - Main.screenPosition, null, Color.White * BladeAlpha, Projectile.rotation + MathHelper.Pi, BladeTex.Size() / 2, Projectile.scale * 0.5f, SP, 0);
            }
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            if (owner.direction >= 0)//725 214
            {
                Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex.Size() / 2, Projectile.scale * 0.4f, SP, 0);
            }
            else
            {
                Main.spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation + MathHelper.Pi, tex.Size() / 2, Projectile.scale * 0.4f, SP, 0);
            }
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float point = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Projectile.rotation.ToRotationVector2() * 363, Projectile.Center + Projectile.rotation.ToRotationVector2() * 363, 214, ref point);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float percent = TwilightData.SwingPaleDamage * TwilightPlayer.GetDamageBonus(Main.player[Projectile.owner]);
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
                Main.player[Projectile.owner].addDPS(dmg);
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
                    modifiers.FinalDamage /= (int)Math.Pow(count, 0.666);
                    if (dmg < owner.life)
                    {
                        owner.life -= dmg;
                    }
                    else
                    {
                        owner.life = 0;
                        owner.checkDead();
                    }
                    Main.player[Projectile.owner].addDPS(dmg);
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
            Utils.PlotTileLine(Projectile.Center - Projectile.rotation.ToRotationVector2() * 363, Projectile.Center + Projectile.rotation.ToRotationVector2() * 363, 214 * Projectile.scale, DelegateMethods.CutTiles);
        }

        internal int SegmentCounts(NPC npc)
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