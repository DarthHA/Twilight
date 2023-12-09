using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace Twilight
{
    public class SkyRenderHandler : ModSystem
    {
        public static RenderTarget2D skyRender;

        //internal static Queue<Action> DrawActionQueue = new();
        public override void Load()
        {
            Main.QueueMainThreadAction(() =>
            {
                skyRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            });
        }
        public override void Unload()
        {
            Main.QueueMainThreadAction(() =>
            {
                skyRender.Dispose();
            });
        }

        public override void OnModLoad()
        {
            On_Main.UpdateDisplaySettings += On_Main_UpdateDisplaySettings;
        }
        public override void OnModUnload()
        {
            On_Main.UpdateDisplaySettings -= On_Main_UpdateDisplaySettings;
        }
        private void On_Main_UpdateDisplaySettings(On_Main.orig_UpdateDisplaySettings orig, Main self)
        {
            orig.Invoke(self);
            Main.QueueMainThreadAction(() =>
            {
                if (skyRender.Width != Main.screenWidth || skyRender.Height != Main.screenHeight)
                {
                    skyRender = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
                }
            });
        }


    }
}
