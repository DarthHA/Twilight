using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Projectiles;
using Twilight.UI;

namespace Twilight.NPCs
{
    public class ApoBirdClaw : ModNPC
    {
        public Vector2 RelaPos = Vector2.Zero;

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
            npc.HitSound = SoundID.NPCHit3;
            npc.DeathSound = SoundID.NPCDeath3;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;
            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.aiStyle = -1;
            npc.dontTakeDamageFromHostiles = true;
            npc.dontTakeDamage = true;
            npc.behindTiles = true;
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

            Player owner = Main.player[(int)npc.ai[3]];
            if (Twilight.FindArmLeft() != npc.whoAmI && Twilight.FindArmRight() != npc.whoAmI)
            {
                npc.active = false;
                return;
            }
            if (Twilight.FindBody() == -1)
            {
                npc.active = false;
                return;
            }
            NPC Body = Main.npc[Twilight.FindBody()];
            if (npc.ai[1] == 0)      //正常移动
            {
                Movement(Body.Center + new Vector2(750 * npc.ai[0], -300));
                RelaPos = npc.Center - Body.Center;
            }
            else if (npc.ai[1] == 1)     //下砸
            {
                npc.Center = Body.Center + RelaPos;
                npc.ai[2]++;
                if (npc.ai[2] < 60)     //抬手
                {
                    npc.Center += new Vector2(0, -15);
                }
                else if (npc.ai[2] < 80)      //砸
                {
                    npc.Center += new Vector2(0, 60);
                    if (npc.ai[2] == 70)
                    {
                        int dmg = (int)(Twilight.ClawDamage * TwilightPlayer.GetDamageBonus(Main.player[(int)npc.ai[3]]));
                        Projectile.NewProjectile(npc.Center + new Vector2(0, 300), Vector2.Zero, ModContent.ProjectileType<MeleeHitEffect>(), dmg, 0, (int)npc.ai[3]);
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/ClawAttack"), owner.Center);
                    }
                }
                else if (npc.ai[2] < 110)
                {

                }
                else if (npc.ai[2] < 140)    //收回
                {
                    npc.Center += new Vector2(0, -10);
                }
                else          //复位
                {
                    //Body.ai[1] = 0;
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }
                //move
                RelaPos = npc.Center - Body.Center;
                if (npc.ai[2] == 80)
                {
                    Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>().InitialiseShake(25, 1);
                }
            }
            else if (npc.ai[1] == 2)
            {
                npc.Center = Body.Center + RelaPos;
                npc.ai[2]++;

                if (npc.ai[2] > 45 && npc.ai[2] < 95)
                {
                    npc.Center -= new Vector2(0, 12);
                }

                if (npc.ai[2] > 170 && npc.ai[2] < 220)
                {
                    npc.Center += new Vector2(0, 12);
                }
                if (npc.ai[2] >= 230)
                {
                    npc.ai[2] = 0;
                    npc.ai[1] = 0;
                }
                RelaPos = npc.Center - Body.Center;
            }
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public void Movement(Vector2 TargetPos, float vel = 4)
        {
            if (npc.Distance(TargetPos) > 100)
            {
                npc.Center = TargetPos + Vector2.Normalize(npc.Center - TargetPos) * 99;
            }
            if (npc.Center == TargetPos)
            {
                npc.velocity = Vector2.Zero;
                return;
            }
            Vector2 MoveVel = Vector2.Normalize(TargetPos - npc.Center) * vel;
            npc.velocity = (npc.velocity * 255 + MoveVel * 6) / 260;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public static int IsStrike()
        {
            //if (Twilight.FindArmLeft() == -1 && Twilight.FindArmRight() == -1) return 0;
            if (Twilight.FindArmLeft() != -1)
            {
                if (Main.npc[Twilight.FindArmLeft()].ai[1] == 1)
                {
                    return -1;
                }
            }
            if (Twilight.FindArmRight() != -1)
            {
                if (Main.npc[Twilight.FindArmRight()].ai[1] == 1)
                {
                    return 1;
                }
            }
            return 0;
        }



        public static void InitiateStrike(bool Left = true)
        {
            NPC Arm;
            if (Left)
            {
                if (Twilight.FindArmLeft() == -1) return;
                Arm = Main.npc[Twilight.FindArmLeft()];
            }
            else
            {
                if (Twilight.FindArmRight() == -1) return;
                Arm = Main.npc[Twilight.FindArmRight()];
            }
            Arm.ai[1] = 1;
            Arm.ai[2] = 0;
            (Arm.modNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[Twilight.FindBody()].Center;
            Arm.velocity = Vector2.Zero;
        }


        public static void HoldUpForPeck()
        {
            if (Twilight.FindArmLeft() == -1) return;
            NPC Arm = Main.npc[Twilight.FindArmLeft()];
            Arm.ai[1] = 2;
            Arm.ai[2] = 0;
            (Arm.modNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[Twilight.FindBody()].Center;
            Arm.velocity = Vector2.Zero;
            if (Twilight.FindArmRight() == -1) return;
            Arm = Main.npc[Twilight.FindArmRight()];
            Arm.ai[1] = 2;
            Arm.ai[2] = 0;
            (Arm.modNPC as ApoBirdClaw).RelaPos = Arm.Center - Main.npc[Twilight.FindBody()].Center;
            Arm.velocity = Vector2.Zero;
        }
    }
}