using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class ApocalypseBuff : ModBuff
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
            rare = ItemRarityID.Master;
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