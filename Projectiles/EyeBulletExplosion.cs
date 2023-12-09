using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Twilight.Items;
using Twilight.UI;

namespace Twilight.Projectiles
{
    public class EyeBulletExplosion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
        }
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 200;
            Projectile.scale = 1.2f;
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.friendly = true;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 100;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = false;
        }
        public override void AI()
        {
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 99999;   //40

            Projectile.ai[1]++;
            if (Projectile.ai[1] == 1 && Main.LocalPlayer.Distance(Projectile.Center) < 1100)
            {
                Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(5, 0.5f);
            }

            if (Projectile.ai[1] > 30)
            {
                Projectile.GetGlobalProjectile<TwilightGProj>().CanKill = true;
                Projectile.Kill();
                return;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 1, 0, Main.player[Projectile.owner].statLifeMax2);
            Main.player[Projectile.owner].HealEffect(1);
            SomeUtils.DeepApplyParadise(target, 480);
            //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 480);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.Additive);

            if (Projectile.ai[1] > 0)
            {
                Texture2D tex2 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Texture2D tex3 = ModContent.Request<Texture2D>("Twilight/Projectiles/MeleeEffect3").Value;
                float ops;
                if (Projectile.ai[1] < 10)
                {
                    ops = Projectile.ai[1] / 10;
                }
                else if (Projectile.ai[1] < 25)
                {
                    ops = (25 - Projectile.ai[1]) / 15;
                }
                else
                {
                    ops = 0;
                }
                Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * ops, 0, tex2.Size() / 2, Projectile.scale * 0.9f, SpriteEffects.None, 0);

                if (Projectile.ai[1] > 5)
                {
                    float ops2 = 1;
                    float scale = Projectile.scale * (Projectile.ai[1] - 5) / 15f * 0.4f;
                    if (Projectile.ai[1] >= 20)
                    {
                        ops2 = (30f - Projectile.ai[1]) / 10f;
                    }

                    Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * ops2, 0, tex3.Size() / 2, scale, SpriteEffects.None, 0);
                }
            }

            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }




    }
}