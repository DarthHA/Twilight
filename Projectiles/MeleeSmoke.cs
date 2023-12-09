using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
namespace Twilight.Projectiles
{
    class MeleeSmoke : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1.0f;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.Opacity = 1;
            Projectile.hide = true;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;
                Projectile.rotation = MathHelper.TwoPi * Main.rand.NextFloat();
            }
            Projectile.rotation += 0.01f;
            Projectile.ai[0]++;
            Projectile.frame = (int)(Projectile.ai[0] * 0.4);
            Projectile.scale = (float)Math.Sqrt(Projectile.ai[0] / 60);
            Projectile.velocity *= 0.93f;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = GetTexRect(Projectile.frame);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rectangle, Color.Black, Projectile.rotation, rectangle.Size() / 2, Projectile.scale * 1.4f, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, rectangle, Color.Red, Projectile.rotation, rectangle.Size() / 2, Projectile.scale * 1.3f, SpriteEffects.None, 0);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }

        public Rectangle GetTexRect(int index)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            index = Math.Clamp(index, 0, 24);
            int dwidth = tex.Width / 5;
            int dheight = tex.Height / 5;
            int x = index % 5;
            int y = index / 5;
            return new Rectangle(x * dwidth, y * dheight, dwidth, dheight);
        }



    }
}