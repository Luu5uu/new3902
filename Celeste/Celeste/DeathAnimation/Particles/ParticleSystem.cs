using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation.Particles
{
    public sealed class ParticleSystem
    {
        private readonly List<Particle> _particles = new();
        private readonly Texture2D _tex;

        public bool IsEmpty => _particles.Count == 0;

        public ParticleSystem(Texture2D particleTexture)
        {
            _tex = particleTexture;
        }

        public void Add(Particle p) => _particles.Add(p);

        public void Update(float dt)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                Particle p = _particles[i];
                p.Age += dt;

                if (!p.Alive)
                {
                    _particles.RemoveAt(i);
                    continue;
                }

                p.Position += p.Velocity * dt;
                p.Rotation += p.AngularVelocity * dt;

                _particles[i] = p;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(_tex.Width / 2f, _tex.Height / 2f);

            foreach (var p in _particles)
            {
                float t = p.T;
                float size = MathHelper.Lerp(p.StartSize, p.EndSize, t);
                float alpha = MathHelper.Lerp(p.StartAlpha, p.EndAlpha, t);

                Color tint = (p.Tint == default) ? Color.White : p.Tint;

                spriteBatch.Draw(
                    _tex,
                    p.Position,
                    null,
                    tint * alpha,
                    p.Rotation,
                    origin,
                    size,
                    SpriteEffects.None,
                    0f
                );
            }
        }
    }
}