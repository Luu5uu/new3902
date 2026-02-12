using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Represents the body (hairless) portion of a character sprite.
    /// Handles frame-based animation for the character body only.
    /// Hair is rendered separately by <see cref="IHairSprite"/>.
    /// </summary>
    public interface IBodySprite
    {
        /// <summary>
        /// Width of a single animation frame in pixels.
        /// </summary>
        int FrameWidth { get; }

        /// <summary>
        /// Height of a single animation frame in pixels.
        /// </summary>
        int FrameHeight { get; }

        /// <summary>
        /// Index of the currently displayed frame.
        /// </summary>
        int CurrentFrame { get; }

        /// <summary>
        /// Advances the animation based on elapsed game time.
        /// </summary>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws the current body frame at the specified position.
        /// </summary>
        /// <param name="spriteBatch">Active SpriteBatch to draw with.</param>
        /// <param name="position">World position to draw at.</param>
        /// <param name="color">Tint color.</param>
        /// <param name="scale">Uniform scale factor.</param>
        /// <param name="faceLeft">If true, sprite is flipped horizontally.</param>
        void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                  float scale = 1f, bool faceLeft = false);
    }
}
