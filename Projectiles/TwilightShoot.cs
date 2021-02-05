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
            DisplayName.SetDefault("Twilight Shoot");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄瞑射线（大雾");
        }

        public override void SetDefaults()
        {
            projectile.melee = true;
            projectile.ranged = true;
            projectile.magic = true;
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.friendly = false;
            projectile.hostile = false;
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
            projectile.rotation = projectile.velocity.ToRotation();
            owner.itemTime = 10;
            owner.itemAnimation = 10;
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
            if (projectile.ai[1] == 20)  //300
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EyeBulletFire"), owner.Center);
                for (int i = 0; i < 5; i++)
                {
                    Vector2 ShootPos = projectile.Center + projectile.rotation.ToRotationVector2() * ShootPoses[i].X;
                    ShootPos += (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * ShootPoses[i].Y;
                    Vector2 Vel = (projectile.rotation + ShootRs[i]).ToRotationVector2() * 15;
                    Projectile.NewProjectile(ShootPos, Vel, ModContent.ProjectileType<TwilightBulletShot>(), projectile.damage, projectile.knockBack, projectile.owner);
                }
            }
            if (projectile.ai[1] >= 40)
            {
                projectile.Kill();
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Player owner = Main.player[projectile.owner];
            Texture2D tex = Main.projectileTexture[projectile.type];
            SpriteEffects SP = owner.direction < 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Vector2 origin = new Vector2(90, 52);
            spriteBatch.Draw(tex, owner.Center - Main.screenPosition, null, Color.White*projectile.Opacity, projectile.rotation, origin, projectile.scale * 0.4f, SP, 0);
            if (projectile.ai[1] >= 20 && projectile.ai[1] < 40)
            {
                float timer = projectile.ai[1] - 20;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 StartPos = projectile.Center + projectile.rotation.ToRotationVector2() * ShootPoses[i].X;
                    StartPos += (projectile.rotation + MathHelper.Pi / 2).ToRotationVector2() * ShootPoses[i].Y;
                    Vector2 ScreenPos = StartPos - Main.screenPosition;
                    float R = timer * 3f;
                    float ops = 1;
                    if (timer >= 10)
                    {
                        ops = (20 - timer) / 10;
                    }
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    DrawData data = new DrawData(TextureManager.Load("Images/Misc/Perlin"), ScreenPos, new Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops, projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(spriteBatch);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
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