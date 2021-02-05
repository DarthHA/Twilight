using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Twilight.Buffs
{
    public class SinBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Mark of Sin");
            DisplayName.AddTranslation(GameCulture.Chinese, "罪痕");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            longerExpertDebuff = false;
            canBeCleared = false;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            if (npc.GetGlobalNPC<ApoBirdGNPC>().Sins < 5)
            {
                npc.GetGlobalNPC<ApoBirdGNPC>().Sins++;
            }
            return false;
        }

    }
}