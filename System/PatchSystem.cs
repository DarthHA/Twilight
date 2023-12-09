using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using Twilight.Sky;

namespace Twilight
{
    public class PatchSystem : ModSystem
    {
        public override void Load()
        {
            Terraria.On_Projectile.Kill += ProjKillHook;
            //On.Terraria.Graphics.Effects.SkyManager.DrawDepthRange += new On.Terraria.Graphics.Effects.SkyManager.hook_DrawDepthRange(DrawDepthRangeHook);
        }

        public override void Unload()
        {
            Terraria.On_Projectile.Kill -= ProjKillHook;
        }

        public static void ProjKillHook(On_Projectile.orig_Kill orig, Projectile self)
        {
            if (!self.GetGlobalProjectile<TwilightGProj>().CanKill)
            {
                self.penetrate = -1;
                self.timeLeft = 99999;
            }
            else
            {
                orig.Invoke(self);
            }
        }


        public static void DrawDepthRangeHook(On_SkyManager.orig_DrawDepthRange orig, SkyManager self, SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            if (ApoBirdSky.CurrentState == ApoBirdSky.State.NoSky)
            {
                orig.Invoke(self, spriteBatch, minDepth, maxDepth);
            }
            else
            {
                FieldInfo fieldInfo = SkyManager.Instance.GetType().GetField("_activeSkies", BindingFlags.NonPublic | BindingFlags.Instance);
                LinkedList<CustomSky> _activeSkies = (LinkedList<CustomSky>)fieldInfo.GetValue(SkyManager.Instance);
                foreach (CustomSky customSky in _activeSkies)
                {
                    if (customSky is not ApoBirdSky)
                    {
                        customSky.Draw(spriteBatch, minDepth, maxDepth);
                    }
                }
                SkyManager.Instance["Twilight:ApoBirdSky"].Draw(spriteBatch, minDepth, maxDepth);
            }
        }

    }
}