using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.CollectableItems;

namespace Celeste.Blocks
{
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

        private ItemAnimation _animation;

        public Spring(Vector2 position, AnimationCatalog catalog)
        {
            AnimationClip clip = catalog.Clips[AnimationKeys.DevicesSpring];
            _animation = new ItemAnimation(clip);
            _animation.Position = position;

        }

        public void Update(GameTime gameTime)
        {
            _animation.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animation.Draw(spriteBatch);
        }
    }
}