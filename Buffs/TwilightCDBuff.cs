using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class TwilightCDBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Apocalypse Bird Synchronize Cooldown");
            DisplayName.AddTranslation(GameCulture.Chinese, "终末鸟同化冷却中");
            Description.SetDefault("You cannot synchronize with Apocalypse Bird temporarily...");
            Description.AddTranslation(GameCulture.Chinese, "你暂时不能与终末鸟同化...");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = time;
            return true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (ApoBirdSky.CurrentState != 0)
            {
                if (player.buffTime[buffIndex] > 1)
                {
                    player.buffTime[buffIndex] = 1;
                }
            }
        }
    }


}