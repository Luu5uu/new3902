using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.UI
{
    public class ClimbStaminaBar
    {
        private readonly Texture2D _pixel;
        private readonly int _width;
        private readonly int _height;

        public ClimbStaminaBar(Texture2D pixel, int width = 6, int height = 40)
        {
            _pixel = pixel;
            _width = width;
            _height = height;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float staminaPercent, float tiredPercent, bool isTired)
        {
            staminaPercent = MathHelper.Clamp(staminaPercent, 0f, 1f);
            tiredPercent = MathHelper.Clamp(tiredPercent, 0f, 1f);

            Rectangle background = new Rectangle((int)position.X, (int)position.Y, _width, _height);

            int fillHeight = (int)(_height * staminaPercent);
            Rectangle fill = new Rectangle(
                (int)position.X,
                (int)position.Y + (_height - fillHeight),
                _width,
                fillHeight
            );

            spriteBatch.Draw(_pixel, background, Color.Black * 0.6f);
            spriteBatch.Draw(_pixel, fill, isTired ? Color.OrangeRed : Color.LimeGreen);

            int markerY = (int)position.Y + (_height - (int)(_height * tiredPercent));
            Rectangle marker = new Rectangle((int)position.X - 2, markerY, _width + 4, 2);
            spriteBatch.Draw(_pixel, marker, Color.White);
        }
    }
}