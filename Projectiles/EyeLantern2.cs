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

    public class EyeLantern2 : ModProjectile
    {
        public static int FakePlayer = 100;
        public float[] CircleAlpha = new float[6];
        public float[] CircleR = { 0, -100, -200, -300, -400, -500 };
        Vector2 MouseRelaPos = Vector2.Zero;

        public override string Texture => "Twilight/Projectiles/EyeLantern";
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
            projectile.timeLeft = 480;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.damage = 1;
            projectile.penetrate = -1;

        }
        public override void AI()
        {
            if (FakeModPlayer.AnyLanterns2() != projectile.whoAmI)
            {
                projectile.active = false;
                return;
            }
            if (Twilight.FindHead() == -1 || ApoBirdSky.CurrentState != 2)
            {
                projectile.Kill();
                return;
            }
            else
            {
                projectile.Center = Main.LocalPlayer.Center + MouseRelaPos;
                if (projectile.Distance(Main.MouseWorld) < 7)
                {
                    projectile.Center = Main.MouseWorld;
                }
                else
                {
                    projectile.Center += Vector2.Normalize(Main.MouseWorld - projectile.Center) * 7;
                }
                MouseRelaPos = projectile.Center - Main.LocalPlayer.Center;
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
            if (!FakeModPlayer.Initialised)
            {
                FakeModPlayer.Initialised = true;
                Main.player[FakePlayer] = new Player(true)
                {
                    name = Language.ActiveCulture == GameCulture.Chinese ? "永燃灯" : "A Lamp that Burns Forever",
                    difficulty = 2,
                    statLifeMax2 = 99999,
                    statLifeMax = 99999,
                    statLife = 99999,
                    Center = projectile.Center,
                    active = true,
                    immuneAlpha = 255,
                    immune = true,
                    immuneTime = 60,
                };
                Main.player[FakePlayer].PlayerFrame();
            }
            else
            {
                if (!Main.player[FakePlayer].active)
                {
                    Main.player[FakePlayer].active = true;
                    Main.player[FakePlayer].statLife = 99999;
                    Main.player[FakePlayer].statLifeMax = 99999;
                    Main.player[FakePlayer].statLifeMax2 = 99999;
                    Main.player[FakePlayer].immuneAlpha = 255;
                    Main.player[FakePlayer].immune = true;
                    Main.player[FakePlayer].immuneTime = 60;
                }
            }

            if (projectile.timeLeft > 440)
            {
                projectile.Opacity = (float)(480 - projectile.timeLeft) / 40;
            }
            if (projectile.timeLeft < 40)
            {
                projectile.Opacity = (float)projectile.timeLeft / 40;
            }
        }
        public override void Kill(int timeLeft)
        {
            Main.player[FakePlayer].active = false;
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
                if (projectile.active && projectile.type == ModContent.ProjectileType<EyeLantern2>())
                {
                    (projectile.modProjectile as EyeLantern2).DrawLantern();
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
    }

    public class FakeModPlayer : ModPlayer
    {
        public static bool Initialised = false;
        public override void PostUpdateMiscEffects()
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                player.gravity = 0;
                player.velocity = Vector2.Zero;
                if (AnyLanterns2() != -1)
                {
                    player.Center = Main.projectile[AnyLanterns2()].Center;
                }
            }
        }
        public override bool CanBeHitByNPC(NPC npc, ref int cooldownSlot)
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                return false;
            }
            return true;
        }
        public override bool CanBeHitByProjectile(Projectile proj)
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                return false;
            }
            return true;
        }
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                return false;
            }
            return true;
        }
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                return false;
            }
            return true;
        }
        public override void PreUpdateBuffs()
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                for (int i = 0; i < player.buffImmune.Length; i++)
                {
                    player.buffImmune[i] = true;
                }

                for (int i = 0; i < player.buffTime.Length; i++)
                {
                    if (player.buffTime[i] != 0)
                    {
                        player.buffTime[i] = 0;
                    }
                }
            }
        }
        public override void ModifyDrawHeadLayers(List<PlayerHeadLayer> layers)
        {
            if (player.whoAmI == EyeLantern.FakePlayer) //dont draw player
            {
                while (layers.Count > 0)
                    layers.RemoveAt(0);
            }
        }
        public override void ModifyDrawLayers(List<PlayerLayer> layers)
        {
            if (player.whoAmI == EyeLantern.FakePlayer) //dont draw player
            {
                while (layers.Count > 0)
                    layers.RemoveAt(0);
            }
        }
        public override void PostUpdate()
        {
            if (player.whoAmI == EyeLantern.FakePlayer)
            {
                if (AnyLanterns2() == -1)
                {
                    player.active = false;
                }
            }
        }
        public static int AnyLanterns()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<EyeLantern>())
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }

        public static int AnyLanterns2()
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<EyeLantern2>())
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }
    }


}