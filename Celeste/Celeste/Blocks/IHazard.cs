using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Celeste.Blocks
{
    public interface IHazard
    {
        Vector2 Position { get; set; }
        Texture2D Texture { get; set; }
        string Type { get; }
        float Scale { get; set; }
        Rectangle Bounds { get; }
        void Draw(SpriteBatch spriteBatch);
    }
}
