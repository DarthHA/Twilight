using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Twilight.Projectiles
{
    public class PeckSplit : ModProjectile
    {
        float Timer = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.scale = 1f + Main.rand.NextFloat() * 0.5f;
            }
            else
            {
                Projectile.scale -= 0.01f;
                if (Projectile.velocity.Length() < 3)
                {
                    Projectile.scale -= 0.05f;
                }
                if (Projectile.scale <= 0)
                {
                    Projectile.Kill();
                }
            }
            Projectile.velocity *= 0.97f;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texExtra = ModContent.Request<Texture2D>("Twilight/Images/BlobGlow").Value;
            Texture2D texExtra2 = ModContent.Request<Texture2D>("Twilight/Images/Extra_193").Value;
            List<CustomVertexInfo> vertexInfos = new();
            Vector2 UnitY = (Projectile.rotation + MathHelper.Pi / 2).ToRotationVector2();
            vertexInfos.Add(new CustomVertexInfo(Projectile.Center + Projectile.rotation.ToRotationVector2() * 15 * Projectile.scale + UnitY * 9 * Projectile.scale, Color.White, new Vector3(0, 0f, 1)));
            vertexInfos.Add(new CustomVertexInfo(Projectile.Center + Projectile.rotation.ToRotationVector2() * 15 * Projectile.scale - UnitY * 9 * Projectile.scale, Color.White, new Vector3(0, 1f, 1)));
            Vector2 CenterOffset = new Vector2(Projectile.width, Projectile.height) / 2f;
            float realLen = -1;
            for (int i = Projectile.oldPos.Length - 1; i >= 0; i--)
            {
                if (Projectile.oldPos[i] == Vector2.Zero)
                {
                    continue;
                }
                else
                {
                    if (realLen == -1)
                    {
                        realLen = i + 1;
                        break;
                    }
                }
            }
            for (int i = 0; i < realLen; i++)
            {
                float progress = 0.25f + i / (realLen - 1f) * 0.75f;
                UnitY = (Projectile.oldRot[i] + MathHelper.Pi / 2).ToRotationVector2();
                vertexInfos.Add(new CustomVertexInfo(Projectile.oldPos[i] + CenterOffset + UnitY * 9 * Projectile.scale, Color.White, new Vector3(progress, 0f, 1)));
                vertexInfos.Add(new CustomVertexInfo(Projectile.oldPos[i] + CenterOffset - UnitY * 9 * Projectile.scale, Color.White, new Vector3(progress, 1f, 1)));
            }
            Timer = (Timer + 1) % 60;
            DrawUtils.DrawShotTrail(texExtra, texExtra2, vertexInfos, Main.spriteBatch, Color.Red, BlendState.Additive, 1 - Timer / 60f);

            List<CustomVertexInfo> vertexInfos2 = new();
            UnitY = (Projectile.rotation + MathHelper.Pi / 2).ToRotationVector2();
            vertexInfos2.Add(new CustomVertexInfo(Projectile.Center + Projectile.rotation.ToRotationVector2() * 10 + UnitY * 1 * Projectile.scale, Color.White, new Vector3(0, 0f, 1)));
            vertexInfos2.Add(new CustomVertexInfo(Projectile.Center + Projectile.rotation.ToRotationVector2() * 10 - UnitY * 1 * Projectile.scale, Color.White, new Vector3(0, 1f, 1)));
            for (int i = 0; i < realLen; i++)
            {
                float progress = 0.25f + i / (realLen - 1f) * 0.75f;
                UnitY = (Projectile.oldRot[i] + MathHelper.Pi / 2).ToRotationVector2();
                vertexInfos2.Add(new CustomVertexInfo(Projectile.oldPos[i] + CenterOffset + UnitY * 1 * Projectile.scale, Color.White, new Vector3(progress, 0f, 1)));
                vertexInfos2.Add(new CustomVertexInfo(Projectile.oldPos[i] + CenterOffset - UnitY * 1 * Projectile.scale, Color.White, new Vector3(progress, 1f, 1)));
            }
            DrawUtils.DrawTrail(texExtra, vertexInfos2, Main.spriteBatch, Color.DarkRed, BlendState.Additive);
            return false;
        }

    }
}
