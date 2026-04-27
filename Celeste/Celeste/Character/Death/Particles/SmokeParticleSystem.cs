using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation.Particles
{
    public sealed class SmokeParticleSystem
    {
        private readonly List<SmokeParticle> _particles = new();
        private readonly Texture2D[] _frames;

        public SmokeParticleSystem(Texture2D[] frames)
        {
            _frames = frames;
        }

        public void Add(SmokeParticle particle)
        {
            if (_frames == null || _frames.Length == 0)
            {
                return;
            }

            _particles.Add(particle);
        }

        public void Clear() => _particles.Clear();

        public void Update(float dt)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                SmokeParticle particle = _particles[i];
                particle.Age += dt;
                if (!particle.Alive)
                {
                    _particles.RemoveAt(i);
                    continue;
                }

                particle.Velocity += particle.Acceleration * dt;
                particle.Velocity -= particle.Velocity * particle.Damping * dt;
                particle.Position += particle.Velocity * dt;
                particle.Rotation += particle.AngularVelocity * dt;
                _particles[i] = particle;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_frames == null || _frames.Length == 0)
            {
                return;
            }

            foreach (SmokeParticle particle in _particles)
            {
                float t = particle.T;
                int frameIndex = (int)(t * _frames.Length);
                if (frameIndex >= _frames.Length)
                {
                    frameIndex = _frames.Length - 1;
                }

                Texture2D frame = _frames[frameIndex];
                Vector2 origin = new(frame.Width / 2f, frame.Height / 2f);
                float scale = MathHelper.Lerp(particle.StartScale, particle.EndScale, t);
                float alpha = MathHelper.Lerp(particle.StartAlpha, particle.EndAlpha, t);

                spriteBatch.Draw(
                    frame,
                    particle.Position,
                    null,
                    particle.Tint * alpha,
                    particle.Rotation,
                    origin,
                    scale,
                    particle.Effects,
                    0f);
            }
        }
    }

    public struct SmokeParticle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Vector2 Acceleration;
        public float Damping;
        public float Age;
        public float Lifetime;
        public float StartScale;
        public float EndScale;
        public float StartAlpha;
        public float EndAlpha;
        public float Rotation;
        public float AngularVelocity;
        public Color Tint;
        public SpriteEffects Effects;

        public bool Alive => Age < Lifetime;

        public float T => Lifetime <= 0f
            ? 1f
            : MathHelper.Clamp(Age / Lifetime, 0f, 1f);
    }
}
