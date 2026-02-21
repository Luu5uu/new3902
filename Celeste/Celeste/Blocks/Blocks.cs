using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public class Blocks : IBlocks
    {
        public Vector2 Position { get; set; }
        public string Type { get; }

        public Texture2D Texture { get; set; }
        public float Scale { get; set; } = 2.0f;

        // Constructor
        public Blocks(string type, Vector2 position, Texture2D texture = null, float scale = 2.5f)
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
                    Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
        }

    }
}