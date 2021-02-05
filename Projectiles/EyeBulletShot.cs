using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Buffs;

namespace Twilight.Projectiles
{
    public class EyeBulletShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twilight");
            DisplayName.AddTranslation(GameCulture.Chinese, "薄暝");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 50;
        }
        public override void SetDefaults()          //280 400  scale = 0.4
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.scale = 1f;
            projectile.ranged = true;
            projectile.friendly = true;
            projectile.timeLeft = 600;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 100;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 3;
            projectile.extraUpdates = 1;
        }
        public override void AI()
        {

            projectile.ai[0]++;
            int t = HomeOnTarget(2000);
            if (t != -1)
            {
                NPC target = Main.npc[t];
                Vector2 MoveVel = Vector2.Normalize(target.Center - projectile.Center) * 10;
                projectile.velocity = (MoveVel * 6 + projectile.velocity * 195) / 200;
            }
            else
            {
                Vector2 MoveVel = Vector2.Normalize(Main.MouseWorld - projectile.Center) * 10;
                projectile.velocity = (MoveVel * 6 + projectile.velocity * 195) / 200;
            }
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi / 2;

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D TrailTex = mod.GetTexture("Projectiles/EyeBulletTrail");
            Texture2D tex2 = mod.GetTexture("Projectiles/EyeBullet2");
            Texture2D tex3 = mod.GetTexture("Projectiles/EyeBullet3");
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);

            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[projectile.type] - 1; i++)
            {
                Rectangle rectangle = new Rectangle(0, 5 * i, 84, 5 * (i + 1));  //10 84
                float len = (projectile.oldPos[i + 1] - projectile.oldPos[i]).Length();
                if (projectile.oldPos[i + 1] == Vector2.Zero || projectile.oldPos[i] == Vector2.Zero) continue;
                Vector2 scale = new Vector2(0.2f, len / 10);
                float ops = (50f - i) / 50f;
                Vector2 MidCenter = (projectile.oldPos[i] + projectile.oldPos[i + 1]) / 2 + projectile.Size / 2;
                spriteBatch.Draw(TrailTex, MidCenter - Main.screenPosition, rectangle, Color.Orange * ops, projectile.oldRot[i], rectangle.Size() / 2, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(tex2, projectile.Center - Main.screenPosition, null, Color.White, projectile.rotation, tex2.Size() / 2, projectile.scale * 0.3f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
            spriteBatch.Draw(tex3, projectile.Center - Main.screenPosition, null, Color.White * 0.75f, 0, tex3.Size() / 2, projectile.scale * 0.45f, SpriteEffects.None, 0);

            if (projectile.ai[0] < 30)
            {
                if (Twilight.FindBody() != -1)
                {
                    Vector2 Pos = new Vector2(projectile.localAI[0], projectile.localAI[1]);
                    Vector2 ScreenPos = Pos + Main.npc[Twilight.FindBody()].Center - Main.screenPosition;
                    float R = projectile.ai[0] * 6f;
                    float ops = 1;
                    if (projectile.ai[0] >= 15)
                    {
                        ops = (30 - projectile.ai[0]) / 15;
                    }
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.EffectMatrix);
                    DrawData data = new DrawData(TextureManager.Load("Images/Misc/Perlin"), ScreenPos, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, 0, (int)(R * 2), (int)(R * 2))), Color.Orange * ops, projectile.rotation, new Vector2(R, R), new Vector2(1, 0.67f), SpriteEffects.None, 0);
                    GameShaders.Misc["ForceField"].UseColor(new Vector3(2f));
                    GameShaders.Misc["ForceField"].Apply(new DrawData?(data));
                    data.Draw(spriteBatch);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
                }
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            return false;
        }

        public override bool CanDamage()
        {
            if (projectile.ai[0] < 30)
            {
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            Main.player[projectile.owner].statLife = Utils.Clamp(Main.player[projectile.owner].statLife + 1, 0, Main.player[projectile.owner].statLifeMax2);
            Main.player[projectile.owner].HealEffect(1);
            Twilight.DeepAddBuff(target, ModContent.BuffType<TwilightParadisedBuff>(), 480);
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/EyeBulletBig"), Main.player[projectile.owner].Center);
            Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<EyeBulletExplosion>(), projectile.damage, 0, projectile.owner);
        }

        public int HomeOnTarget(int Range)
        {
            float homingMaximumRangeInPixels = Range;
            int selectedTarget = -1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC n = Main.npc[i];
                if (n.CanBeChasedBy(projectile))
                {
                    float distance = projectile.Distance(n.Center);
                    if (distance <= homingMaximumRangeInPixels &&
                        (
                            selectedTarget == -1 || //there is no selected target
                            projectile.Distance(Main.npc[selectedTarget].Center) > distance) //or we are closer to this target than the already selected target
                    )
                        selectedTarget = i;
                }
            }

            return selectedTarget;
        }


    }
}