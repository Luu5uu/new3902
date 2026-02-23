using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static Celeste.PlayerConstants;

namespace Celeste.Sprites
{
    // Wraps AnimationController to satisfy IBodySprite.
    public sealed class BodySprite<TState> : IBodySprite where TState : notnull
    {
        private readonly AnimationController<TState> _controller;

        public int FrameWidth { get; }
        public int FrameHeight { get; }
        public int CurrentFrame => _controller.Get(_controller.CurrentState).CurrentFrame;
        public AnimationController<TState> Controller => _controller;

        public BodySprite(AnimationController<TState> controller,
                          int frameWidth = PlayerBodyFrameWidth, int frameHeight = PlayerBodyFrameHeight)
        {
            _controller = controller;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
        }

        public void Update(GameTime gameTime) => _controller.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                         float scale = 1f, bool faceLeft = false)
        {
            // Negative X scale for flipping 
            float scaleX = faceLeft ? -scale : scale;
            _controller.Draw(spriteBatch, position, color, new Vector2(scaleX, scale));
        }
    }
}
