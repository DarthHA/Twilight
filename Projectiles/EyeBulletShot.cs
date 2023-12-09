using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Items;

namespace Twilight.Projectiles
{
    public class EyeBulletShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
        }
        public override void SetDefaults()          //280 400  scale = 0.4
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.scale = 1f;
            Projectile.DamageType = ModContent.GetInstance<TwilightDamage>();
            Projectile.friendly = true;
            Projectile.timeLeft = 480;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.damage = 100;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 3;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {

            Projectile.ai[0]++;
            int t = HomeOnTarget(2000);
            if (t != -1)
            {
                NPC target = Main.npc[t];
                Vector2 MoveVel = Vector2.Normalize(target.Center - Projectile.Center) * 10;
                Projectile.velocity = (MoveVel * 6 + Projectile.velocity * 195) / 200;
            }
            else
            {
                Vector2 MoveVel = Vector2.Normalize(Main.MouseWorld - Projectile.Center) * 10;
                Projectile.velocity = (MoveVel * 6 + Projectile.velocity * 195) / 200;
                if (Projectile.Distance(Main.MouseWorld) < 40)
                {
                    Projectile.Kill();
                    return;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi / 2;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D TrailTex = ModContent.Request<Texture2D>("Twilight/Projectiles/EyeBulletTrail").Value;
            Texture2D tex2 = ModContent.Request<Texture2D>("Twilight/Projectiles/EyeBullet2").Value;
            Texture2D tex3 = ModContent.Request<Texture2D>("Twilight/Projectiles/EyeBullet3").Value;
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.Additive);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type] - 1; i++)
            {
                //int i2 = (int)(i + Projectile.ai[0]) % (ProjectileID.Sets.TrailCacheLength[Projectile.type] - 1);
                Rectangle rectangle = new(0, 5 * i, 84, 5 * (i + 1));  //10 84
                float len = (Projectile.oldPos[i + 1] - Projectile.oldPos[i]).Length();
                if (Projectile.oldPos[i + 1] == Vector2.Zero || Projectile.oldPos[i] == Vector2.Zero) continue;
                Vector2 scale = new(0.2f, len / 10);
                float ops = (50f - i) / 50f;
                Vector2 MidCenter = (Projectile.oldPos[i] + Projectile.oldPos[i + 1]) / 2 + Projectile.Size / 2;
                Main.spriteBatch.Draw(TrailTex, MidCenter - Main.screenPosition, rectangle, Color.Orange * ops, Projectile.oldRot[i], rectangle.Size() / 2, scale, SpriteEffects.None, 0);
            }

            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.NonPremultiplied);
            Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, tex2.Size() / 2, Projectile.scale * 0.3f, SpriteEffects.None, 0);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            EasyDraw.AnotherDraw(BlendState.Additive);
            Main.spriteBatch.Draw(tex3, Projectile.Center - Main.screenPosition, null, Color.White * 0.75f, 0, tex3.Size() / 2, Projectile.scale * 0.45f, SpriteEffects.None, 0);

            if (Projectile.ai[0] < 30)
            {
                if (BirdUtils.FindBody() != -1)
                {
                    Vector2 Pos = new(Projectile.localAI[0], Projectile.localAI[1]);
                    Vector2 ScreenPos = Pos + Main.npc[BirdUtils.FindBody()].Center - Main.screenPosition;
                    float R = Projectile.ai[0] * 6f;
                    float ops = 1;
                    if (Projectile.ai[0] >= 15)
                    {
                        ops = (30 - Projectile.ai[0]) / 15;
                    }
                    Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    //EasyDraw.AnotherDraw(SpriteSortMode.Immediate);
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicWrap, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
                    DrawData data = new(ModContent.Request<Texture2D>("Twilight/Images/Extra_193").Value, ScreenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops, Projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(Main.spriteBatch);
                    EasyDraw.AnotherDraw(SpriteSortMode.Deferred);
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                }
            }
            EasyDraw.AnotherDraw(BlendState.AlphaBlend);
            //spriteBatch.End();
            //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool? CanDamage()
        {
            if (Projectile.ai[0] < 30)
            {
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.player[Projectile.owner].statLife = Utils.Clamp(Main.player[Projectile.owner].statLife + 1, 0, Main.player[Projectile.owner].statLifeMax2);
            Main.player[Projectile.owner].HealEffect(1);
            SomeUtils.DeepApplyParadise(target, 480);
            //Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 480);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/EyeBulletBig") { Volume = 0.5f }, Main.player[Projectile.owner].Center);
            Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EyeBulletExplosion>(), Projectile.damage, 0, Projectile.owner);
        }

        public int HomeOnTarget(int Range)
        {
            float homingMaximumRangeInPixels = Range;
            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(Projectile))
                {
                    float distance = Projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            Projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }


    }
}