using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SolemnLament.Sky;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Terraria.UI;
using Twilight.Buffs;
using Twilight.NPCs;
using Twilight.Projectiles;
using Twilight.Sky;
using Twilight.UI;

namespace Twilight
{
    public class Twilight : Mod
    {
        public static Twilight Instance;
        public static TwilightConfig config;
        private UserInterface _DrawingUIInterface;
        internal DrawingUI _DrawingUI;
        public Twilight()
        {
            Instance = this;
        }
        public override void Load()
        {
            
            Filters.Scene["Twilight:ApoBirdSky"] = new Filter(new ApoBirdSkyScreenShaderData("FilterMiniTower").UseColor(1.0f, 1.0f, 1.0f).UseOpacity(0.0f), EffectPriority.VeryHigh);
            SkyManager.Instance["Twilight:ApoBirdSky"] = new ApoBirdSky();
            _DrawingUI = new DrawingUI();
            _DrawingUIInterface = new UserInterface();
            _DrawingUIInterface.SetState(_DrawingUI);
            On.Terraria.NPC.TargetClosest += new On.Terraria.NPC.hook_TargetClosest(TargetClosestHook);
            On.Terraria.Player.KillMe += new On.Terraria.Player.hook_KillMe(KillMeHook);
            On.Terraria.Player.KillMeForGood += new On.Terraria.Player.hook_KillMeForGood(KillMeForGoodHook);
            On.Terraria.Player.Ghost += new On.Terraria.Player.hook_Ghost(GhostHook);
            On.Terraria.GameContent.Skies.MoonLordSky.GetIntensity += new On.Terraria.GameContent.Skies.MoonLordSky.hook_GetIntensity(GetIntensityHook);
            On.Terraria.Projectile.Kill += new On.Terraria.Projectile.hook_Kill(ProjKillHook);
        }

        public static float GetIntensityHook(On.Terraria.GameContent.Skies.MoonLordSky.orig_GetIntensity orig,Terraria.GameContent.Skies.MoonLordSky self)
        {
            if (ApoBirdSky.CurrentState == 0)
            {
                return orig.Invoke(self);
            }
            else if (ApoBirdSky.CurrentState == 2)
            {
                return 0;
            }
            else
            {
                float intensity1 = orig.Invoke(self);
                intensity1 *= 1 - ApoBirdSky.GetIntensity();
                return intensity1;
            }
            
        }

        public static void GhostHook(On.Terraria.Player.orig_Ghost orig, Player self)
        {
            if (self.whoAmI == EyeLantern.FakePlayer)
            {
                self.ghost = false;
                self.dead = false;
                return;
            }
            orig.Invoke(self);
        }
        public static void KillMeForGoodHook(On.Terraria.Player.orig_KillMeForGood orig, Player self)
        {
            if (self.whoAmI == EyeLantern.FakePlayer)
            {
                self.ghost = false;
                self.dead = false;
                return;
            }
            orig.Invoke(self);
        }
        public static void KillMeHook(On.Terraria.Player.orig_KillMe orig, Player self, PlayerDeathReason damageSource, double dmg, int hitdirection, bool pvp = false)
        {
            if (self.whoAmI == EyeLantern.FakePlayer)
            {
                self.statLife = self.statLifeMax2;
                if (self.statLifeMax < 99999) self.statLifeMax = 99999;
                if (self.statLifeMax2 < 99999) self.statLifeMax2 = 99999;
                self.dead = false;
                self.ghost = false;
                self.statLife = self.statLifeMax2;
                return;
            }
            orig.Invoke(self, damageSource, dmg, hitdirection, pvp);
        }

        public static void TargetClosestHook(On.Terraria.NPC.orig_TargetClosest orig, NPC self, bool faceTarget)
        {
            if (FakeModPlayer.Initialised)
            {
                if (Main.player[EyeLantern.FakePlayer].active)
                {
                    self.target = EyeLantern.FakePlayer;

                    if (self.target < 0 || self.target >= 255)
                    {
                        self.target = 0;
                    }
                    self.targetRect = new Rectangle((int)Main.player[self.target].position.X, (int)Main.player[self.target].position.Y, Main.player[self.target].width, Main.player[self.target].height);
                    if (Main.player[self.target].dead)
                    {
                        faceTarget = false;
                    }
                    if (Main.player[self.target].npcTypeNoAggro[self.type] && self.direction != 0)
                    {
                        faceTarget = false;
                    }
                    if (faceTarget)
                    {
                        if (Main.player[self.target].itemAnimation != 0 || Main.player[self.target].aggro >= 0 || self.oldTarget < 0 || self.oldTarget > 254)
                        {
                            self.direction = 1;
                            if (self.targetRect.X + self.targetRect.Width / 2 < self.Center.X)
                            {
                                self.direction = -1;
                            }
                            self.directionY = 1;
                            if (self.targetRect.Y + self.targetRect.Height / 2 < self.Center.Y)
                            {
                                self.directionY = -1;
                            }
                        }
                    }

                    if (self.confused)
                    {
                        self.direction *= -1;
                    }

                    return;
                }
            }

            orig.Invoke(self, faceTarget);
        }

