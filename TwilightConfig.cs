using System.ComponentModel;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Twilight
{
    public class TwilightConfig : ModConfig
    {
        public override bool Autoload(ref string name)
        {
            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                name = "配置";
            }
            else
            {
                name = "Config";
            }
            return true;
        }
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(true)]
        [Label("$Mods.Twilight.UseUI")]
        public bool UseUI;

        


        public override ModConfig Clone()
        {
            var clone = (TwilightConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            Twilight.config = this;
            ModTranslation modTranslation = Twilight.Instance.CreateTranslation("UseUI");
            modTranslation.SetDefault("Use the screen effects");
            modTranslation.AddTranslation(GameCulture.Chinese, "使用屏幕特效");
            Twilight.Instance.AddTranslation(modTranslation);
        }


        public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string messageline)
        {
            string message = "";
            string messagech = "";

            if (Language.ActiveCulture == GameCulture.Chinese)
            {
                messageline = messagech;
            }
            else
            {
                messageline = message;
            }

            if (whoAmI == 0)
            {
                //message = "Changes accepted!";
                //messagech = "设置改动成功!";
                return true;
            }
            if (whoAmI != 0)
            {
                //message = "You have no rights to change config.";
                //messagech = "你没有设置改动权限.";
                return false;
            }
            return false;
        }
    }
}