using System;
using Celeste.Character;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Items
{
    public sealed class FlyFeatherItem : Celeste.GamePlay.IUpdateable, Celeste.GamePlay.IDrawable
    {
        private const float RespawnSeconds = 2.5f;
        private const float ShieldBounceCooldownSeconds = 0.18f;
        private const float PickupBoundsScale = 0.5f;
        private readonly ItemAnimation _animation;
        private readonly bool _startsShielded;
        private float _respawnTimer;
        private float _shieldBounceCooldown;
        private bool _shielded;

        public FlyFeatherItem(ItemAnimation animation, Vector2 position, bool shielded = false)
        {
            _animation = animation ?? throw new ArgumentNullException(nameof(animation));
            Position = position;
            _startsShielded = shielded;
            _shielded = shielded;
            Active = true;
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

        public bool Active { get; private set; }

        public Rectangle Bounds
        {
            get
            {
                if (!Active)
                {
                    return Rectangle.Empty;
                }

                int width = (int)(_animation.Clip.FrameWidth * Scale);
                int height = (int)(_animation.Clip.FrameHeight * Scale);
                Vector2 center = Position + new Vector2(width * 0.5f, height * 0.5f);
                int pickupWidth = Math.Max(1, (int)(width * PickupBoundsScale));
                int pickupHeight = Math.Max(1, (int)(height * PickupBoundsScale));
                return new Rectangle(
                    (int)(center.X - pickupWidth * 0.5f),
                    (int)(center.Y - pickupHeight * 0.5f),
                    pickupWidth,
                    pickupHeight);
            }
        }

        public void Reset()
        {
            Active = true;
            _shielded = _startsShielded;
            _respawnTimer = 0f;
            _shieldBounceCooldown = 0f;
            _animation.Reset();
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _shieldBounceCooldown = Math.Max(0f, _shieldBounceCooldown - dt);
            if (!Active)
            {
                _respawnTimer = Math.Max(0f, _respawnTimer - dt);
                if (_respawnTimer <= 0f)
                {
                    Active = true;
                    _shielded = _startsShielded;
                    _animation.Reset();
                }
                return;
            }

            _animation.Update(gameTime);
        }

        public bool TryCollect(Madeline player)
        {
            if (player == null || !Active || !Bounds.Intersects(player.Bounds))
            {
                return false;
            }

            if (_shielded && !player.isDashing)
            {
                if (_shieldBounceCooldown <= 0f)
                {
                    player.RefillDash();
                    player.BounceAwayFrom(GetCenter());
                    _shieldBounceCooldown = ShieldBounceCooldownSeconds;
                }

                return false;
            }

            Active = false;
            _respawnTimer = RespawnSeconds;
            _shielded = _startsShielded;
            _animation.Reset();
            return true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!Active)
            {
                return;
            }

            _animation.Draw(spriteBatch, _shielded ? Color.LightBlue : Color.White);
        }

        private Vector2 GetCenter()
        {
            float width = _animation.Clip.FrameWidth * Scale;
            float height = _animation.Clip.FrameHeight * Scale;
            return Position + new Vector2(width * 0.5f, height * 0.5f);
        }
    }
}
