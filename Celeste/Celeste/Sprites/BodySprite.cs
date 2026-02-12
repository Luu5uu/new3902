using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Wraps an <see cref="AnimationController{TState}"/> to satisfy
    /// <see cref="IBodySprite"/>. This keeps the existing animation
    /// infrastructure intact while fitting into the new composite pattern.
    /// </summary>
    /// <typeparam name="TState">Enum that identifies each animation state.</typeparam>
    public sealed class BodySprite<TState> : IBodySprite where TState : notnull
    {
        private readonly AnimationController<TState> _controller;

        public int FrameWidth { get; }
        public int FrameHeight { get; }

        public int CurrentFrame
        {
            get
            {
                var anim = _controller.Get(_controller.CurrentState);
                return anim.CurrentFrame;
            }
        }

        public BodySprite(AnimationController<TState> controller,
                          int frameWidth = 32, int frameHeight = 32)
        {
            _controller = controller;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        /// <summary>
        /// Exposes the inner controller so the owning class can call
        /// <c>SetState</c> when the player's gameplay state changes.
        /// </summary>
        public AnimationController<TState> Controller => _controller;

        public void Update(GameTime gameTime)
            => _controller.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                         float scale = 1f, bool faceLeft = false)
        {
            var effects = faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _controller.Draw(spriteBatch, position, color, scale, effects);
        }
    }
}
