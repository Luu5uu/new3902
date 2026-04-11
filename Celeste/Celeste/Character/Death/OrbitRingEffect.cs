using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation
{
    public sealed class OrbitRingEffect
    {
        private readonly Texture2D _dotTex;
        private readonly int _count;
        private readonly float _startRadius;
        private readonly float _endRadius;
        private readonly float _angularSpeed; // rad/s, clockwise => negative
        private readonly float _lifetime;
        private readonly float _startDotScale;
        private readonly float _endDotScale;
        private readonly Color _startColor;
        private readonly Color _endColor;
        private readonly float _fadeStart;

        private float _age;
        private float _initialAngle;

        public Vector2 Center { get; }
        public bool IsFinished => _age >= _lifetime;

        public OrbitRingEffect(
            Texture2D dotTex,
            Vector2 center,
            int count,
            float radius,
            float angularSpeed,
            float lifetime,
            float dotScale,
            Color color,
            float initialAngle = 0f,
            float? endRadius = null,
            float? endDotScale = null,
            Color? endColor = null,
            float fadeStart = 0.8f)
        {
            _dotTex = dotTex;
            Center = center;

            _count = Math.Max(1, count);
            _startRadius = radius;
            _endRadius = endRadius ?? radius;

            _angularSpeed = -Math.Abs(angularSpeed); // clockwise
            _lifetime = Math.Max(0.01f, lifetime);

            _startDotScale = dotScale;
            _endDotScale = endDotScale ?? dotScale;
            _startColor = color;
            _endColor = endColor ?? color;
            _fadeStart = MathHelper.Clamp(fadeStart, 0f, 1f);

            _initialAngle = initialAngle;
            _age = 0f;
        }

        public void Update(float dt)
        {
            _age += dt;
        }

        public void Draw(SpriteBatch sb)
        {
            float t = MathHelper.Clamp(_age / _lifetime, 0f, 1f);
            float alpha = (t < _fadeStart) ? 1f : MathHelper.Lerp(1f, 0f, (t - _fadeStart) / Math.Max(0.0001f, 1f - _fadeStart));
            float motionT = MathHelper.SmoothStep(0f, 1f, t);
            float radius = MathHelper.Lerp(_startRadius, _endRadius, motionT);
            float dotScale = MathHelper.Lerp(_startDotScale, _endDotScale, motionT);
            Color color = Color.Lerp(_startColor, _endColor, motionT);

            float angle = _initialAngle + _angularSpeed * _age;
            Vector2 origin = new Vector2(_dotTex.Width / 2f, _dotTex.Height / 2f);

            for (int i = 0; i < _count; i++)
            {
                float a = angle + MathHelper.TwoPi * (i / (float)_count);
                Vector2 offset = new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * radius;

                sb.Draw(
                    _dotTex,
                    Center + offset,
                    null,
                    color * alpha,
                    0f,
                    origin,
                    dotScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}
