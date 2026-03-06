using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    /// <summary>
    /// Playback state for an AnimationClip (atlas-based).
    /// Draw uses clip.GetSourceRect(frame) so Y is not forced to 0.
    /// </summary>
    public sealed class AutoAnimation
    {
        private AnimationClip _clip = null!;

        public Texture2D Texture => _clip.Texture;
        public int FrameWidth => _clip.FrameWidth;
        public int FrameHeight => _clip.FrameHeight;
        public int FrameCount => _clip.FrameCount;

        public bool Loop
        {
            get => _clip.Loop;
            set
            {
                // optional: if you want runtime override, store a separate field.
                _loopOverride = value;
                _hasLoopOverride = true;
            }
        }

        private bool _hasLoopOverride = false;
        private bool _loopOverride = true;

        private bool EffectiveLoop => _hasLoopOverride ? _loopOverride : _clip.Loop;

        public bool IsPlaying { get; private set; }
        public int CurrentFrame { get; private set; }

        public Vector2 Origin { get; set; } = Vector2.Zero;

        // seconds per frame
        public float FrameTime { get; private set; } = 1f / 12f;
        private float _accumulator;

        public static AutoAnimation FromClip(AnimationClip clip)
        {
            if (clip == null) throw new ArgumentNullException(nameof(clip));

            var anim = new AutoAnimation();
            anim._clip = clip;

            float fps = clip.Fps > 0f ? clip.Fps : 12f;
            anim.FrameTime = 1f / fps;

            anim.CurrentFrame = 0;
            anim._accumulator = 0f;
            anim.IsPlaying = true;
            return anim;
        }

        public void Play()
        {
            if (FrameCount <= 0) return;
            IsPlaying = true;
        }

        public void Pause() => IsPlaying = false;

        public void Stop()
        {
            IsPlaying = false;
            CurrentFrame = 0;
            _accumulator = 0f;
        }

        public void SetFrame(int frame)
        {
            if (FrameCount <= 0) return;
            CurrentFrame = ((frame % FrameCount) + FrameCount) % FrameCount;
            _accumulator = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (!IsPlaying || FrameCount <= 0) return;

            _accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_accumulator >= FrameTime)
            {
                _accumulator -= FrameTime;
                CurrentFrame++;

                if (CurrentFrame >= FrameCount)
                {
                    if (EffectiveLoop) CurrentFrame = 0;
                    else
                    {
                        CurrentFrame = FrameCount - 1;
                        IsPlaying = false;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f)
        {
            if (FrameCount <= 0) return;

            Rectangle src = _clip.GetSourceRect(CurrentFrame);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale, SpriteEffects effects)
        {
            if (FrameCount <= 0) return;

            Rectangle src = _clip.GetSourceRect(CurrentFrame);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, effects, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale)
        {
            if (FrameCount <= 0) return;

            Rectangle src = _clip.GetSourceRect(CurrentFrame);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, SpriteEffects.None, 0f);
        }
    }
}
