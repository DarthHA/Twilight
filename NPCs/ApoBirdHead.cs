using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Projectiles;

namespace Twilight.NPCs
{
    public class ApoBirdHead : ModNPC
    {
        public int Head = -1;

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
            if (Twilight.FindHead() != npc.whoAmI)
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
            if (npc.ai[1] == 0)
            {
                Movement(Body.Center - new Vector2(0, 250));
            }
            else if (npc.ai[1] == 1)
            {
                if (npc.ai[2] == 30)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/LanternAttract"), owner.Center);
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeLantern>(), 0, 0, default);
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<EyeLantern2>(), 0, 0, default);
                }
                npc.ai[2]++;
                if (npc.ai[2] < 30)
                {
                    Movement(Body.Center - new Vector2(0, 250 + 150f * npc.ai[2] / 30f));
                }
                else if (npc.ai[2] < 130)
                {
                    Movement(Body.Center - new Vector2(0, 400));
                }
                else
                {
                    Movement(Body.Center - new Vector2(0, 250 + 150f * (160f - npc.ai[2]) / 30f));
                }
                if (npc.ai[2] == 100)
                {
                    EyeLantern.CloseLantern();
                }
                if (npc.ai[2] >= 160)
                {
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }
            }
            else if (npc.ai[1] == 2)
            {
                npc.ai[2]++;
                if (npc.ai[2] == 30)
                {
                    Projectile.NewProjectile(npc.Center, Vector2.Zero, ModContent.ProjectileType<JustitiaEffect>(), 9999, 0, (int)npc.ai[3]);
                }
                if (npc.ai[2] < 30)
                {
                    Movement(Body.Center - new Vector2(0, 250 + 100f * npc.ai[2] / 30f));
                }
                else if (npc.ai[2] < 230)
                {
                    Movement(Body.Center - new Vector2(0, 350));
                }
                else
                {
                    Movement(Body.Center - new Vector2(0, 250 + 100f * (260f - npc.ai[2]) / 30f));
                }
                if (npc.ai[2] >= 260)
                {
                    npc.ai[1] = 0;
                    npc.ai[2] = 0;
                }
            }

            //Main.NewText(Vector2.Distance(Main.MouseWorld, npc.Center));
        }

        public override bool PreNPCLoot()
        {
            return false;
        }

        public void Movement(Vector2 TargetPos, float vel = 4)
        {
            if (npc.Distance(TargetPos) > 50)
            {
                npc.Center = TargetPos + Vector2.Normalize(npc.Center - TargetPos) * 49;
            }
            if (npc.Center == TargetPos)
            {
                npc.velocity = Vector2.Zero;
                return;
            }
            Vector2 MoveVel = Vector2.Normalize(TargetPos - npc.Center) * vel;
            npc.velocity = (npc.velocity * 155 + MoveVel * 6) / 160;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public static void HoldUpForLantern()
        {
            if (Twilight.FindHead() == -1) return;
            NPC Head = Main.npc[Twilight.FindHead()];
            Head.ai[1] = 1;
            Head.ai[2] = 0;
        }

        public static void HoldUpForJustice()
        {
            if (Twilight.FindHead() == -1) return;
            NPC Head = Main.npc[Twilight.FindHead()];
            Head.ai[1] = 2;
            Head.ai[2] = 0;
        }
    }
}