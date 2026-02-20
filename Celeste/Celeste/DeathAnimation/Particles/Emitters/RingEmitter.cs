using System;
using Microsoft.Xna.Framework;
using Celeste.DeathAnimation.Particles;

namespace Celeste.DeathAnimation.Particles.Emitters
{
    /// <summary>
    /// Emits particles arranged evenly on a ring, moving outward.
    /// </summary>
    public sealed class RingEmitter
    {
        private readonly int _count;
        private readonly float _speed;
        private readonly float _life;
        private readonly float _size;

        private readonly Random _rng = new();

        public RingEmitter(int count, float speed, float life, float size)
        {
            _count = Math.Max(1, count);
            _speed = speed;
            _life = life;
            _size = size;
        }

        public void Emit(ParticleSystem system, Vector2 center)
        {
            // evenly spaced angles + small jitter
            for (int i = 0; i < _count; i++)
            {
                float baseAngle = MathHelper.TwoPi * (i / (float)_count);
                float jitter = MathHelper.ToRadians((float)(_rng.NextDouble() * 10.0 - 5.0)); // ±5 deg
                float a = baseAngle + jitter;

                var dir = new Vector2((float)Math.Cos(a), (float)Math.Sin(a));

                Particle p = new Particle
                {
                    Position = center,
                    Velocity = dir * _speed,

                    Age = 0f,
                    Lifetime = _life,

                    StartSize = _size,
                    EndSize = _size * 0.75f,

                    StartAlpha = 0.95f,
                    EndAlpha = 0.0f,

                    Rotation = 0f,
                    AngularVelocity = 0f
                };

                system.Add(p);
            }
        }
    }
}
