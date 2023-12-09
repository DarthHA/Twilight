using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class BigEyeBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Orange;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = time;
            return true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (ApoBirdSky.CurrentState != ApoBirdSky.State.ApoSky)
            {
                if (player.buffTime[buffIndex] > 1)
                {
                    player.buffTime[buffIndex] = 1;
                }
            }
        }
    }


}