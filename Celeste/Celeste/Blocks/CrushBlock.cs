using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using Celeste.Items;

namespace Celeste.Blocks
{
    public class CrushBlock : IBlocks
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

        public string Type => "crushBlock";
        public float Scale { get; set; } = 2.0f;

        private readonly ItemAnimation _animation;

        public CrushBlock(Vector2 position, AnimationCatalog catalog, float scale = 2.5f)
        {
            var clip = catalog.Clips[AnimationKeys.DevicesCrushBlock];
            _animation = new ItemAnimation(clip);
            _animation.Position = position;
            Scale = scale;
        }

        public void Update(GameTime gameTime) => _animation.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => _animation.Draw(spriteBatch, Position, Scale);

        public Rectangle Bounds
        {
            get
            {
                var tex = Texture;
                if (tex == null) return Rectangle.Empty;

                int w = (int)(tex.Width * Scale);
                int h = (int)(tex.Height * Scale);

                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    w,
                    h
                );
            }
        }
    }
}
