using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.Items;

namespace Celeste.Blocks
{
    /// <summary>
    /// Animated spring block using ItemAnimation.
    /// </summary>
    public class Spring : IBlocks
    {
        public Vector2 Position
        {
            get => _animation.Position;
            set => _animation.Position = value;
        }
        public Texture2D Texture
        {
            get => _animation.Clip?.Texture;
            set { }
        }
        public string Type => "spring";
        public float Scale { get; set; } = 2.0f;

        private readonly ItemAnimation _animation;

        public Spring(Vector2 position, AnimationCatalog catalog, float scale = 2.5f)
        {
            var clip = catalog.Clips[AnimationKeys.DevicesSpring];
            _animation = new ItemAnimation(clip);
            _animation.Position = position;
            Scale = scale;
        }

        public void Update(GameTime gameTime) => _animation.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => _animation.Draw(spriteBatch, Position, Scale);
    }
}
