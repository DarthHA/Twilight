using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;
using Twilight.Buffs;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class EyeLantern : ModProjectile
    {
        public float[] CircleAlpha = new float[6];
        public float[] CircleR = { 0, -100, -200, -300, -400, -500 };

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.scale = 1f;
            Projectile.timeLeft = 999999;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 1;
            Projectile.penetrate = -1;

        }
        public override void AI()
        {
            if (BirdUtils.FindHead() == -1 || ApoBirdSky.CurrentState != ApoBirdSky.State.ApoSky)
            {
                Projectile.Kill();
                return;
            }
            else
            {
                Projectile.Center = Main.npc[BirdUtils.FindHead()].Center + new Vector2(0, 60);
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


            if (Projectile.ai[0] == 1)
            {
                if (Projectile.timeLeft > 40)
                {
                    Projectile.timeLeft = 40;
                }
                Projectile.Opacity = (float)Projectile.timeLeft / 40;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (!Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()))
            {
                DrawLantern();
            }
            return false;
        }

        public static void DrawLanterns()
        {
            foreach (Projectile Projectile in Main.projectile)
            {
                if (Projectile.active && Projectile.type == ModContent.ProjectileType<EyeLantern>())
                {
                    (Projectile.ModProjectile as EyeLantern).DrawLantern();
                }
            }
        }

        private void DrawLantern()
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/Projectiles/EyeLantern1").Value;
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f * Projectile.Opacity, 0, tex.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            //EasyDraw.AnotherDraw(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
            Vector2 ScreenPos = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 6; i++)
            {
                if (CircleR[i] > 0)
                {
                    float R = CircleR[i];
                    float ops = CircleAlpha[i] * 0.4f;

                    DrawData data = new(ModContent.Request<Texture2D>("Twilight/Images/Extra_193").Value, ScreenPos, new Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops * Projectile.Opacity, Projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(Main.spriteBatch);
                }
            }
            EasyDraw.AnotherDraw(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            for (int i = 0; i < 6; i++)
            {
                if (CircleR[i] > 0)
                {
                    float scale = CircleR[i] / 500;
                    float ops = CircleAlpha[i];
                    Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * ops, 0, tex2.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public static void CloseLantern()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<EyeLantern>())
                {
                    proj.ai[0] = 1;
                }
            }
        }
    }




}