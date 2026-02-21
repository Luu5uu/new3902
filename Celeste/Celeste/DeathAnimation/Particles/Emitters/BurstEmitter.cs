using System;
using Microsoft.Xna.Framework;
using Celeste.DeathAnimation.Particles;

namespace Celeste.DeathAnimation.Particles.Emitters
{
    /// <summary>
    /// One-shot burst particles (explosion pop).
    /// </summary>
    public sealed class BurstEmitter
    {
        private readonly int _count;
        private readonly float _minSpeed, _maxSpeed;
        private readonly float _minLife, _maxLife;
        private readonly float _minSize, _maxSize;
        private readonly Color _tint;

        private readonly Random _rng = new();

        public BurstEmitter(
            int count,
            float minSpeed, float maxSpeed,
            float minLife, float maxLife,
            float minSize, float maxSize,
            Color tint)
        {
            _count = Math.Max(1, count);
            _minSpeed = minSpeed; _maxSpeed = maxSpeed;
            _minLife = minLife; _maxLife = maxLife;
            _minSize = minSize; _maxSize = maxSize;
            _tint = tint;
        }

        public void Emit(ParticleSystem system, Vector2 center)
        {
            for (int i = 0; i < _count; i++)
            {
                float a = (float)(_rng.NextDouble() * MathHelper.TwoPi);
                Vector2 dir = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));

                float speed = Lerp(_minSpeed, _maxSpeed, (float)_rng.NextDouble());
                float life = Lerp(_minLife, _maxLife, (float)_rng.NextDouble());
                float size = Lerp(_minSize, _maxSize, (float)_rng.NextDouble());

                var p = new Particle
                {
                    Position = center,
                    Velocity = dir * speed,

                    Age = 0f,
                    Lifetime = life,

                    StartSize = size,
                    EndSize = size * 0.6f,

                    StartAlpha = 1.0f,
                    EndAlpha = 0.0f,

                    Rotation = 0f,
                    AngularVelocity = (float)(_rng.NextDouble() * 6.0 - 3.0),

                    Tint = _tint
                };

                system.Add(p);
            }
        }

        private static float Lerp(float a, float b, float t) => a + (b - a) * t;
    }
}