using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Projectiles;
using Twilight.UI;

namespace Twilight.NPCs
{
    public class ApoBirdClaw : ModNPC
    {
        public Vector2 RelaPos = Vector2.Zero;

        public enum State
        {
            Normal,
            ClawAttack,
            HoldingUpForPeck,
            ClawAlt,
        }

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
            NPC.HitSound = SoundID.NPCHit3;
            NPC.DeathSound = SoundID.NPCDeath3;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.aiStyle = -1;
            NPC.dontTakeDamageFromHostiles = true;
            NPC.dontTakeDamage = true;
            NPC.behindTiles = true;
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }

        }

        public override void AI()
        {

            Player owner = Main.player[(int)NPC.ai[3]];
            if (BirdUtils.FindArmLeft() != NPC.whoAmI && BirdUtils.FindArmRight() != NPC.whoAmI)
            {
                NPC.active = false;
                return;
            }
            if (BirdUtils.FindBody() == -1)
            {
                NPC.active = false;
                return;
            }
            NPC Body = Main.npc[BirdUtils.FindBody()];
            if (NPC.ai[1] == (int)State.Normal)      //正常移动
            {
                Movement(Body.Center + new Vector2(750 * NPC.ai[0], -300));
                RelaPos = NPC.Center - Body.Center;
            }
            else if (NPC.ai[1] == (int)State.ClawAttack)     //下砸
            {
                NPC.Center = Body.Center + RelaPos;
                NPC.ai[2]++;
                if (NPC.ai[2] < 60)     //抬手
                {
                    NPC.Center += new Vector2(0, -15);
                }
                else if (NPC.ai[2] < 80)      //砸
                {
                    NPC.Center += new Vector2(0, 60);
                    if (NPC.ai[2] == 70)
                    {
                        int dmg = (int)(TwilightData.ClawDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)NPC.ai[3]]));
                        Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), NPC.Center + new Vector2(0, 300), Vector2.Zero, ModContent.ProjectileType<MeleeHitEffect>(), dmg, 0, (int)NPC.ai[3]);
                        SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/ClawAttack") { Volume = 0.5f }, owner.Center);
                    }
                }
                else if (NPC.ai[2] < 110)
                {

                }
                else if (NPC.ai[2] < 140)    //收回
                {
                    NPC.Center += new Vector2(0, -10);
                }
                else          //复位
                {
                    //Body.ai[1] = 0;
                    NPC.ai[1] = (int)State.Normal;
                    NPC.ai[2] = 0;
                }
                //move
                RelaPos = NPC.Center - Body.Center;
                if (NPC.ai[2] == 80)
                {
                    Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(25, 1);
                }
            }
            else if (NPC.ai[1] == (int)State.HoldingUpForPeck)
            {
                NPC.Center = Body.Center + RelaPos;
                NPC.ai[2]++;

                if (NPC.ai[2] > 45 && NPC.ai[2] < 95)
                {
                    NPC.Center -= new Vector2(0, 12);
                }

                if (NPC.ai[2] > 170 && NPC.ai[2] < 220)
                {
                    NPC.Center += new Vector2(0, 12);
                }
                if (NPC.ai[2] >= 230)
                {
                    NPC.ai[2] = 0;
                    NPC.ai[1] = (int)State.Normal;
                }
                RelaPos = NPC.Center - Body.Center;
            }
            else if (NPC.ai[1] == (int)State.ClawAlt)
            {
                NPC.Center = Body.Center + RelaPos;
                NPC.ai[2]++;
                if (NPC.ai[2] < 60)     //抬手  180
                {
                    NPC.Center += new Vector2(0, 3);
                }
                else if (NPC.ai[2] < 80)      //砸 240
                {
                    NPC.Center += new Vector2(0, -12);
                }
                else if (NPC.ai[2] < 110)
                {

                }
                else if (NPC.ai[2] < 140)    //收回 60
                {
                    NPC.Center += new Vector2(0, 2);
                }
                else          //复位
                {
                    NPC.ai[1] = (int)State.Normal;
                    NPC.ai[2] = 0;
                }
                //move
                RelaPos = NPC.Center - Body.Center;
            }
        }


        public override bool PreKill()
        {
            return false;
        }

        public void Movement(Vector2 TargetPos, float vel = 4)
        {
            if (NPC.Distance(TargetPos) > 100)
            {
                NPC.Center = TargetPos + Vector2.Normalize(NPC.Center - TargetPos) * 99;
            }
            if (NPC.Center == TargetPos)
            {
                NPC.velocity = Vector2.Zero;
                return;
            }
            Vector2 MoveVel = Vector2.Normalize(TargetPos - NPC.Center) * vel;
            NPC.velocity = (NPC.velocity * 255 + MoveVel * 6) / 260;
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public static int IsStrike()
        {
            //if (BirdUtils.FindArmLeft() == -1 && BirdUtils.FindArmRight() == -1) return 0;
            if (BirdUtils.FindArmLeft() != -1)
            {
                if (Main.npc[BirdUtils.FindArmLeft()].ai[1] == (int)State.ClawAttack)
                {
                    return -1;
                }
            }
            if (BirdUtils.FindArmRight() != -1)
            {
                if (Main.npc[BirdUtils.FindArmRight()].ai[1] == (int)State.ClawAttack)
                {
                    return 1;
                }
            }
            return 0;
        }



        public static void InitiateStrike(bool Left = true)
        {
            NPC Arm, ArmAlt;
            if (Left)
            {
                if (BirdUtils.FindArmLeft() == -1) return;
                Arm = Main.npc[BirdUtils.FindArmLeft()];
                ArmAlt = Main.npc[BirdUtils.FindArmRight()];
            }
            else
            {
                if (BirdUtils.FindArmRight() == -1) return;
                Arm = Main.npc[BirdUtils.FindArmRight()];
                ArmAlt = Main.npc[BirdUtils.FindArmLeft()];
            }
            Arm.ai[1] = (int)State.ClawAttack;
            Arm.ai[2] = 0;

            ArmAlt.ai[1] = (int)State.ClawAlt;
            ArmAlt.ai[2] = 0;

            (Arm.ModNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[BirdUtils.FindBody()].Center;
            Arm.velocity = Vector2.Zero;

            (ArmAlt.ModNPC as ApoBirdClaw).RelaPos = ArmAlt.Center - Main.npc[BirdUtils.FindBody()].Center;
            ArmAlt.velocity = Vector2.Zero;
        }


        public static void HoldUpForPeck()
        {
            if (BirdUtils.FindArmLeft() == -1) return;
            NPC Arm = Main.npc[BirdUtils.FindArmLeft()];
            Arm.ai[1] = (int)State.HoldingUpForPeck;
            Arm.ai[2] = 0;
            (Arm.ModNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[BirdUtils.FindBody()].Center;
            Arm.velocity = Vector2.Zero;
            if (BirdUtils.FindArmRight() == -1) return;
            Arm = Main.npc[BirdUtils.FindArmRight()];
            Arm.ai[1] = (int)State.HoldingUpForPeck;
            Arm.ai[2] = 0;
            (Arm.ModNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[BirdUtils.FindBody()].Center;
            Arm.velocity = Vector2.Zero;
        }
    }
}