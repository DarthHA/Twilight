using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Twilight.Items;
namespace Twilight.Projectiles
{
    public class TwilightShoot : ModProjectile
    {
        public static Vector2[] ShootPoses =
        {
            new Vector2(220,0),
            new Vector2(180,10),
            new Vector2(150,-10),
            new Vector2(120,10),
            new Vector2(90,-10),
        };
        public static float[] ShootRs =
        {
            0f,
            MathHelper.Pi / 7,
            -MathHelper.Pi / 7,
            MathHelper.Pi / 5,
            -MathHelper.Pi / 5,
        };
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
            if (Projectile.ai[1] == 20)  //300
            {
                SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/EyeBulletFire") { Volume = 0.5f }, owner.Center);
                for (int i = 0; i < 5; i++)
                {
                    Vector2 ShootPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * ShootPoses[i].X;
                    ShootPos += (Projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * ShootPoses[i].Y;
                    Vector2 Vel = (Projectile.rotation + ShootRs[i]).ToRotationVector2() * 15;
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), ShootPos, Vel, ModContent.ProjectileType<TwilightBulletShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }
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
            if (Projectile.ai[1] >= 20 && Projectile.ai[1] < 40)
            {
                float timer = Projectile.ai[1] - 20;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 StartPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * ShootPoses[i].X;
                    StartPos += (Projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * ShootPoses[i].Y;
                    Vector2 ScreenPos = StartPos - Main.screenPosition;
                    float R = timer * 3f;
                    float ops = 1;
                    if (timer >= 10)
                    {
                        ops = (20 - timer) / 10;
                    }
                    EasyDraw.AnotherDraw(SpriteSortMode.Immediate);
                    DrawData data = new DrawData(ModContent.Request<Texture2D>("Twilight/Images/Extra_193").Value, ScreenPos, new Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops, Projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(Main.spriteBatch);
                    EasyDraw.AnotherDraw(SpriteSortMode.Deferred);
                }
            }
            return false;
        }


        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }
}