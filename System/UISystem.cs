

using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Twilight.UI;

namespace Twilight
{

    public class UISystem : ModSystem
    {
        private UserInterface _DrawingUIInterface;
        internal DrawingUI _DrawingUI;

        public override void Load()
        {
            _DrawingUI = new DrawingUI();
            _DrawingUI.Activate();
            _DrawingUIInterface = new UserInterface();
            _DrawingUIInterface.SetState(_DrawingUI);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {

            int DrawingUIIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Interface Logic 4"));
            if (DrawingUIIndex != -1)
            {
                layers.Insert(DrawingUIIndex, new LegacyGameInterfaceLayer(
                    "Twilight: DrawingUI",
                    delegate
                    {
                        _DrawingUIInterface.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }
    }

}