using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;

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
        }
        public override void SetDefaults()          //512 164  scale = 1
        {
            Projectile.width = 1400;                     //560 160
            Projectile.height = 1000;
            Projectile.scale = 1.2f;
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.friendly = true;
            Projectile.timeLeft = 6000;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 100;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;   //40
            if (Projectile.ai[1] == 0)
            {
                GenerateEffect1();
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 10)
            {
                Effect1Ops = Projectile.ai[1] / 10;
                if (Projectile.ai[1] >= 5)
                {
                    Effect2Ops = (Projectile.ai[1] - 5) / 5;
                }
                if (Projectile.ai[1] == 5)
                {
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 Vel = new(Main.rand.Next(-55, 55), Main.rand.Next(-35, 20));
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(0, 400), Vel, ModContent.ProjectileType<MeleeSmoke>(), 0, 0, Projectile.owner);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 Vel = (Main.rand.NextFloat() * -MathHelper.Pi).ToRotationVector2() * Main.rand.Next(10, 30);
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center + new Vector2(0, 400) + Vector2.Normalize(Vel) * Main.rand.Next(1, 100) + new Vector2(Main.rand.Next(-55, 55), Main.rand.Next(-35, 35)), Vel, ModContent.ProjectileType<MeleeSharp>(), Projectile.damage / 50, Projectile.knockBack, Projectile.owner);
                    }
                }
            }
            else if (Projectile.ai[1] < 30)
            {
                if (Projectile.ai[1] <= 15)
                {
                    Effect1Ops = (15 - Projectile.ai[1]) / 5;
                }
                else
                {
                    Effect1Ops = 0;
                }
                Effect2Ops = (30 - Projectile.ai[1]) / 20;
            }
            else
            {
                Effect1Ops = 0;
                Effect2Ops = 0;
            }
            if (Projectile.ai[1] >= 10)
            {
                Effect3Progress = (Projectile.ai[1] - 10) / 30;
            }


            if (Projectile.ai[1] > 40)
            {
                Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                Projectile.Kill();
                return;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 2, 0, Main.player[Projectile.owner].statLifeMax2);
            Main.player[Projectile.owner].HealEffect(2);
            //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightStunnedBuff>(), 80);
            SomeUtils.DeepApplyStun(target, 80);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            EasyDraw.AnotherDraw(BlendState.Additive, Main.GameViewMatrix.ZoomMatrix);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            if (Projectile.ai[1] > 0)
            {
                Texture2D tex1 = ModContent.Request<Texture2D>("Twilight/Projectiles/MeleeEffect1").Value;
                Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/Projectiles/MeleeFx").Value;   //2
                Texture2D tex3 = ModContent.Request<Texture2D>("Twilight/Projectiles/MeleeEffect3").Value;
                Main.spriteBatch.Draw(tex1, Projectile.Center - Main.screenPosition, null, Color.Red * Effect1Ops, 0, tex1.Size() / 2, new Vector2(40, 10) * Projectile.scale, SpriteEffects.None, 0);
                for (int i = 0; i < 60; i++)
                {
                    //400x1000,20x100
                    Vector2 scale = new Vector2(StrayScale[i] * 3, 9.75f) * Projectile.scale;
                    Vector2 Pos = Projectile.Center + new Vector2((StrayPos[i] - 0.5f) * 600, -50 * (1 - (float)Math.Sin(StrayPos[i] * MathHelper.Pi))) * Projectile.scale - Main.screenPosition;
                    Main.spriteBatch.Draw(tex1, Pos, null, Color.IndianRed * Effect1Ops, 0, tex1.Size() / 2, scale, SpriteEffects.None, 0);
                }
                //spriteBatch.End();
                //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                EasyDraw.AnotherDraw(BlendState.Additive, Main.GameViewMatrix.ZoomMatrix); //non
                Main.spriteBatch.Draw(tex2, Projectile.Center + new Vector2(0, 225) - Main.screenPosition, null, Color.Orange * Effect2Ops, 0, tex2.Size() / 2, Projectile.scale * 6.5f, SpriteEffects.None, 0);
                if (Effect3Progress > 0)
                {
                    EasyDraw.AnotherDraw(BlendState.Additive);
                    //spriteBatch.End();
                    //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    float ops = 1;
                    if (Projectile.ai[1] >= 30)
                    {
                        ops = (40f - Projectile.ai[1]) / 10f;
                    }
                    Main.spriteBatch.Draw(tex3, Projectile.Center + new Vector2(0, 500) - Main.screenPosition, null, Color.White * ops, 0, tex3.Size() / 2, Projectile.scale * Effect3Progress * 1.6f, SpriteEffects.None, 0);
                    Main.spriteBatch.Draw(tex3, Projectile.Center + new Vector2(0, 450) - Main.screenPosition, null, Color.White * ops, 0, tex3.Size() / 2, Projectile.scale * Effect3Progress * 1.2f, SpriteEffects.None, 0);
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend, Main.GameViewMatrix.ZoomMatrix);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
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