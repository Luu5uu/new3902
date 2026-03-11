using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public class Blocks : IBlocks
    {
        public Vector2 Position { get; set; }
        public string Type { get; }

        public Texture2D Texture { get; set; }
        // get the individual block from the sheet
        public Rectangle? SourceRect { get; set; }
        public float Scale { get; set; } = 2.0f;

        // get the correct size of the block
        public Vector2 Size => SourceRect.HasValue
            ? new Vector2(SourceRect.Value.Width * Scale, SourceRect.Value.Height * Scale) : (Texture != null ? new Vector2(Texture.Width * Scale, Texture.Height * Scale) : Vector2.Zero);

        public Blocks(string type, Vector2 position, Texture2D texture = null, int? frameIndex = null, float scale = 2.5f)
        {
            Type = type;
            Position = position;
            Texture = texture;
            Scale = scale;

            if (frameIndex.HasValue && Texture != null)
            {
                int blocksPerRow = texture.Width / 8;
                int x = (frameIndex.Value % blocksPerRow);
                int y = (frameIndex.Value / blocksPerRow);
                SourceRect = new Rectangle(x * 8, y * 8, 8, 8);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(
                    Texture, Position, SourceRect, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
        }
    }
}
