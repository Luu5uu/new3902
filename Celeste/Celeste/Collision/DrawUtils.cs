using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Utils
{
    public static class DrawUtils
    {
        public static void DrawRectangleOutline(
            SpriteBatch spriteBatch,
            Texture2D pixel,
            Rectangle rect,
            Color color,
            int thickness = 2)
        {
            // top
            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                color);

            // bottom
            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness),
                color);

            // left
            spriteBatch.Draw(pixel,
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                color);

            // right
            spriteBatch.Draw(pixel,
                new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                color);
        }
    }
}