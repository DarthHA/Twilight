using Microsoft.Xna.Framework;
using Terraria;

namespace Twilight
{
    public static class PlayerUtils
    {
        public static bool IsDead(this Player player)
        {
            return !player.active || player.dead || player.ghost;
        }

        public static bool channelLeft(this Player player)
        {
            if (Main.mouseLeft && !Main.mouseRight && !Main.mouseMiddle && !player.mouseInterface && !Main.blockMouse && player.ItemAnimationActive)
            {
                return true;
            }
            return false;
        }
        public static bool channelRight(this Player player)
        {
            if (!Main.mouseLeft && Main.mouseRight && !Main.mouseMiddle && !player.mouseInterface && !Main.blockMouse && player.ItemAnimationActive)
            {
                return true;
            }
            return false;
        }
        public static bool channelBoth(this Player player)
        {
            if (Main.mouseLeft && Main.mouseRight && !Main.mouseMiddle && !player.mouseInterface && !Main.blockMouse && player.ItemAnimationActive)
            {
                return true;
            }
            return false;
        }

        public static bool channelMiddle(this Player player)
        {
            if (Main.mouseMiddle && !Main.mouseLeft && !Main.mouseRight && !player.mouseInterface && !Main.blockMouse)
            {
                return true;
            }
            return false;
        }

        public static bool channelNo(this Player player)
        {
            return !player.channelBoth() && !player.channelLeft() && !player.channelRight() && !player.channelMiddle();
        }

        public static int FindProjectile(int type)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == type)
                {
                    return proj.whoAmI;
                }
            }
            return -1;
        }

        public static int FindEnemy(Vector2 Pos, float range = 800, bool IgnoreTile = true)
        {
            int result = -1;
            float distance = -1;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.CanBeChasedBy())
                {
                    if (!IgnoreTile && !Collision.CanHit(npc.position, npc.width, npc.height, Pos, 1, 1))
                    {
                        continue;
                    }
                    if (npc.Distance(Pos) < range)
                    {
                        if (npc.Distance(Pos) < distance || distance == -1)
                        {
                            result = npc.whoAmI;
                            distance = npc.Distance(Pos);
                        }
                    }
                }
            }
            return result;
        }



        public static void SetIFrame(this Player owner, int time)
        {
            bool success = false;
            for (int i = 0; i < owner.hurtCooldowns.Length; i++)
            {
                if (owner.hurtCooldowns[i] < time)
                {
                    success = true;
                    owner.hurtCooldowns[i] = time;
                }
            }
            if (success)
            {
                owner.immune = true;
                if (owner.immuneTime < time) owner.immuneTime = time;
                owner.immuneNoBlink = true;
            }
        }
    }
}