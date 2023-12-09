using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight
{
    public class Twilight : Mod
    {
        public static Twilight Instance;

        public static CustomConfig config;

        public static Effect CurtainEffect;
        public static Effect ShotTrailEffect;
        public static Effect NormalTrailEffect;

        public Twilight()
        {
            Instance = this;
        }
        public override void Load()
        {
            SkyManager.Instance["Twilight:ApoBirdSky"] = new ApoBirdSky();

            CurtainEffect = ModContent.Request<Effect>("Twilight/Effects/Content/CurtainEffect", AssetRequestMode.ImmediateLoad).Value;
            NormalTrailEffect = ModContent.Request<Effect>("Twilight/Effects/Content/NormalTrailEffect", AssetRequestMode.ImmediateLoad).Value;
            ShotTrailEffect = ModContent.Request<Effect>("Twilight/Effects/Content/ShotTrailEffect", AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Unload()
        {
            SkyManager.Instance["Twilight:ApoBirdSky"].Deactivate();
            CurtainEffect = null;
            NormalTrailEffect = null;
            ShotTrailEffect = null;

            config = null;
            Instance = null;
        }
    }



}