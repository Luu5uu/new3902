using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Hairless body animation. Hair is rendered separately by IHairSprite.
    public interface IBodySprite
    {
        int FrameWidth { get; }
        int FrameHeight { get; }
        int CurrentFrame { get; }
        void Update(GameTime gameTime);
        void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                  float scale = 1f, bool faceLeft = false);
    }
}
