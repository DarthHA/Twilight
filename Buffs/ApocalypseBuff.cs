using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight.Buffs
{
    public class ApocalypseBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Apocalypse Bird");
            DisplayName.AddTranslation(GameCulture.Chinese, "终末鸟");
            Description.SetDefault("\"The day when Big Bird’s eyes that could see hundreds of kilometers away,\n" +
                " Long Bird who could judge any sin,\n" +
                " and Small Bird’s mouth that could devour everything united into one,\n" +
                " darkness fell upon the forest.\"\n" +
                "Immune to most non-persistent debuffs\n" +
                "Your attack will heal yourself\n" +
                "The Apocalypse Bird will stand by your side.");
            Description.AddTranslation(GameCulture.Chinese,
                "“大鸟那能够看清数百里外的眼睛，\n" +
                "高鸟那能够审判任何罪孽的天秤，\n" +
                "小鸟那能够吞噬一切生物的巨口，\n" +
                "在它们合为一体的那天，灾难降临了这片森林。”\n" +
                "免疫大多数非持续性debuff\n" +
                "你的攻击会治疗自己\n" +
                "天启鸟将会为你而战。");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
            canBeCleared = false;
            longerExpertDebuff = false;
        }
        public override void ModifyBuffTip(ref string tip, ref int rare)
        {
            rare = ItemRarityID.Expert;
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