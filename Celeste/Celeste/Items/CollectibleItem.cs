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

        private readonly ItemAnimation _animation;
        private readonly CollectTextPrompt _prompt = new CollectTextPrompt();
        private readonly ItemType _itemType;
        private readonly string _collectibleId;
        private readonly bool _fliesAwayOnDash;

        private bool _previouslyCollected;

        public CollectibleItem(
            string collectibleId,
            ItemAnimation animation,
            ItemType itemType = ItemType.Strawberry,
            bool fliesAwayOnDash = false)
        {
            _collectibleId = collectibleId ?? throw new ArgumentNullException(nameof(collectibleId));
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
            _itemType = itemType;
            _fliesAwayOnDash = fliesAwayOnDash;
        }

        public string CollectibleId => _collectibleId;
        public ItemType Type => _itemType;

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

        public void SetPreviouslyCollected(bool previouslyCollected)
        {
            _previouslyCollected = previouslyCollected;
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

        public bool TryCollect(Rectangle playerBounds)
        {
            if (Collected || IsFlyingAway || Escaped || !Bounds.Intersects(playerBounds))
            {
                return false;
            }

            Collected = true;
            CollectAnimFinished = false;

            _prompt.Position = new Vector2(Position.X, Position.Y);
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
                Position += new Vector2(0f, -420f * dt);

                if (Position.Y + (_animation.Clip.FrameHeight * Scale) < -64f)
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
                Color tint = _previouslyCollected && _itemType == ItemType.Strawberry
                    ? Color.LightBlue
                    : Color.White;

                _animation.Draw(spriteBatch, tint);
            }
            else if (!CollectAnimFinished)
            {
                _prompt.Draw(spriteBatch);
            }
        }
    }
}
