using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Projectiles;
using Twilight.Sky;

namespace Twilight.NPCs
{
    public class ApoBirdBody : ModNPC
    {
        public float WingRot = 0;
        public float WingLight = 0;
        public float ExtraTimer
        {
            get
            {
                return npc.localAI[2];
            }
            set
            {
                npc.localAI[2] = value;
            }
        }
        public Vector2 TargetPos
        {
            get
            {
                return new Vector2(npc.localAI[0], npc.localAI[1]);
            }
            set
            {
                npc.localAI[0] = value.X;
                npc.localAI[1] = value.Y;
            }
        }
        public float CurrentState
        {
            get
            {
                return npc.ai[1];
            }
            set
            {
                npc.ai[1] = value;
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
            DisplayName.SetDefault("Apocalypse Bird");
            DisplayName.AddTranslation(GameCulture.Chinese, "天启鸟");
        }

        public override void SetDefaults()
        {
            npc.friendly = true;
            npc.width = 1;
            npc.height = 1;
            npc.damage = 2000;
            npc.lifeMax = 10000;
            //npc.HitSound = SoundID.NPCHit3;
            //npc.DeathSound = SoundID.NPCDeath3;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.dontTakeDamageFromHostiles = true;
            npc.dontTakeDamage = true;
            for (int i = 0; i < npc.buffImmune.Length; i++)
            {
                npc.buffImmune[i] = true;
            }
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Binah");
            musicPriority = MusicPriority.BossHigh;
            //NPCID.Sets.MustAlwaysDraw[npc.type] = true;
        }

        public override void AI()
        {
            //npc.ai[3] = Player.FindClosest(npc.Center, 1, 1);
            Player owner = Main.player[(int)npc.ai[3]];
            if (ApoBirdSky.CurrentState == 0)
            {
                npc.active = false;
                return;
            }
            if (Twilight.FindBody() != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (ApoBirdSky.CurrentState == 2)
            {
                WingLight = (WingLight + 1) % 100;
            }
            if (npc.ai[0] == 0)
            {
                npc.ai[0] = 1;
                npc.localAI[0] = -1;
                npc.localAI[1] = -1;
                NPC.NewNPC((int)npc.Center.X - 750, (int)npc.Center.Y - 300, ModContent.NPCType<ApoBirdClaw>(), npc.whoAmI, -1, 0, 0, owner.whoAmI);
                NPC.NewNPC((int)npc.Center.X + 750, (int)npc.Center.Y - 300, ModContent.NPCType<ApoBirdClaw>(), npc.whoAmI, 1, 0, 0, owner.whoAmI);
                NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y - 250, ModContent.NPCType<ApoBirdHead>(), npc.whoAmI, 0, 0, 0, owner.whoAmI);
            }
            NPC Head = Main.npc[Twilight.FindHead()];
            NPC ClawL = Main.npc[Twilight.FindArmLeft()];
            NPC ClawR = Main.npc[Twilight.FindArmRight()];

            //Main.NewText(CurrentState);

            if (CurrentState == 1 && ApoBirdClaw.IsStrike() == 0)
            {
                CurrentState = 0;
            }
            if (CurrentState == 2 && !EyeBulletReady.AnyBulletReady())
            {
                CurrentState = 0;
            }
            if (CurrentState == 4 && Head.ai[1] == 0)
            {
                CurrentState = 0;
            }
            if (CurrentState == 5 && Head.ai[1] == 0)
            {
                CurrentState = 0;
            }

            //Main.NewText(npc.ai[2]);

            if (ApoBirdSky.CurrentState == 2)
            {
                if (owner.channel)
                {
                    int dir = owner.direction;
                    Vector2 MouseDir = Vector2.Normalize(Main.MouseWorld - owner.Center);
                    owner.itemRotation = (float)Math.Atan2(MouseDir.Y * dir, MouseDir.X * dir);
                }
                if (owner.channel && !owner.GetModPlayer<TwilightPlayer>().RightClick)
                {
                    if (CurrentState == 0)
                    {
                        npc.ai[2]++;
                    }
                    if (npc.ai[2] == Twilight.EyeBulletCD)
                    {
                        npc.ai[2]++;
                        InitiateBullet();
                    }
                    if (npc.ai[2] == Twilight.EyeBulletCD + Twilight.ClawCD) 
                    {
                        npc.ai[2] = 0;
                        bool Left = Main.MouseWorld.X <= Main.LocalPlayer.Center.X;
                        InitiateStrike(Left);
                    }
                }

                if (owner.GetModPlayer<TwilightPlayer>().RightClick)
                {
                    if (CurrentState == 0)
                    {
                        if (owner.GetModPlayer<TwilightPlayer>().SpecialAttackCD == 0)
                        {
                            switch (owner.GetModPlayer<TwilightPlayer>().CurrentEgg)
                            {
                                case 0:
                                    InitiateLantern();
                                    owner.GetModPlayer<TwilightPlayer>().SpecialAttackCD = Twilight.EyeSpecialCD;
                                    break;
                                case 1:
                                    InitiateJustice();
                                    owner.GetModPlayer<TwilightPlayer>().SpecialAttackCD = Twilight.JusticeSpecialCD;
                                    break;
                                case 2:
                                    InitiatePeck();
                                    owner.GetModPlayer<TwilightPlayer>().SpecialAttackCD = Twilight.PeckSpecialCD;
                                    break;
                                default:
                                    break;
                            }

                        }

                    }
                }

            }


            bool Moven = false;


            #region 控制下砸时的身体活动
            if (CurrentState == 1)
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
                    WingRot = 0;
                }
                else
                {
                    StrikingClaw = null;
                }

                if (StrikingClaw != null)
                {
                    if (StrikingClaw.ai[2] < 60)
                    {
                        npc.rotation = MathHelper.Pi / 12 * StrikingClaw.ai[2] / 60 * (-ApoBirdClaw.IsStrike());
                        WingRot = MathHelper.Pi / 10 * StrikingClaw.ai[2] / 60;
                    }
                    else if (StrikingClaw.ai[2] < 80)
                    {
                        npc.rotation = MathHelper.Pi / 12 * (80 - StrikingClaw.ai[2]) / 20 * (-ApoBirdClaw.IsStrike());
                        WingRot = MathHelper.Pi / 10 * (80 - StrikingClaw.ai[2]) / 20;
                    }
                    else
                    {
                        npc.rotation = 0;
                        WingRot = 0;
                    }
                    Head.rotation = npc.rotation;
                }
                else
                {
                    npc.rotation = 0;
                    WingRot = 0;
                }
            }
            #endregion

