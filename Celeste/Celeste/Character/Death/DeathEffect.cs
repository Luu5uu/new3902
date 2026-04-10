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
        private static Texture2D _sharedPixelTexture;

        private readonly float _scale;

        private readonly DeathSpritePlayer _deathSprite;

        private readonly Texture2D _dotTexture;
        private readonly Texture2D _pixelTexture;
        private readonly ParticleSystem _burstParticles;
        private readonly BurstEmitter _burstEmitter;
        private readonly OrbitRingEffect _deathRing;

        private float _elapsed;
        private bool _spawnedAfterDeath = false;

        public bool IsFinished =>
            _elapsed >= DeathCutoutDelay + DeathCutoutDuration
            && _deathSprite.IsFinished
            && (_deathRing == null || _deathRing.IsFinished)
            && _burstParticles.IsEmpty;

        /// <param name="worldTopLeft">
        /// TOP-LEFT of sprite, same as Madeline.position (critical for alignment).
        /// </param>
        public DeathEffect(AnimationClip deathClip, Texture2D dotTexture, Vector2 worldTopLeft, Color particleColor, bool faceLeft = false, float scale = 1f)
        {
            _scale = scale;
            _dotTexture = dotTexture;
            if (_sharedPixelTexture == null)
            {
                _sharedPixelTexture = new Texture2D(dotTexture.GraphicsDevice, 1, 1);
                _sharedPixelTexture.SetData(new[] { Color.White });
            }
            _pixelTexture = _sharedPixelTexture;

            // Death sprite now anchored at TOP-LEFT
            _deathSprite = new DeathSpritePlayer(deathClip, worldTopLeft, _scale, faceLeft, DeathSpritePlaybackMultiplier);
            Vector2 center = _deathSprite.GetScaledCenter();

            _deathRing = new OrbitRingEffect(
                _dotTexture,
                center,
                count: DeathRingCount,
                radius: DeathRingRadius * _scale,
                angularSpeed: 0f,
                lifetime: DeathRingLifetime,
                dotScale: DeathRingDotScale * _scale,
                color: particleColor,
                initialAngle: -MathHelper.PiOver2,
                endRadius: DeathRingEndRadius * _scale,
                endDotScale: DeathRingEndDotScale * _scale,
                endColor: Color.White,
                fadeStart: DeathRingFadeStart);

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
            _elapsed += dt;
            _deathSprite.Update(dt);
            _deathRing?.Update(dt);

            if (!_spawnedAfterDeath && _elapsed >= DeathBurstDelay)
            {
                _burstEmitter.Emit(_burstParticles, _deathSprite.GetScaledCenter());
                _spawnedAfterDeath = true;
            }

            _burstParticles.Update(dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            DrawCutout(spriteBatch);
            _deathSprite.Draw(spriteBatch);
            _deathRing?.Draw(spriteBatch);
            _burstParticles.Draw(spriteBatch);
        }

        private void DrawCutout(SpriteBatch spriteBatch)
        {
            float progress = MathHelper.Clamp((_elapsed - DeathCutoutDelay) / DeathCutoutDuration, 0f, 1f);
            if (progress <= 0f)
            {
                return;
            }

            float holeHalfSize = MathHelper.Lerp(DeathCutoutMaxHalfSize * _scale, 0f, progress * progress);
            Rectangle viewport = spriteBatch.GraphicsDevice.Viewport.Bounds;
            Vector2 center = _deathSprite.GetScaledCenter();
            int holeLeft = (int)(center.X - holeHalfSize);
            int holeRight = (int)(center.X + holeHalfSize);
            int holeTop = (int)(center.Y - holeHalfSize);
            int holeBottom = (int)(center.Y + holeHalfSize);
            Color mask = Color.Black * progress;

            if (holeTop > viewport.Top)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(viewport.Left, viewport.Top, viewport.Width, holeTop - viewport.Top), mask);
            }

            if (holeBottom < viewport.Bottom)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(viewport.Left, holeBottom, viewport.Width, viewport.Bottom - holeBottom), mask);
            }

            int middleHeight = System.Math.Max(0, holeBottom - holeTop);
            if (holeLeft > viewport.Left && middleHeight > 0)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(viewport.Left, holeTop, holeLeft - viewport.Left, middleHeight), mask);
            }

            if (holeRight < viewport.Right && middleHeight > 0)
            {
                spriteBatch.Draw(_pixelTexture, new Rectangle(holeRight, holeTop, viewport.Right - holeRight, middleHeight), mask);
            }
        }
    }
}
