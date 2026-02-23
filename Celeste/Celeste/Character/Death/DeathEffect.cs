using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Celeste.Animation;
using Celeste.DeathAnimation.Particles;
using Celeste.DeathAnimation.Particles.Emitters;

namespace Celeste.DeathAnimation
{
    public sealed class DeathEffect
    {
        private readonly float _scale;

        private readonly DeathSpritePlayer _deathSprite;

        private readonly Texture2D _dotTexture;
        private readonly ParticleSystem _burstParticles;
        private readonly BurstEmitter _burstEmitter;

        private OrbitRingEffect _orbitRing;
        private bool _spawnedAfterDeath = false;

        public bool IsFinished =>
            _deathSprite.IsFinished
            && _burstParticles.IsEmpty
            && (_orbitRing == null || _orbitRing.IsFinished);

        /// <param name="worldTopLeft">
        /// TOP-LEFT of sprite, same as Madeline.position (critical for alignment).
        /// </param>
        public DeathEffect(AnimationClip deathClip, Texture2D dotTexture, Vector2 worldTopLeft, float scale = 1f)
        {
            _scale = scale;
            _dotTexture = dotTexture;

            // Death sprite now anchored at TOP-LEFT
            _deathSprite = new DeathSpritePlayer(deathClip, worldTopLeft, _scale);

            _burstParticles = new ParticleSystem(_dotTexture);

            var cyanBlue = new Color(120, 225, 255);

            _burstEmitter = new BurstEmitter(
                count: 12,
                minSpeed: 110f * _scale,
                maxSpeed: 190f * _scale,
                minLife: 0.18f,
                maxLife: 0.35f,
                minSize: 1.2f * _scale,
                maxSize: 2.2f * _scale,
                tint: cyanBlue
            );
        }

        public void Update(float dt)
        {
            _deathSprite.Update(dt);

            // strict: only after death sprite finished
            if (!_spawnedAfterDeath && _deathSprite.IsFinished)
            {
                // spawn particles at sprite CENTER (computed from top-left + scaled size)
                Vector2 center = _deathSprite.GetScaledCenter();

                _burstEmitter.Emit(_burstParticles, center);

                _orbitRing = new OrbitRingEffect(
                    dotTex: _dotTexture,
                    center: center,
                    count: 8,
                    radius: 25f * _scale,
                    angularSpeed: 1.25f,
                    lifetime: 1.5f,
                    dotScale: 1.8f * _scale,
                    color: new Color(120, 225, 255),
                    initialAngle: 0f
                );

                _spawnedAfterDeath = true;
            }

            _burstParticles.Update(dt);
            _orbitRing?.Update(dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _deathSprite.Draw(spriteBatch);
            _burstParticles.Draw(spriteBatch);
            _orbitRing?.Draw(spriteBatch);
        }
    }
}
