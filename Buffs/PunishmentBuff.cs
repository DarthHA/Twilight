using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class PunishmentBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }
        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            rare = ItemRarityID.Red;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (ApoBirdSky.CurrentState != ApoBirdSky.State.ApoSky || player.GetModPlayer<TwilightPlayer>().CurrentEgg != 2)
            {
                if (player.buffTime[buffIndex] > 1)
                {
                    player.buffTime[buffIndex] = 1;
                }
            }
        }
    }


}