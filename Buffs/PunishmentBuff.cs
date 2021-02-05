using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class PunishmentBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Punishment!");
            DisplayName.AddTranslation(GameCulture.Chinese, "惩戒！");
            Description.SetDefault("Your next attack will deal 2x damage.");
            Description.AddTranslation(GameCulture.Chinese, "你的下次攻击会造成两倍伤害。");
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Red;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            if (ApoBirdSky.CurrentState != 2)
            {
                if (player.buffTime[buffIndex] > 1)
                {
                    player.buffTime[buffIndex] = 1;
                }
            }
        }
    }


}