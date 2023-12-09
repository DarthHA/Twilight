using Terraria;

namespace Twilight
{
    public static class SomeUtils
    {

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

        public static void ApplySin(NPC target)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            modNPC.Sins++;
            if (modNPC.Sins > 5)
            {
                modNPC.Sins = 5;
            }
        }

        public static int SinCount(NPC target)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            return modNPC.Sins;
        }


        private static void ApplyStun(NPC target, int time)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            if (modNPC.StunBuffTime < time)
            {
                modNPC.StunBuffTime = time;
            }
        }

        private static void ApplyParadise(NPC target, int time)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            if (modNPC.ParadiseBuffTime < time)
            {
                modNPC.ParadiseBuffTime = time;
            }
        }

        public static void DeepApplyStun(NPC target, int buffTime)
        {
            if (target.realLife == -1)
            {
                ApplyStun(target, buffTime);
            }
            if (target.realLife >= 0)
            {
                if (Main.npc[target.realLife].active)
                {
                    ApplyStun(Main.npc[target.realLife], buffTime);
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active)
                        {
                            if (npc.active && npc.realLife == target.realLife)
                            {
                                if (npc.whoAmI != target.whoAmI && npc.whoAmI != target.realLife)
                                {
                                    ApplyStun(npc, buffTime);
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void DeepApplyParadise(NPC target, int buffTime)
        {
            if (target.realLife == -1)
            {
                ApplyParadise(target, buffTime);
            }
            if (target.realLife >= 0)
            {
                if (Main.npc[target.realLife].active)
                {
                    ApplyParadise(Main.npc[target.realLife], buffTime);
                    foreach (NPC npc in Main.npc)
                    {
                        if (npc.active)
                        {
                            if (npc.active && npc.realLife == target.realLife)
                            {
                                if (npc.whoAmI != target.whoAmI && npc.whoAmI != target.realLife)
                                {
                                    ApplyParadise(npc, buffTime);
                                }
                            }
                        }
                    }
                }
            }
        }


        public static bool HasStun(NPC target)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            if (modNPC.StunBuffTime > 0)
            {
                return true;
            }
            return false;
        }

        public static bool HasParadise(NPC target)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            if (modNPC.ParadiseBuffTime > 0)
            {
                return true;
            }
            return false;
        }

        public static void ApplyBleeding(NPC target, int time)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            modNPC.Bleed++;
            if (modNPC.BleedingBuffTime < time)
            {
                modNPC.BleedingBuffTime = time;
            }
        }

        public static bool HasBleeding(NPC target)
        {
            ApoBirdGNPC modNPC = target.GetGlobalNPC<ApoBirdGNPC>();
            if (modNPC.BleedingBuffTime > 0)
            {
                return true;
            }
            return false;
        }


    }
}