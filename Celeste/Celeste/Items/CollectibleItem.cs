using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Items
{
    public sealed class CollectibleItem : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable
    {
        private readonly ItemAnimation _animation;

        public CollectibleItem(ItemAnimation animation)
        {
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
        }

        public Vector2 Position
        {
            get => _animation.Position;
            set => _animation.Position = value;
        }

        public float Scale
        {
            get => _animation.Scale;
            set => _animation.Scale = value;
        }

        public bool Collected { get; private set; }

        public Rectangle Bounds
        {
            get
            {
                if (Collected)
                {
                    return Rectangle.Empty;
                }

                int width = (int)(_animation.Clip.FrameWidth * Scale);
                int height = (int)(_animation.Clip.FrameHeight * Scale);
                return new Rectangle((int)Position.X, (int)Position.Y, width, height);
            }
        }

        public void Reset()
        {
            Collected = false;
            _animation.Reset();
        }

        public bool TryCollect(Rectangle playerBounds)
        {
            if (Collected || !Bounds.Intersects(playerBounds))
            {
                return false;
            }

            Collected = true;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Collected)
            {
                _animation.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Collected)
            {
                _animation.Draw(spriteBatch);
            }
        }
    }
}
