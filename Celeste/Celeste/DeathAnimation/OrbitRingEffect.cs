using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation
{
    public sealed class OrbitRingEffect
    {
        private readonly Texture2D _dotTex;
        private readonly int _count;
        private readonly float _radius;
        private readonly float _angularSpeed; // rad/s, clockwise => negative
        private readonly float _lifetime;
        private readonly float _dotScale;
        private readonly Color _color;

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
            float initialAngle = 0f)
        {
            _dotTex = dotTex;
            Center = center;

            _count = Math.Max(1, count);
            _radius = radius;

            _angularSpeed = -Math.Abs(angularSpeed); // clockwise
            _lifetime = Math.Max(0.01f, lifetime);

            _dotScale = dotScale;
            _color = color;

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
            float alpha = (t < 0.9f) ? 1f : MathHelper.Lerp(1f, 0f, (t - 0.9f) / 0.1f);

            float angle = _initialAngle + _angularSpeed * _age;
            Vector2 origin = new Vector2(_dotTex.Width / 2f, _dotTex.Height / 2f);

            for (int i = 0; i < _count; i++)
            {
                float a = angle + MathHelper.TwoPi * (i / (float)_count);
                Vector2 offset = new Vector2((float)Math.Cos(a), (float)Math.Sin(a)) * _radius;

                sb.Draw(
                    _dotTex,
                    Center + offset,
                    null,
                    _color * alpha,
                    0f,
                    origin,
                    _dotScale,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}