using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Twilight.Buffs;
using Twilight.Projectiles;

namespace Twilight.UI
{
    internal class DrawingUI : UIState
    {
        // For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
        // Once this is all set up make sure to go and do the required stuff for most UI's in the Mod class.
        private UIElement area;
        private float DarkIntensity = 0;
        public override void OnInitialize()
        {
            // Create a UIElement for all the elements to sit on top of, this simplifies the numbers as nested elements can be positioned relative to the top left corner of this element. 
            // UIElement is invisible and has no padding. You can use a UIPanel if you wish for a background.
            area = new UIElement();
            area.Left.Set(20, 1f); // Place the resource bar to the left of the hearts.
            area.Top.Set(30, 0f); // Placing it just a bit below the top of the screen.
            area.Width.Set(20, 0f); // We will be placing the following 2 UIElements within this 182x60 area.
            area.Height.Set(20, 0f);

            Append(area);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            if (Twilight.config.UseUI)
            {
                TwilightUIPlayer modplayer = Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>();
                if (modplayer.Timer > 0)
                {
                    Texture2D tex;
                    switch (modplayer.CurrentPlay)
                    {
                        case 0:
                            tex = Twilight.Instance.GetTexture("UI/BigBirdArrived");
                            break;
                        case 1:
                            tex = Twilight.Instance.GetTexture("UI/LongBirdArrived");
                            break;
                        case 2:
                            tex = Twilight.Instance.GetTexture("UI/SmallBirdArrived");
                            break;
                        default:
                            tex = Twilight.Instance.GetTexture("UI/BigBirdArrived");
                            break;
                    }
                    float alpha;
                    if (modplayer.Timer > 40)
                    {
                        alpha = (float)(50f - modplayer.Timer) / 10f;
                    }
                    else
                    {
                        alpha = (float)modplayer.Timer / 40;
                    }
                    alpha /= 2;
                    spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * alpha);
                }

                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()))
                {
                    DarkIntensity += 0.025f;
                    if (DarkIntensity > 1)
                    {
                        DarkIntensity = 1;
                    }
                }
                else
                {
                    DarkIntensity -= 0.025f;
                    if (DarkIntensity < 0)
                    {
                        DarkIntensity = 0;
                    }
                }
                if (DarkIntensity > 0)
                {
                    spriteBatch.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.4f * DarkIntensity);
                }
                if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()))
                {
                    EyeLantern.DrawLanterns();
                    EyeLantern2.DrawLanterns();
                }
            }
        }



    }
}
