using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Represents the hair portion of a character sprite.
    ///
    /// In the original Celeste, the character is drawn without hair and the
    /// hair is added in real time as a chain of trailing circles anchored to
    /// the head. This interface abstracts that rendering so it can be developed
    /// and tested independently of the body sprite.
    /// </summary>
    public interface IHairSprite
    {
        /// <summary>
        /// The current color of the hair (changes during dash, etc.).
        /// </summary>
        Color HairColor { get; set; }

        /// <summary>
        /// Number of nodes in the hair chain.
        /// </summary>
        int NodeCount { get; }

        /// <summary>
        /// Updates the hair physics simulation.
        /// </summary>
        /// <param name="gameTime">Provides elapsed time since last update.</param>
        /// <param name="anchorPosition">
        /// World-space position where the first hair node attaches to the head.
        /// This is typically derived from the body sprite's current frame and
        /// per-animation hair offset metadata.
        /// </param>
        /// <param name="faceLeft">Current facing direction of the character.</param>
        void Update(GameTime gameTime, Vector2 anchorPosition, bool faceLeft);

        /// <summary>
        /// Draws the hair chain (circles + bangs overlay).
        /// </summary>
        /// <param name="spriteBatch">Active SpriteBatch to draw with.</param>
        /// <param name="color">Tint color (usually <see cref="HairColor"/>).</param>
        /// <param name="scale">Uniform scale factor, should match the body scale.</param>
        void Draw(SpriteBatch spriteBatch, Color color, float scale = 1f);
    }
}
