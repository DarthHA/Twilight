using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class BigEyeBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Big Eye");
            DisplayName.AddTranslation(GameCulture.Chinese, "巨目");
            Description.SetDefault("\"The Big Bird’s eyes imprisoned light.\"\n" +
                "Damage taken is reduced by 25%.\n" +
                "Crit rate is increased when you hit an enchanted enemy.");
            Description.AddTranslation(GameCulture.Chinese, "“巨目遮蔽着光源...”\n" +
                "受到的伤害降低25%.\n" +
                "当击中被迷惑的敌人时暴击率会提升。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
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