using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Projectiles;
using Twilight.Sky;
using Twilight.UI;

namespace Twilight.NPCs
{
    public class ApoBirdBody : ModNPC
    {
        public enum State
        {
            Default,
            ClawAttack,
            EyeAttack,
            Beak,
            Lantern,
            Justice,
        }

        /// <summary>
        /// 翅膀的旧角度
        /// </summary>
        public float? OldWingRot = null;

        /// <summary>
        /// 翅膀的角度
        /// </summary>
        public float WingRot = 0;

        /// <summary>
        /// 翅膀的亮度
        /// </summary>
        public float WingLight = 0;

        /// <summary>
        /// 正常情况下终末鸟翅膀的摆动
        /// </summary>
        public float NormalWingTimer = 0;

        /// <summary>
        /// 额外计时器
        /// </summary>
        public float ExtraTimer
        {
            get
            {
                return NPC.localAI[2];
            }
            set
            {
                NPC.localAI[2] = value;
            }
        }

        /// <summary>
        /// 目标坐标
        /// </summary>
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(NPC.localAI[0], NPC.localAI[1]);
            }
            set
            {
                NPC.localAI[0] = value.X;
                NPC.localAI[1] = value.Y;
            }
        }

        /// <summary>
        /// 终末鸟当前状态
        /// </summary>
        public State CurrentState
        {
            get
            {
                return (State)(int)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = (int)value;
            }
        }

        public static Vector2[] WingEyePos =   //70 100
        {
            new Vector2(261,-215),
            new Vector2(293,-318),
            new Vector2(321,-470),
            new Vector2(160,-447),
            new Vector2(168,-302),
            new Vector2(78,-420),

            new Vector2(836,-229),
            new Vector2(748,-321),
            new Vector2(612,-369),
            new Vector2(554,-465),
            new Vector2(466,-453),
            new Vector2(416,-347),
            new Vector2(446,-242),
            new Vector2(531,-125),
            new Vector2(377,-161),
        };
        public static float[] WingEyeRot =
        {
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            0,
            0,
            MathHelper.Pi / 12,

            MathHelper.Pi / 6,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            MathHelper.Pi / 12,
            0,
            MathHelper.Pi / 12,
        };
        public static float[] WingEyeScale =
        {
            0.25f,
            0.45f,
            0.55f,
            0.3f,
            0.3f,
            0.35f,

            0.3f,
            0.45f,
            0.45f,
            0.25f,
            0.3f,
            0.3f,
            0.25f,
            0.35f,
            0.45f,
        };

        public override string Texture => "Twilight/NPCs/ApoBirb";
        public override void SetStaticDefaults()
        {
            NPCID.Sets.ImmuneToAllBuffs[NPC.type] = true;
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 1;
            NPC.height = 1;
            NPC.damage = 2000;
            NPC.lifeMax = 10000;
            //NPC.HitSound = SoundID.NPCHit3;
            //NPC.DeathSound = SoundID.NPCDeath3;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontTakeDamage = true;
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }
        }

        public override void AI()
        {
            //NPC.ai[3] = Player.FindClosest(NPC.Center, 1, 1);
            Player owner = Main.player[(int)NPC.ai[3]];
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky)
            {
                NPC.active = false;
                return;
            }
            if (BirdUtils.FindBody() != NPC.whoAmI)
            {
                NPC.active = false;
                return;
            }
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                WingLight = (WingLight + 1) % 100;
            }
            if (NPC.ai[0] == 0)
            {
                NPC.ai[0] = 1;
                NPC.localAI[0] = -1;
                NPC.localAI[1] = -1;
                NPC.NewNPC(Terraria.Entity.InheritSource(NPC), (int)NPC.Center.X - 750, (int)NPC.Center.Y - 300, ModContent.NPCType<ApoBirdClaw>(), NPC.whoAmI, -1, 0, 0, owner.whoAmI);
                NPC.NewNPC(Terraria.Entity.InheritSource(NPC), (int)NPC.Center.X + 750, (int)NPC.Center.Y - 300, ModContent.NPCType<ApoBirdClaw>(), NPC.whoAmI, 1, 0, 0, owner.whoAmI);
                NPC.NewNPC(Terraria.Entity.InheritSource(NPC), (int)NPC.Center.X, (int)NPC.Center.Y - 250, ModContent.NPCType<ApoBirdHead>(), NPC.whoAmI, 0, 0, 0, owner.whoAmI);
            }
            NPC Head = Main.npc[BirdUtils.FindHead()];
            NPC ClawL = Main.npc[BirdUtils.FindArmLeft()];
            NPC ClawR = Main.npc[BirdUtils.FindArmRight()];

            //Main.NewText(CurrentState);

            if (CurrentState == State.ClawAttack && ApoBirdClaw.IsStrike() == 0)
            {
                OldWingRot = null;
                NormalWingTimer = 0;
                CurrentState = State.Default;
            }
            if (CurrentState == State.Lantern && Head.ai[1] == 0)
            {
                CurrentState = State.Default;
            }
            if (CurrentState == State.Justice && Head.ai[1] == 0)
            {
                CurrentState = State.Default;
            }

            //Main.NewText(NPC.ai[2]);

            if (ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                /*
                if (owner.channel)
                {
                    int dir = owner.direction;
                    Vector2 MouseDir = Vector2.Normalize(Main.MouseWorld - owner.Center);
                    owner.itemRotation = (float)Math.Atan2(MouseDir.Y * dir, MouseDir.X * dir);
                }
                */
                if (owner.GetModPlayer<TwilightPlayer>().AbilityCD == 0 && CurrentState == State.Default)
                {
                    if (owner.channelLeft())
                    {
                        if (owner.GetModPlayer<TwilightPlayer>().NextNormalAttack == 0)
                        {
                            owner.GetModPlayer<TwilightPlayer>().NextNormalAttack = 1;
                            owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.EyeBulletCD;
                            InitiateBullet();
                        }
                        else
                        {
                            owner.GetModPlayer<TwilightPlayer>().NextNormalAttack = 0;
                            owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.ClawCD;
                            bool Left = Main.MouseWorld.X <= Main.LocalPlayer.Center.X;
                            InitiateStrike(Left);
                        }
                    }

                    if (owner.channelRight())
                    {
                        if (owner.GetModPlayer<TwilightPlayer>().AbilityCD == 0)
                        {
                            switch (owner.GetModPlayer<TwilightPlayer>().CurrentEgg)
                            {
                                case 0:
                                    EyeLantern2.CloseLantern2();
                                    InitiateLantern();
                                    owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.EyeSpecialCD;
                                    break;
                                case 1:
                                    InitiateJustice();
                                    owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.JusticeSpecialCD;
                                    break;
                                case 2:
                                    InitiatePeck();
                                    owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.PeckSpecialCD;
                                    break;
                                default:
                                    break;
                            }

                        }
                    }

                    if (owner.channelMiddle())
                    {
                        if (owner.GetModPlayer<TwilightPlayer>().AbilityCD == 0)
                        {
                            owner.GetModPlayer<TwilightPlayer>().AbilityCD = TwilightData.PhaseSwitchCD;
                            owner.GetModPlayer<TwilightPlayer>().CurrentEgg = (owner.GetModPlayer<TwilightPlayer>().CurrentEgg + 1) % 3;
                            owner.GetModPlayer<TwilightUIPlayer>().Initialise(owner.GetModPlayer<TwilightPlayer>().CurrentEgg);
                            if (owner.GetModPlayer<TwilightPlayer>().CurrentEgg != 0)
                            {
                                EyeLantern2.CloseLantern2();
                            }
                        }
                    }
                }

            }


            bool Moven = false;

            if (ApoBirdSky.CurrentState == ApoBirdSky.State.ApoSky)
            {
                if (CurrentState == State.Default)
                {
                    OldWingRot = null;
                    NormalWingTimer = (NormalWingTimer + 1) % 200;
                    WingRot = (float)Math.Sin(MathHelper.TwoPi * NormalWingTimer / 200f) * MathHelper.Pi / 60;
                }
            }


            #region 控制下砸时的身体活动
            if (CurrentState == State.ClawAttack)
            {
                NPC StrikingClaw;
                //Main.NewText(ApoBirdClaw.IsStrike());
                if (ApoBirdClaw.IsStrike() == -1)
                {
                    StrikingClaw = ClawL;
                }
                else if (ApoBirdClaw.IsStrike() == 1)
                {
                    StrikingClaw = ClawR;
                }
                else
                {
                    StrikingClaw = null;
                }

                if (StrikingClaw != null)
                {
                    if (StrikingClaw.ai[2] < 60)
                    {
                        if (OldWingRot == null)
                        {
                            OldWingRot = WingRot;
                        }
                        NPC.rotation = MathHelper.Lerp(0, MathHelper.Pi / 12 * (-ApoBirdClaw.IsStrike()), StrikingClaw.ai[2] / 60f);
                        WingRot = MathHelper.Lerp(OldWingRot.Value, MathHelper.Pi / 10, StrikingClaw.ai[2] / 60f);
                    }
                    else if (StrikingClaw.ai[2] < 80)
                    {
                        NPC.rotation = MathHelper.Lerp(MathHelper.Pi / 12 * (-ApoBirdClaw.IsStrike()), 0, (StrikingClaw.ai[2] - 60) / 20f);
                        WingRot = MathHelper.Lerp(MathHelper.Pi / 10, 0, (StrikingClaw.ai[2] - 60) / 20f);
                    }
                    else
                    {
                        NPC.rotation = 0;
                        WingRot = 0;
                    }
                    Head.rotation = NPC.rotation;
                }
                else
                {
                    NPC.rotation = 0;
                    WingRot = 0;
                }
            }
            #endregion


            #region 射线攻击活动
            if (CurrentState == State.EyeAttack)
            {
                ExtraTimer++;
                if (ExtraTimer < 60)
                {
                    if (OldWingRot == null)
                    {
                        OldWingRot = WingRot;
                    }
                    WingRot = MathHelper.Lerp(OldWingRot.Value, 0, ExtraTimer / 60f);
                }
                else
                {
                    WingRot = 0;
                    if (ExtraTimer == 60)
                    {
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/EyeBulletStart") { Volume = 0.5f }, owner.Center);
                        int dmg = (int)(TwilightData.BulletDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)NPC.ai[3]]));

                        //左翅膀
                        Vector2 WingPos = NPC.Center + new Vector2(5, 0);
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 EyePos = WingEyePos[i].RotatedBy(-WingRot) * 0.9f;
                            EyePos.X = -EyePos.X;
                            float EyeRot = WingEyeRot[i];
                            int protmp = Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), WingPos + EyePos, Vector2.Zero, ModContent.ProjectileType<EyeBulletReady>(), dmg, 0, (int)NPC.ai[3]);
                            Main.projectile[protmp].ai[0] = -Main.rand.Next(60);
                            Main.projectile[protmp].rotation = EyeRot;
                            (Main.projectile[protmp].ModProjectile as EyeBulletReady).Pos = WingPos + EyePos - NPC.Center;
                        }
                        //右翅膀
                        WingPos = NPC.Center + new Vector2(-5, 0);
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 EyePos = WingEyePos[i].RotatedBy(WingRot) * 0.9f;
                            float EyeRot = -WingEyeRot[i];
                            int protmp = Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), WingPos + EyePos, Vector2.Zero, ModContent.ProjectileType<EyeBulletReady>(), dmg, 0, (int)NPC.ai[3]);
                            Main.projectile[protmp].ai[0] = -Main.rand.Next(60);
                            Main.projectile[protmp].rotation = EyeRot;
                            (Main.projectile[protmp].ModProjectile as EyeBulletReady).Pos = WingPos + EyePos - NPC.Center;
                        }

                    }
                    if (ExtraTimer > 121)
                    {
                        ExtraTimer = 0;
                        CurrentState = State.Default;
                        OldWingRot = null;
                        NormalWingTimer = 0;
                    }
                }
            }
            #endregion


            #region 尖喙时翅膀运动相关
            if (CurrentState == State.Beak)
            {
                if (ExtraTimer < 120)
                {
                    if (OldWingRot == null)
                    {
                        OldWingRot = WingRot;
                    }
                    WingRot = MathHelper.Lerp(OldWingRot.Value, MathHelper.Pi / 15, ExtraTimer / 120f);
                }
                else if (ExtraTimer < 130)
                {
                    WingRot = MathHelper.Pi / 15;
                }
                else if (ExtraTimer < 140)
                {
                    WingRot = MathHelper.Lerp(MathHelper.Pi / 15, 0, (ExtraTimer - 130) / 10f);
                }
                else
                {
                    WingRot = 0;
                }
            }
            #endregion



            if (CurrentState == State.Default ||
                CurrentState == State.ClawAttack ||
                CurrentState == State.EyeAttack ||
                CurrentState == State.Lantern ||
                CurrentState == State.Justice)
            {
                Movement(owner.Center);
                Moven = true;
            }

            if (CurrentState == State.Beak)
            {
                ExtraTimer++;
                if (ExtraTimer < 60)            //移动到指定位置
                {
                    Movement(owner.Center + TargetPos * ExtraTimer / 60f);
                    Moven = true;
                }
                else if (ExtraTimer < 170)    //60-100停止，100-160攻击，160-170停止
                {
                    if (ExtraTimer == 100)
                    {
                        int dmg = (int)(TwilightData.PeckDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)NPC.ai[3]]));
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), NPC.Center + new Vector2(0, 138), Vector2.Zero, ModContent.ProjectileType<PeckCenter>(), dmg, 0, (int)NPC.ai[3]);
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/PeckAttack") { Volume = 0.5f }, owner.Center);
                    }
                    Movement(owner.Center + TargetPos);
                    Moven = true;
                }
                else if (ExtraTimer < 230)     //返回
                {
                    Movement(owner.Center + TargetPos * (230 - ExtraTimer) / 60f);
                    Moven = true;
                }
                else
                {
                    ExtraTimer = 0;
                    CurrentState = State.Default;
                    OldWingRot = null;
                    NormalWingTimer = 0;
                }
            }

            if (!Moven)
            {
                NPC.velocity *= 0.8f;
            }
        }

        //射线攻击
        public void InitiateBullet()
        {
            CurrentState = State.EyeAttack;
            ExtraTimer = 0;
        }

        //高鸟特殊攻击
        public void InitiateJustice()
        {
            CurrentState = State.Justice;
            ExtraTimer = 0;
            ApoBirdHead.HoldUpForJustice();
        }

        //小喙特殊攻击
        public void InitiatePeck()
        {
            CurrentState = State.Beak;
            ExtraTimer = 0;
            TargetPos = Main.MouseWorld - Main.player[(int)NPC.ai[3]].Center + new Vector2(0, -139);
            ApoBirdClaw.HoldUpForPeck();
        }

        //大鸟特殊攻击
        public void InitiateLantern()
        {
            CurrentState = State.Lantern;
            ExtraTimer = 0;
            ApoBirdHead.HoldUpForLantern();
        }

        //巨爪拍击
        public void InitiateStrike(bool Left)
        {
            CurrentState = State.ClawAttack;
            ApoBirdClaw.InitiateStrike(Left);
        }

        public override bool PreKill() => false;


        public void Movement(Vector2 TargetPos, float vel = 12)
        {
            if (NPC.Distance(TargetPos) > 100)
            {
                NPC.Center = TargetPos + Vector2.Normalize(NPC.Center - TargetPos) * 99;
            }
            if (NPC.Distance(TargetPos) > 20f)
            {
                Vector2 desiredVelocity = Vector2.Normalize(TargetPos - NPC.Center - NPC.velocity) * vel;
                Vector2 velocity2 = NPC.velocity;
                NPC.SimpleFlyMovement(desiredVelocity, 0.5f);
                NPC.velocity = Vector2.Lerp(NPC.velocity, velocity2, 0.5f);
            }
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }

        public bool HasTarget()
        {
            foreach (NPC target in Main.npc)
            {
                if (target.active && target.Distance(NPC.Center) <= 2000 && target.CanBeChasedBy())
                {
                    return true;
                }
            }
            return false;
        }
    }


}