using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Celeste.Animation;
using Celeste.DeathAnimation.Particles;
using Celeste.DeathAnimation.Particles.Emitters;

using static Celeste.DeathConstants;

namespace Celeste.DeathAnimation
{
    public sealed class DeathEffect
    {
        private readonly float _scale;

        private readonly DeathSpritePlayer _deathSprite;

        private readonly Texture2D _dotTexture;
        private readonly ParticleSystem _burstParticles;
        private readonly BurstEmitter _burstEmitter;

        private bool _spawnedAfterDeath = false;

        public bool IsFinished =>
            _deathSprite.IsFinished
            && _burstParticles.IsEmpty;

        /// <param name="worldTopLeft">
        /// TOP-LEFT of sprite, same as Madeline.position (critical for alignment).
        /// </param>
        public DeathEffect(AnimationClip deathClip, Texture2D dotTexture, Vector2 worldTopLeft, Color particleColor, float scale = 1f)
        {
            _scale = scale;
            _dotTexture = dotTexture;

            // Death sprite now anchored at TOP-LEFT
            _deathSprite = new DeathSpritePlayer(deathClip, worldTopLeft, _scale);

            _burstParticles = new ParticleSystem(_dotTexture);

            _burstEmitter = new BurstEmitter(
                count:    BurstCount,
                minSpeed: BurstMinSpeed * _scale,
                maxSpeed: BurstMaxSpeed * _scale,
                minLife:  BurstMinLife,
                maxLife:  BurstMaxLife,
                minSize:  BurstMinSize * _scale,
                maxSize:  BurstMaxSize * _scale,
                tint:     particleColor
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

                _spawnedAfterDeath = true;
            }

            _burstParticles.Update(dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _deathSprite.Draw(spriteBatch);
            _burstParticles.Draw(spriteBatch);
        }
    }
}
