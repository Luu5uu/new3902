using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Composite sprite interface: body (hairless) + procedural hair.
    // Gameplay code depends only on this interface.
    public interface IMaddySprite
    {
        IBodySprite Body { get; }
        IHairSprite Hair { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                  float scale = 1f, bool faceLeft = false);
    }
}
