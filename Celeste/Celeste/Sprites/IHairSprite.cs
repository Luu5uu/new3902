using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Procedural hair chain (trailing circles + bangs overlay).
    public interface IHairSprite
    {
        Color HairColor { get; set; }
        int NodeCount { get; }
        void Update(GameTime gameTime, Vector2 anchorPosition, bool faceLeft);
        void Draw(SpriteBatch spriteBatch, Color color, float scale = 1f);
    }
}
