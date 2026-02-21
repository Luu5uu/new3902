using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.CollectableItems;

namespace Celeste.Blocks
{
    /// <summary>
    /// For the animated spring block building off of ItemAnimation
    /// </summary>
    public class Spring : IBlocks
    {
        /// <summary>
        ///  positioning of the spring block
        /// </summary>
        public Vector2 Position
        {
            get => _animation.Position;
            set => _animation.Position = value;
        }
        /// <summary>
        /// get  springtexture
        /// </summary>
        public Texture2D Texture
        {
            get => _animation.Clip?.Texture;
            set { }
        }
        /// <summary>
        /// for ID
        /// </summary>
        public string Type => "spring";
        public float Scale { get; set; } = 2.0f;

        private ItemAnimation _animation;

        public Spring(Vector2 position, AnimationCatalog catalog, float scale = 2.5f)
        {
            // spring animation clip
            AnimationClip clip = catalog.Clips[AnimationKeys.DevicesSpring];
            _animation = new ItemAnimation(clip);
            _animation.Position = position;
            Scale = scale;

        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch, Position, Scale);
        }
    }
}