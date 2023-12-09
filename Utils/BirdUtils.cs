

using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Twilight.NPCs;

namespace Twilight
{
    public static class BirdUtils
    {
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

        public static bool IsNotBird(NPC npc)
        {
            return npc.type != ModContent.NPCType<ApoBirdBody>() && npc.type != ModContent.NPCType<ApoBirdClaw>() && npc.type != ModContent.NPCType<ApoBirdHead>();
        }

        public static float Rotating(float rot, float target, float factor)
        {
            Vector2 OriginalVec = rot.ToRotationVector2();
            Vector2 TargetVec = target.ToRotationVector2();
            Vector2 ResultVec = OriginalVec * (1 - factor) + TargetVec * factor;
            return ResultVec.ToRotation();
        }
    }
}