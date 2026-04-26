using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.Blocks
{
    public class MoveBlock : IBlocks
    {
        private enum MoveBlockState
        {
            ArrivedRed,
            PrepareGreen,
            MovingYellow,
        }

        private const int RedFrameIndex = 0;
        private const int YellowFrameIndex = 1;
        private const int GreenFrameIndex = 2;
        private const float RedLightSeconds = 3f;
        private const float GreenLightSeconds = 2f;

        public Vector2 Position
        { get; set; }

        public Texture2D Texture
        {
            get => _clip.Texture;
            set { }
        }

        public string Type => "moveBlock";
        public float Scale { get; set; } = 2.0f;
        public Vector2 PreviousPosition { get; private set; }
        public Vector2 MovementDelta { get; private set; }
        public Rectangle PreviousBounds => GetBoundsAt(PreviousPosition);

        private readonly AnimationClip _clip;
        private readonly Vector2 _start;
        private readonly Vector2 _end;
        private readonly float _speed;
        private bool _movingTowardEnd = true;
        private MoveBlockState _state;
        private float _stateTimer;
        private int _frameIndex;

        public MoveBlock(Vector2 start, float distance, float speed, float angleDegrees, AnimationCatalog catalog, float scale = 2.5f)
        {
            _clip = catalog.Clips[AnimationKeys.DevicesMoveBlock];
            float angleRad = MathHelper.ToRadians(angleDegrees);
            _speed = speed;
            Vector2 direction = new Vector2((float)Math.Cos(angleRad), (float)Math.Sin(angleRad));
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
            }

            _start = start;
            _end = start + direction * distance;
            Position = _start;
            PreviousPosition = _start;
            MovementDelta = Vector2.Zero;
            _state = MoveBlockState.ArrivedRed;
            _stateTimer = RedLightSeconds;
            _frameIndex = RedFrameIndex;
            Scale = scale;
        }

        public void Update(GameTime gameTime)
        {
            PreviousPosition = Position;
            MovementDelta = Vector2.Zero;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (_state)
            {
                case MoveBlockState.ArrivedRed:
                    UpdateArrivedRed(dt);
                    break;
                case MoveBlockState.PrepareGreen:
                    UpdatePrepareGreen(dt);
                    break;
                case MoveBlockState.MovingYellow:
                    UpdateMovingYellow(dt);
                    break;
            }

            MovementDelta = Position - PreviousPosition;
        }

        private void UpdateArrivedRed(float dt)
        {
            _frameIndex = RedFrameIndex;
            _stateTimer -= dt;
            if (_stateTimer > 0f)
            {
                return;
            }

            _state = MoveBlockState.PrepareGreen;
            _stateTimer = GreenLightSeconds;
            _frameIndex = GreenFrameIndex;
        }

        private void UpdatePrepareGreen(float dt)
        {
            _frameIndex = GreenFrameIndex;
            _stateTimer -= dt;
            if (_stateTimer > 0f)
            {
                return;
            }

            _state = MoveBlockState.MovingYellow;
            _frameIndex = YellowFrameIndex;
        }

        private void UpdateMovingYellow(float dt)
        {
            _frameIndex = YellowFrameIndex;

            Vector2 target = _movingTowardEnd ? _end : _start;
            Vector2 remaining = target - Position;
            float remainingDistance = remaining.Length();
            if (remainingDistance <= float.Epsilon)
            {
                ArriveAt(target);
                return;
            }

            float travelDistance = _speed * dt;
            if (travelDistance >= remainingDistance)
            {
                ArriveAt(target);
                return;
            }

            remaining /= remainingDistance;
            Position += remaining * travelDistance;
        }

        private void ArriveAt(Vector2 target)
        {
            Position = target;
            _movingTowardEnd = !_movingTowardEnd;
            _state = MoveBlockState.ArrivedRed;
            _stateTimer = RedLightSeconds;
            _frameIndex = RedFrameIndex;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _clip.Texture,
                Position,
                _clip.GetSourceRect(_frameIndex),
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }

        public Rectangle Bounds
        {
            get => GetBoundsAt(Position);
        }

        private Rectangle GetBoundsAt(Vector2 position)
        {
            if (Texture == null)
            {
                return Rectangle.Empty;
            }

            int w = (int)(_clip.FrameWidth * Scale);
            int h = (int)(_clip.FrameHeight * Scale);

            return new Rectangle(
                (int)position.X,
                (int)position.Y,
                w,
                h
            );
        }
    }
}
