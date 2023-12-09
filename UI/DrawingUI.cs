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
        private float DarkIntensity = 0;
        public override void OnInitialize()
        {
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            TwilightUIPlayer modplayer = Main.LocalPlayer.GetModPlayer<TwilightUIPlayer>();
            if (modplayer.Timer > 0)
            {
                Texture2D tex;
                switch (modplayer.CurrentPlay)
                {
                    case 0:
                        tex = ModContent.Request<Texture2D>("Twilight/UI/BigBirdArrived").Value;
                        break;
                    case 1:
                        tex = ModContent.Request<Texture2D>("Twilight/UI/LongBirdArrived").Value;
                        break;
                    case 2:
                        tex = ModContent.Request<Texture2D>("Twilight/UI/SmallBirdArrived").Value;
                        break;
                    default:
                        tex = ModContent.Request<Texture2D>("Twilight/UI/BossBirdAppear").Value;
                        break;
                }
                float alpha;
                if (modplayer.CurrentPlay != 3)
                {
                    if (modplayer.Timer > 40)
                    {
                        alpha = (float)(50f - modplayer.Timer) / 10f;
                    }
                    else
                    {
                        alpha = (float)modplayer.Timer / 40;
                    }
                    alpha /= 2;
                }
                else
                {
                    if (modplayer.Timer > 55)
                    {
                        alpha = (float)(70f - modplayer.Timer) / 15f;
                    }
                    else if (modplayer.Timer > 40)
                    {
                        alpha = 1;
                    }
                    else
                    {
                        alpha = (float)modplayer.Timer / 40;
                    }
                }
                if (Twilight.config.UseScreenEffect)
                {
                    spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * alpha);
                }
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
            if (DarkIntensity > 0 && Twilight.config.UseScreenEffect)
            {
                spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.4f * DarkIntensity);
            }
            if (Main.LocalPlayer.HasBuff(ModContent.BuffType<BigEyeBuff>()) && Twilight.config.UseScreenEffect)
            {
                EyeLantern.DrawLanterns();
                EyeLantern2.DrawLanterns();
            }
        }
    }
}
