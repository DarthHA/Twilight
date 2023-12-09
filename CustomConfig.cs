using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Twilight
{
    //[Label($"$Mods.Twilight.Config.Label")]
    public class CustomConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        //[Label($"$Mods.Twilight.Config.UseScreenEffect")]
        //[Tooltip($"$Mods.Twilight.Config.UseScreenEffect2")]
        [DefaultValue(true)]
        public bool UseScreenEffect;

        //[Label($"$Mods.Twilight.Config.UseBGM")]
        [DefaultValue(true)]
        public bool UseBGM;



        public override ModConfig Clone()
        {
            var clone = (CustomConfig)base.Clone();
            return clone;
        }

        public override void OnLoaded()
        {
            Twilight.config = this;
        }


    }
}