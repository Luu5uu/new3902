using System;
using Celeste.CollectText;
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Items
{
    public sealed class CollectibleItem : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable
    {

        public enum ItemType
        {
            Strawberry,
            Crystal
        }

        private static int _strawberryCount = 0;
        public static int StrawberryCount => _strawberryCount;

        private readonly ItemAnimation _animation;
        private readonly CollectTextPrompt _prompt = new CollectTextPrompt();

        private readonly ItemType _itemType;

        public CollectibleItem(ItemAnimation animation, ItemType itemType = ItemType.Strawberry)
        {
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
            _itemType = itemType;
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
        public bool CollectAnimFinished { get; private set; }

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
            CollectAnimFinished = false;
            _animation.Reset();
            _prompt.Reset();
        }

        public static void ResetStrawberryCount()
        {
            _strawberryCount = 0;
        }

        public bool TryCollect(Rectangle playerBounds)
        {
            if (Collected || !Bounds.Intersects(playerBounds))
            {
                return false;
            }

            Collected = true;
            CollectAnimFinished = false;

            if (_itemType == ItemType.Strawberry)
            {
                _strawberryCount++;
                System.Diagnostics.Debug.WriteLine($"Strawberries collected: {_strawberryCount}");
            }

            _prompt.Position = new Vector2(
                Position.X,
                Position.Y
            );

            _prompt.Scale = Scale;
            _prompt.Reset();

            return true;
        }

        public void Update(GameTime gameTime)
        {
            if (!Collected)
            {
                _animation.Update(gameTime);
            }
            else if (!CollectAnimFinished)
            {
                _prompt.Update(gameTime);

                if (_prompt.Finished)
                {
                    CollectAnimFinished = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Collected)
            {
                _animation.Draw(spriteBatch);
            }
            else if (!CollectAnimFinished)
            {
                _prompt.Draw(spriteBatch);
            }
        }
    }
}