        public static void ProjKillHook(On.Terraria.Projectile.orig_Kill orig,Projectile self)
        {
            if (!self.GetGlobalProjectile<TwilightGProj>().CanKill)
            {
                self.penetrate = -1;
                self.timeLeft = 99999;
            }
            else
            {
                orig.Invoke(self);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int DrawingUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 1"));
            if (DrawingUIIndex != -1)
            {
                layers.Insert(DrawingUIIndex, new LegacyGameInterfaceLayer(
                    "Twilight: DrawingUI",
                    delegate
                    {
                        _DrawingUIInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.myPlayer == -1 || Main.gameMenu || !Main.LocalPlayer.active)
            {
                return;
            }
            if (FindBody() != -1)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/Binah");
                priority = MusicPriority.BossHigh;
            }
        }

        public override void Unload()
        {
            SkyManager.Instance["Twilight:ApoBirdSky"].Deactivate();
            config = null;
            Instance = null;
            
        }



        public static void DeepAddBuff(NPC target, int buffType, int buffTime, bool dot = false)
        {
            if (!dot || target.realLife == -1)
            {
                target.buffImmune[buffType] = false;
                target.AddBuff(buffType, buffTime);
            }
            if (target.realLife >= 0)
            {
                if (Main.npc[target.realLife].active)
                {
                    Main.npc[target.realLife].buffImmune[buffType] = false;
                    Main.npc[target.realLife].AddBuff(buffType, buffTime);
                    if (!dot)
                    {
                        foreach (NPC npc in Main.npc)
                        {
                            if (npc.active)
                            {
                                if (npc.active && npc.realLife == target.realLife)
                                {
                                    if (npc.whoAmI != target.whoAmI && npc.whoAmI != target.realLife)
                                    {
                                        npc.buffImmune[buffType] = false;
                                        npc.AddBuff(buffType, buffTime);
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }

        public static int ClawDamage = 2000;
        public static int BulletDamage = 400;
        public static int PeckDamage = 12000;
        public static int JusticeDamage = 6666;
        public static float TiltedScaleDamage = 0.15f;
        public static float PunishmentDamage = 2f;
        public static float SalvationCritRange = 600f;
        public static float SinChance = 0.1f;
        public static float SinPaleDamage = 0.04f;
        public static float StrikeDamage = 15;
        public static float SlashDamage = 0.25f;
        public static float SwingDamage = 1;
        public static float SwingPaleDamage = 0.003f;
        public static float ShootDamage = 0.45f;
        public static int BleedingDamage = 60;
        public static int EyeBulletCD = 100;
        public static int ClawCD = 100;
        public static int EyeSpecialCD = 550;
        public static int JusticeSpecialCD = 240;
        public static int PeckSpecialCD = 240;

        #region 一些方法
        public static int FindHead()
        {
            foreach (NPC head in Main.npc)
            {
                if (head.active)
                {
                    if (head.type == ModContent.NPCType<ApoBirdHead>())
                    {
                        return head.whoAmI;
                    }
                }
            }
            return -1;
        }

        public static int FindArmLeft()
        {
            foreach (NPC armleft in Main.npc)
            {
                if (armleft.active && armleft.ai[0] < 0)
                {
                    if (armleft.type == ModContent.NPCType<ApoBirdClaw>())
                    {
                        return armleft.whoAmI;
                    }
                }
            }
            return -1;
        }


        public static int FindArmRight()
        {
            foreach (NPC armright in Main.npc)
            {
                if (armright.active && armright.ai[0] > 0)
                {
                    if (armright.type == ModContent.NPCType<ApoBirdClaw>())
                    {
                        return armright.whoAmI;
                    }
                }
            }
            return -1;
        }


        public static int FindBody()
        {
            foreach (NPC body in Main.npc)
            {
                if (body.active)
                {
                    if (body.type == ModContent.NPCType<ApoBirdBody>())
                    {
                        return body.whoAmI;
                    }
                }
            }
            return -1;
        }

        public static Vector2 RotateVector2(Vector2 vec, float r = 0)
        {
            if (vec == Vector2.Zero) return Vector2.Zero;
            return (vec.ToRotation() + r).ToRotationVector2() * vec.Length();
        }
        #endregion
    }

    public class TwilightGProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public override bool CloneNewInstances => true;
        public bool CanKill = true;
    }
    
}