using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public interface IBlocks
    {
        Vector2 Position { get; set; }
        Texture2D Texture { get; set; }
        string Type { get; }
        void Draw(SpriteBatch spriteBatch);

    }
}