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
        private const float FlyAwaySpeed = 420f;
        private const float FlyAwayDespawnPadding = 64f;

        private readonly ItemAnimation _animation;
        private readonly CollectTextPrompt _prompt = new CollectTextPrompt();

        private readonly ItemType _itemType;
        private readonly bool _fliesAwayOnDash;

        public CollectibleItem(
            ItemAnimation animation,
            ItemType itemType = ItemType.Strawberry,
            bool fliesAwayOnDash = false)
        {
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
            _itemType = itemType;
            _fliesAwayOnDash = fliesAwayOnDash;
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
        public bool IsFlyingAway { get; private set; }
        public bool Escaped { get; private set; }

        public Rectangle Bounds
        {
            get
            {
                if (Collected || IsFlyingAway || Escaped)
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
            IsFlyingAway = false;
            Escaped = false;
            _animation.Reset();
            _prompt.Reset();
        }

        public static void ResetStrawberryCount()
        {
            _strawberryCount = 0;
        }

        public bool TryCollect(Rectangle playerBounds)
        {
            if (Collected || IsFlyingAway || Escaped || !Bounds.Intersects(playerBounds))
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

        public void TriggerFlyAway()
        {
            if (!_fliesAwayOnDash || Collected || IsFlyingAway || Escaped)
            {
                return;
            }

            IsFlyingAway = true;
        }

        public void Update(GameTime gameTime)
        {
            if (Escaped)
            {
                return;
            }

            if (IsFlyingAway)
            {
                float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _animation.Update(gameTime);
                Position += new Vector2(0f, -FlyAwaySpeed * dt);

                if (Position.Y + (_animation.Clip.FrameHeight * Scale) < -FlyAwayDespawnPadding)
                {
                    Escaped = true;
                    IsFlyingAway = false;
                }
            }
            else if (!Collected)
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
            if (Escaped)
            {
                return;
            }

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

