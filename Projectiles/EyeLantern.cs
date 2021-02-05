using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class EyeLantern : ModProjectile
    {
        public static int FakePlayer = 100;
        public float[] CircleAlpha = new float[6];
        public float[] CircleR = { 0, -100, -200, -300, -400, -500 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eye Lantern");
            DisplayName.AddTranslation(GameCulture.Chinese, "目灯");
        }

        public override void SetDefaults()
        {
            projectile.width = 1;
            projectile.height = 1;
            projectile.scale = 1f;
            projectile.timeLeft = 999999;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 1;
            projectile.penetrate = -1;

        }
        public override void AI()
        {
            if (Twilight.FindHead() == -1 || ApoBirdSky.CurrentState != 2)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = Main.npc[Twilight.FindHead()].Center + new Vector2(0, 60);
            }
            for (int i = 0; i < 6; i++)
            {
                CircleR[i] += 5;
                if (CircleR[i] > 200)
                {
                    CircleAlpha[i] = (400 - CircleR[i]) / 200;
                }
                else
                {
                    CircleAlpha[i] = 1;
                }
                if (CircleR[i] > 400)
                {
                    CircleR[i] -= 400;
                }
            }
            

            if (projectile.ai[0] == 1)
            {
                if (projectile.timeLeft > 40)
                {
                    projectile.timeLeft = 40;
                }
                projectile.Opacity = (float)projectile.timeLeft / 40;
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (!Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()) || !Twilight.config.UseUI)
            {
                DrawLantern();
            }
            return false;
        }

        public static void DrawLanterns()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.active && projectile.type == ModContent.ProjectileType<EyeLantern>())
                {
                    (projectile.modProjectile as EyeLantern).DrawLantern();
                }
            }
        }

        private void DrawLantern()
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            Texture2D tex2 = mod.GetTexture("Projectiles/EyeLantern1");
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Main.spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, Color.White * 0.5f * projectile.Opacity, 0, tex.Size() / 2, projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Vector2 ScreenPos = projectile.Center - Main.screenPosition;
            for (int i = 0; i < 6; i++)
            {
                if (CircleR[i] > 0)
                {
                    float R = CircleR[i];
                    float ops = CircleAlpha[i] * 0.4f;

                    DrawData data = new DrawData(TextureManager.Load("Images/Misc/Perlin"), ScreenPos, new Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops * projectile.Opacity, projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(Main.spriteBatch);
                }
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            for (int i = 0; i < 6; i++)
            {
                if (CircleR[i] > 0)
                {
                    float scale = CircleR[i] / 500;
                    float ops = CircleAlpha[i];
                    Main.spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White * ops, 0, tex2.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public static void CloseLantern()
        {
            if (FakeModPlayer.AnyLanterns() == -1) return;
            Main.projectile[FakeModPlayer.AnyLanterns()].ai[0] = 1;
        }
    }

    


}