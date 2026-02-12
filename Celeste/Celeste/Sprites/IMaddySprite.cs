using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Composite sprite for Madeline that combines a body sprite and a hair sprite.
    ///
    /// This follows the professor's recommended decomposition:
    ///   IMaddySprite
    ///       IBodySprite   – draws the hairless character body
    ///       IHairSprite   – draws the procedural hair chain
    ///
    /// Gameplay code should depend only on this interface. The body and hair
    /// are updated and drawn together, but can be developed independently.
    /// </summary>
    public interface IMaddySprite
    {
        /// <summary>
        /// The body sub-sprite for this character.
        /// </summary>
        IBodySprite Body { get; }

        /// <summary>
        /// The hair sub-sprite for this character.
        /// </summary>
        IHairSprite Hair { get; }

        /// <summary>
        /// Updates both the body animation and hair physics.
        /// Must be called once per frame.
        /// </summary>
        void Update(GameTime gameTime);

        /// <summary>
        /// Draws the complete character (body + hair) at the given position.
        /// </summary>
        /// <param name="spriteBatch">Active SpriteBatch to draw with.</param>
        /// <param name="position">World position to draw at.</param>
        /// <param name="color">Tint color for the body.</param>
        /// <param name="scale">Uniform scale factor.</param>
        /// <param name="faceLeft">If true, character faces left.</param>
        void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                  float scale = 1f, bool faceLeft = false);
    }
}
