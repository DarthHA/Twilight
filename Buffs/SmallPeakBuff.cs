using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class SmallPeakBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Small Peak");
            DisplayName.AddTranslation(GameCulture.Chinese, "小喙");
            Description.SetDefault("\"and the Small Bird’s beak whispered, endlessly…\"\n" +
                "Taking damage will cause the next attack deal double damage.\n" +
                "During the Torn Mouth Attack, all enemies will slow down.");
            Description.AddTranslation(GameCulture.Chinese, "“小喙窃窃私语，喋喋不休......”\n" +
                "受到伤害后，下次攻击将会造成双倍伤害。\n" +
                "发动巨口攻击时所有敌人移动速度降低。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Red;
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