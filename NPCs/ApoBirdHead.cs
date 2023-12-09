using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Projectiles;

namespace Twilight.NPCs
{
    public class ApoBirdHead : ModNPC
    {
        public float EyeRotation = -MathHelper.Pi / 2;

        public enum State
        {
            Normal,
            Lantern,
            Justice
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
            if (BirdUtils.FindHead() != NPC.whoAmI)
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
            if (NPC.ai[1] == (int)State.Normal)
            {
                Vector2 BaseHeadPos = Body.Center - new Vector2(0, 250);
                Vector2 MouseRelaPos = Main.MouseWorld - BaseHeadPos;
                if (MouseRelaPos.Length() > 48) MouseRelaPos = Vector2.Normalize(MouseRelaPos) * 48;
                MovementAlt(Body, BaseHeadPos + MouseRelaPos);
            }
            else if (NPC.ai[1] == (int)State.Lantern)
            {
                if (NPC.ai[2] == 30)
                {
                    SoundEngine.PlaySound(new SoundStyle("Twilight/Sounds/LanternAttract") { Volume = 0.5f }, owner.Center);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<EyeLantern>(), 0, 0, default);
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<EyeLantern2>(), 0, 0, default);
                }
                NPC.ai[2]++;
                if (NPC.ai[2] < 30)
                {
                    Movement(Body.Center - new Vector2(0, 250 + 150f * NPC.ai[2] / 30f));
                }
                else if (NPC.ai[2] < 130)
                {
                    Movement(Body.Center - new Vector2(0, 400));
                }
                else
                {
                    Movement(Body.Center - new Vector2(0, 250 + 150f * (160f - NPC.ai[2]) / 30f));
                }
                if (NPC.ai[2] == 100)
                {
                    EyeLantern.CloseLantern();
                }
                if (NPC.ai[2] >= 160)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                }
            }
            else if (NPC.ai[1] == (int)State.Justice)
            {
                NPC.ai[2]++;
                if (NPC.ai[2] == 30)
                {
                    Projectile.NewProjectile(Terraria.Entity.InheritSource(NPC), NPC.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect>(), 9999, 0, (int)NPC.ai[3]);
                }
                if (NPC.ai[2] < 30)
                {
                    Movement(Body.Center - new Vector2(0, 250 + 100f * NPC.ai[2] / 30f));
                }
                else if (NPC.ai[2] < 230)
                {
                    Movement(Body.Center - new Vector2(0, 350));
                }
                else
                {
                    Movement(Body.Center - new Vector2(0, 250 + 100f * (260f - NPC.ai[2]) / 30f));
                }
                if (NPC.ai[2] >= 260)
                {
                    NPC.ai[1] = 0;
                    NPC.ai[2] = 0;
                }
            }

            //Main.NewText(Vector2.Distance(Main.MouseWorld, NPC.Center));
        }


        public override bool PreKill()
        {
            return false;
        }

        public void MovementAlt(NPC Body, Vector2 TargetPos, float vel = 12)
        {
            Vector2 BaseHeadPos = Body.Center - new Vector2(0, 250);
            if (NPC.Distance(BaseHeadPos) > 50)
            {
                NPC.Center = BaseHeadPos + Vector2.Normalize(NPC.Center - BaseHeadPos) * 49;
            }
            if (NPC.Center == TargetPos)
            {
                NPC.velocity = Vector2.Zero;
                return;
            }
            Vector2 MoveVel = Vector2.Normalize(TargetPos - NPC.Center) * vel;
            NPC.velocity = (NPC.velocity * 155 + MoveVel * 6) / 160;
        }


        public void Movement(Vector2 TargetPos, float vel = 4)
        {
            if (NPC.Distance(TargetPos) > 50)
            {
                NPC.Center = TargetPos + Vector2.Normalize(NPC.Center - TargetPos) * 49;
            }
            if (NPC.Center == TargetPos)
            {
                NPC.velocity = Vector2.Zero;
                return;
            }
            Vector2 MoveVel = Vector2.Normalize(TargetPos - NPC.Center) * vel;
            NPC.velocity = (NPC.velocity * 155 + MoveVel * 6) / 160;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return false;
        }
        public override bool CheckActive()
        {
            return false;
        }

        public static void HoldUpForLantern()
        {
            if (BirdUtils.FindHead() == -1) return;
            NPC Head = Main.npc[BirdUtils.FindHead()];
            Head.ai[1] = (int)State.Lantern;
            Head.ai[2] = 0;
        }

        public static void HoldUpForJustice()
        {
            if (BirdUtils.FindHead() == -1) return;
            NPC Head = Main.npc[BirdUtils.FindHead()];
            Head.ai[1] = (int)State.Justice;
            Head.ai[2] = 0;
        }
    }
}