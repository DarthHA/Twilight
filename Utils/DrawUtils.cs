
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;

namespace Twilight
{
    public static class DrawUtils
    {
        public static Vector2 GetZoomScreenPos()
        {
            Vector2 screenCenter = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
            Vector2 screenPos = screenCenter - screenSize / 2f;
            return screenPos;
        }

        public static Vector2 GetZoomScreenSize()
        {
            Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
            return screenSize;
        }
        public static void DrawTrail(Texture2D tex, List<CustomVertexInfo> bars, SpriteBatch spriteBatch, Color color, BlendState blendState)
        {
            List<CustomVertexInfo> triangleList = new();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                Vector2 vector = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, screenSize.X, screenSize.Y, 0f, 0f, 1f);
                Vector2 screenPos = vector - screenSize / 2f;
                Matrix model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0f));
                Twilight.NormalTrailEffect.Parameters["uTransform"].SetValue(model * projection);
                Twilight.NormalTrailEffect.Parameters["color"].SetValue(color.ToVector4());
                Main.graphics.GraphicsDevice.Textures[0] = tex;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Twilight.NormalTrailEffect.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangleList.ToArray(), 0, triangleList.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }


        public static void DrawShotTrail(Texture2D tex1, Texture2D tex2, List<CustomVertexInfo> bars, SpriteBatch spriteBatch, Color color, BlendState blendState, float progress)
        {
            List<CustomVertexInfo> triangleList = new();
            if (bars.Count > 2)
            {
                for (int k = 0; k < bars.Count - 2; k += 2)
                {
                    triangleList.Add(bars[k]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 1]);
                    triangleList.Add(bars[k + 2]);
                    triangleList.Add(bars[k + 3]);
                }
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                RasterizerState originalState = Main.graphics.GraphicsDevice.RasterizerState;
                Vector2 vector = Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
                Vector2 screenSize = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;
                Matrix projection = Matrix.CreateOrthographicOffCenter(0f, screenSize.X, screenSize.Y, 0f, 0f, 1f);
                Vector2 screenPos = vector - screenSize / 2f;
                Matrix model = Matrix.CreateTranslation(new Vector3(-screenPos.X, -screenPos.Y, 0f));
                Twilight.ShotTrailEffect.Parameters["uTransform"].SetValue(model * projection);
                Twilight.ShotTrailEffect.Parameters["color"].SetValue(color.ToVector4());
                Twilight.ShotTrailEffect.Parameters["progress"].SetValue(progress);
                Main.graphics.GraphicsDevice.Textures[0] = tex1;
                Main.graphics.GraphicsDevice.Textures[1] = tex2;
                Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
                Twilight.ShotTrailEffect.CurrentTechnique.Passes[0].Apply();
                Main.graphics.GraphicsDevice.DrawUserPrimitives(0, triangleList.ToArray(), 0, triangleList.Count / 3);
                Main.graphics.GraphicsDevice.RasterizerState = originalState;
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
    }
}
