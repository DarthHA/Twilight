using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
namespace Twilight.Buffs
{
    public class TwilightParadisedBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("Paradised");
            DisplayName.AddTranslation(GameCulture.Chinese, "麻痹");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = false;
            Main.buffNoTimeDisplay[Type] = false;
            canBeCleared = false;
            longerExpertDebuff = false;
        }

    }

    
}