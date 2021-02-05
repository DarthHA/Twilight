using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class LongArmBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Long Arm");
            DisplayName.AddTranslation(GameCulture.Chinese, "长臂");
            Description.SetDefault("\"The Long Bird’s arms concealed time.\"\n" +
                "When hitting the enemy with the most health percent,all the enemies will take 15% of the damage.\n" +
                "All of your attacks have a chance to inflict Mark of Sin, which will increase the effect of all attacks.");
            Description.AddTranslation(GameCulture.Chinese, "“长臂掩藏了时间......”\n" +
                "当击中生命百分比最高的敌人时，会对所有敌人造成15%的伤害。\n" +
                "你的所有攻击都有几率施加罪痕debuff，该debuff会增加所有攻击的效果。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Cyan;
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