            #region 尖喙时翅膀运动相关
            if (CurrentState == 3)
            {
                if (ExtraTimer < 120)
                {
                    WingRot = MathHelper.Pi / 15 * ExtraTimer / 120;
                }
                else if (ExtraTimer < 130)
                {
                    WingRot = MathHelper.Pi / 15;
                }
                else if (ExtraTimer < 140)
                {
                    WingRot = MathHelper.Pi / 15 * (140 - ExtraTimer) / 10;
                }
                else
                {
                    WingRot = 0;
                }
            }
            #endregion



            if (CurrentState == 0 ||
                CurrentState == 1 ||
                CurrentState == 2 ||
                CurrentState == 4 ||
                CurrentState == 5)
            {
                Movement(owner.Center);
                Moven = true;
            }

            if (CurrentState == 3)
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
                        int dmg = (int)(Twilight.PeckDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)npc.ai[3]]));
                        Projectile.NewProjectile(npc.Center + new Vector2(0, 138), Vector2.Zero, ModContent.ProjectileType<PeckCenter>(), dmg, 0, (int)npc.ai[3]);
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/PeckAttack"), owner.Center);
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
                    CurrentState = 0;
                }
            }

            if (!Moven)
            {
                npc.velocity *= 0.8f;
            }
        }

        //射线攻击
        public void InitiateBullet()
        {
            CurrentState = 2;
            //左
            int dmg = (int)(Twilight.BulletDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)npc.ai[3]]));
            Vector2 WingPos = npc.Center + new Vector2(5, 0);
            for (int i = 0; i < 6; i++)
            {
                Vector2 EyePos = WingEyePos[i] * 0.9f;
                EyePos.X = -EyePos.X;
                float EyeRot = WingEyeRot[i];
                int protmp = Projectile.NewProjectile(WingPos + EyePos, Vector2.Zero, ModContent.ProjectileType<EyeBulletReady>(), dmg, 0, (int)npc.ai[3]);
                Main.projectile[protmp].ai[0] = -Main.rand.Next(60);
                Main.projectile[protmp].rotation = EyeRot;
            }

            WingPos = npc.Center + new Vector2(-5, 0);
            for (int i = 0; i < 6; i++)
            {
                Vector2 EyePos = WingEyePos[i] * 0.9f;
                float EyeRot = -WingEyeRot[i];
                int protmp = Projectile.NewProjectile(WingPos + EyePos, Vector2.Zero, ModContent.ProjectileType<EyeBulletReady>(), dmg, 0, (int)npc.ai[3]);
                Main.projectile[protmp].ai[0] = -Main.rand.Next(60);
                Main.projectile[protmp].rotation = EyeRot;
            }

        }

        //高鸟特殊攻击
        public void InitiateJustice()
        {
            CurrentState = 5;
            ExtraTimer = 0;
            ApoBirdHead.HoldUpForJustice();
        }

        //小喙特殊攻击
        public void InitiatePeck()
        {
            CurrentState = 3;
            ExtraTimer = 0;
            TargetPos = Main.MouseWorld - Main.player[(int)npc.ai[3]].Center + new Vector2(0, -139);
            ApoBirdClaw.HoldUpForPeck();
        }

        //大鸟特殊攻击
        public void InitiateLantern()
        {
            CurrentState = 4;
            ExtraTimer = 0;
            ApoBirdHead.HoldUpForLantern();
        }

        //巨爪拍击
        public void InitiateStrike(bool Left)
        {
            CurrentState = 1;
            ApoBirdClaw.InitiateStrike(Left);
        }

        public override bool PreNPCLoot() => false;

        public void Movement(Vector2 TargetPos, float vel = 12)
        {
            if (npc.Distance(TargetPos) > 100)
            {
                npc.Center = TargetPos + Vector2.Normalize(npc.Center - TargetPos) * 99;
            }
            if (npc.Distance(TargetPos) > 20f)
            {
                Vector2 desiredVelocity = Vector2.Normalize(TargetPos - npc.Center - npc.velocity) * vel;
                Vector2 velocity2 = npc.velocity;
                npc.SimpleFlyMovement(desiredVelocity, 0.5f);
                npc.velocity = Vector2.Lerp(npc.velocity, velocity2, 0.5f);
            }
        }

        public override bool CheckActive() => false;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }


        public bool HasTarget()
        {
            foreach (NPC target in Main.npc)
            {
                if (target.active && target.Distance(npc.Center) <= 2000 && target.CanBeChasedBy())
                {
                    return true;
                }
            }
            return false;
        }
    }


}