using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;

namespace Twilight.Projectiles
{
    public class MeleeHitEffect : ModProjectile
    {
        public float[] StrayPos = new float[60];
        public float[] StrayScale = new float[60];
        public float Effect1Ops = 0;
        public float Effect2Ops = 0;
        public float Effect3Progress = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Apocalypse");
            DisplayName.AddTranslation(GameCulture.Chinese, "终末");
        }
        public override void SetDefaults()          //512 164  scale = 1
        {
            projectile.width = 1400;                     //560 160
            projectile.height = 1000;
            projectile.scale = 1.2f;
            projectile.melee = true;
            projectile.friendly = true;
            projectile.timeLeft = 6000;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 100;
            projectile.penetrate = -1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
            projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 99999;   //40
            if (projectile.ai[1] == 0)
            {
                GenerateEffect1();
            }
            projectile.ai[1]++;
            if (projectile.ai[1] < 10)
            {
                Effect1Ops = projectile.ai[1] / 10;
                if (projectile.ai[1] >= 5)
                {
                    Effect2Ops = (projectile.ai[1] - 5) / 5;
                }
            }
            else if (projectile.ai[1] < 30)
            {
                if (projectile.ai[1] <= 15)
                {
                    Effect1Ops = (15 - projectile.ai[1]) / 5;
                }
                else
                {
                    Effect1Ops = 0;
                }
                Effect2Ops = (30 - projectile.ai[1]) / 20;
            }
            else
            {
                Effect1Ops = 0;
                Effect2Ops = 0;
            }
            if (projectile.ai[1] >= 10)
            {
                Effect3Progress = (projectile.ai[1] - 10) / 30;
            }


            if (projectile.ai[1] > 40)
            {
                projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                projectile.Kill();
                return;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 2, 0, Main.player[projectile.owner].statLifeMax2);
            Main.player[projectile.owner].HealEffect(2);
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightStunnedBuff>(), 80);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            if (projectile.ai[1] > 0)
            {
                Texture2D tex1 = mod.GetTexture("Projectiles/MeleeEffect1");
                Texture2D tex2 = mod.GetTexture("Projectiles/MeleeEffect2");
                Texture2D tex3 = mod.GetTexture("Projectiles/MeleeEffect3");
                spriteBatch.Draw(tex1, projectile.Center - Main.screenPosition, null, Color.Red * Effect1Ops, 0, tex1.Size() / 2, new Vector2(40, 10) * projectile.scale, SpriteEffects.None, 0);
                for (int i = 0; i < 60; i++)
                {
                    //400x1000,20x100
                    Vector2 scale = new Vector2(StrayScale[i] * 3, 10) * projectile.scale;
                    Vector2 Pos = projectile.Center + new Vector2((StrayPos[i] - 0.5f) * 600, -50 * (1 - (float)Math.Sin(StrayPos[i] * MathHelper.Pi))) * projectile.scale - Main.screenPosition;
                    spriteBatch.Draw(tex1, Pos, null, Color.IndianRed * Effect1Ops, 0, tex1.Size() / 2, scale, SpriteEffects.None, 0);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                spriteBatch.Draw(tex2, projectile.Center + new Vector2(0, 250) - Main.screenPosition, null, Color.White * Effect2Ops, 0, tex2.Size() / 2, projectile.scale * 6, SpriteEffects.None, 0);
                if (Effect3Progress > 0)
                {
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    float ops = 1;
                    if (projectile.ai[1] >= 30)
                    {
                        ops = (40f - projectile.ai[1]) / 10f;
                    }
                    spriteBatch.Draw(tex3, projectile.Center + new Vector2(0, 500) - Main.screenPosition, null, Color.White * ops, 0, tex3.Size() / 2, projectile.scale * Effect3Progress * 1.6f, SpriteEffects.None, 0);
                    spriteBatch.Draw(tex3, projectile.Center + new Vector2(0, 450) - Main.screenPosition, null, Color.White * ops, 0, tex3.Size() / 2, projectile.scale * Effect3Progress * 1.2f, SpriteEffects.None, 0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }


        public void GenerateEffect1()
        {
            for (int i = 0; i < 60; i++)
            {
                StrayPos[i] = Main.rand.NextFloat();
                StrayScale[i] = Main.rand.NextFloat() * 0.9f + 0.1f;
            }
        }

    }
}