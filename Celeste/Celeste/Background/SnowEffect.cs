using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Background
{
    public sealed class SnowEffect
    {
        private struct SnowParticle
        {
            public Vector2 Position;
            public float SpeedX;
            public float SpeedY;
            public float Age;
            public float Lifetime;
            public Color Color;
            public float Size;
        }

        private readonly struct SnowLayer
        {
            public readonly Color Color;
            public readonly float MinSpeed;
            public readonly float MaxSpeed;
            public readonly int Count;
            public readonly float Lifetime;
            public readonly float Size;

            public SnowLayer(Color color, float minSpeed, float maxSpeed, int count, float lifetime, float size)
            {
                Color = color; MinSpeed = minSpeed; MaxSpeed = maxSpeed;
                Count = count; Lifetime = lifetime; Size = size;
            }
        }

        private static readonly SnowLayer[] Layers =
        {
            new(new Color(110, 168, 255), 160f, 220f, 32, 6.0f, 1.5f),
            new(Color.White,              160f, 220f, 32, 6.0f, 1.2f),
            new(new Color(58,  64,  71),   40f,  80f, 32, 6.0f, 2.0f),
        };

        private readonly List<SnowParticle> _particles = new();
        private Texture2D _dot;
        private int _screenWidth;
        private int _screenHeight;
        private readonly Random _rng = new();

        public void Initialize(GraphicsDevice gd, int screenWidth, int screenHeight)
        {
            _screenWidth  = screenWidth;
            _screenHeight = screenHeight;

            _dot = new Texture2D(gd, 2, 2);
            _dot.SetData(new[] { Color.White, Color.White, Color.White, Color.White });

            foreach (var layer in Layers)
            {
                for (int i = 0; i < layer.Count; i++)
                {
                    _particles.Add(SpawnParticle(layer, scattered: true, randomizeAge: true));
                }
            }
        }

        public void Update(float dt)
        {
            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                SnowParticle p = _particles[i];
                p.Age += dt;
                p.Position.X += p.SpeedX * dt;
                p.Position.Y += p.SpeedY * dt + (float)Math.Sin(p.Age * 2.5f) * 6f * dt;

                bool exitedLeft = p.Position.X < -4f;
                bool expired    = p.Age >= p.Lifetime;
                if (exitedLeft || expired)
                {
                    _particles.RemoveAt(i);
                    _particles.Insert(i, RespawnParticle(p, scattered: expired && !exitedLeft));
                    continue;
                }

                _particles[i] = p;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_dot == null) return;

            foreach (SnowParticle p in _particles)
            {
                float t = p.Age / p.Lifetime;
                float alpha = t < 0.1f ? t / 0.1f : (t > 0.85f ? (1f - t) / 0.15f : 1f);
                spriteBatch.Draw(
                    _dot,
                    p.Position,
                    null,
                    p.Color * (alpha * 0.75f),
                    0f,
                    Vector2.Zero,
                    p.Size,
                    SpriteEffects.None,
                    0f);
            }
        }

        private SnowParticle SpawnParticle(SnowLayer layer, bool scattered, bool randomizeAge = false)
        {
            float x = scattered ? (float)(_rng.NextDouble() * _screenWidth) : _screenWidth + 4f;
            float y = (float)(_rng.NextDouble() * _screenHeight);
            float speedX = -(layer.MinSpeed + (float)(_rng.NextDouble() * (layer.MaxSpeed - layer.MinSpeed)));
            float spreadY = (float)(_rng.NextDouble() * 20f - 10f);
            float age     = randomizeAge ? (float)(_rng.NextDouble() * layer.Lifetime) : 0f;
            return new SnowParticle
            {
                Position = new Vector2(x, y),
                SpeedX   = speedX,
                SpeedY   = spreadY,
                Age      = age,
                Lifetime = layer.Lifetime,
                Color    = layer.Color,
                Size     = layer.Size,
            };
        }

        private SnowParticle RespawnParticle(SnowParticle old, bool scattered)
        {
            foreach (var layer in Layers)
            {
                if (layer.Color == old.Color)
                {
                    return SpawnParticle(layer, scattered);
                }
            }
            return SpawnParticle(Layers[0], scattered);
        }
    }
}
