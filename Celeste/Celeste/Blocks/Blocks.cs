using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public class Blocks : IBlocks
    {
        public Vector2 Position { get; set; }
        public string Type { get; }

        public Texture2D Texture { get; set; }

        // Constructor
        public Blocks(string type, Vector2 position, Texture2D texture = null)
        {
            Type = type;
            Position = position;
            Texture = texture;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(
                    Texture, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }

    }
}