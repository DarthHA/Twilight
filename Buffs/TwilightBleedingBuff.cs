using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace Twilight.Buffs
{
    public class TwilightBleedingBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Bleeding");
            DisplayName.AddTranslation(GameCulture.Chinese, "流血");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.GetGlobalNPC<ApoBirdGNPC>().Bleed++;
            return false;
        }
    }


}