using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace Twilight.Buffs
{
    public class TwilightBleedingBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.buffTime[buffIndex] = time;
            return true;
        }
    }


}