using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Projectiles
{
    public class JustitiaEffect : ModProjectile
    {
        public static int ArmLength = 135;
        public float ArmRot = 0;
        public override string Texture => "Twilight/Projectiles/Justice1";
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
            Projectile.alpha = 255;
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
                Projectile.Center = Main.npc[BirdUtils.FindHead()].Center + new Vector2(0, -100);
            }
            Projectile.ai[1]++;
            if (Projectile.ai[1] < 30)   //0-30显形，30-60静止，60-70旋转，70-120惯性，120-160静止，160-190消失
            {
                Projectile.Opacity = Projectile.ai[1] / 30;
            }
            else if (Projectile.ai[1] < 60)
            {
                Projectile.Opacity = 1;
            }
            else if (Projectile.ai[1] < 70)
            {
                Projectile.rotation += MathHelper.Pi / 50;
                ArmRot = MathHelper.Pi / 6 * (Projectile.ai[1] - 60) / 10;
            }
            else if (Projectile.ai[1] < 120)
            {
                float k = (120f - Projectile.ai[1]) / 50 * MathHelper.Pi / 6;
                float a = (Projectile.ai[1] - 70f) / 10f * MathHelper.Pi;
                ArmRot = k * (float)Math.Cos(a);
            }
            else if (Projectile.ai[1] < 160)
            {
                ArmRot = 0;
            }
            else
            {
                Projectile.Opacity = (190f - Projectile.ai[1]) / 30;
                if (Projectile.ai[1] >= 190)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (Projectile.ai[1] == 70)
            {
                int dmg = (int)(TwilightData.JusticeDamage * TwilightPlayer.GetDamageBonus(Main.player[Projectile.owner]));
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Main.screenPosition, Vector2.Zero, ModContent.ProjectileType<JusticeDamage>(), dmg, 0, Projectile.owner);
                SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/JusticeOn") { Volume = 0.5f }, Main.player[Projectile.owner].Center);
            }
            if (Projectile.ai[1] == 60)
            {
                Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect2>(), 0, 0, Projectile.owner);
            }
            //Projectile.rotation += 0.05f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex1 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice1").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice2").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Twilight/Projectiles/Justice3").Value;
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(tex1, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex1.Size() / 2, Projectile.scale * 3f, SpriteEffects.None, 0);
            Vector2 DrawPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * ArmLength * Projectile.scale - Main.screenPosition;
            Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White * Projectile.Opacity, ArmRot, new Vector2(tex2.Width / 2, 0), Projectile.scale * 3f, SpriteEffects.None, 0);

            DrawPos = Projectile.Center - Projectile.rotation.ToRotationVector2() * ArmLength * Projectile.scale - Main.screenPosition;
            Main.spriteBatch.Draw(tex2, DrawPos, null, Color.White * Projectile.Opacity, -ArmRot, new Vector2(tex2.Width / 2, 0), Projectile.scale * 3f, SpriteEffects.None, 0);

            Main.spriteBatch.Draw(tex3, Projectile.Center + new Vector2(0, -10) - Main.screenPosition, null, Color.White * Projectile.Opacity, 0, tex3.Size() / 2, Projectile.scale / 2, SpriteEffects.None, 0);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

    }

}