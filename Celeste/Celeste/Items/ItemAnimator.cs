using System;
using Microsoft.Xna.Framework;
using Celeste.Animation;

namespace Celeste.Items
{
    /// <summary>
    /// Playback state machine for an AnimationClip.
    /// Holds timing + current frame; no rendering.
    /// </summary>
    /// <author> Albert Liu </author>
    public sealed class ItemAnimator
    {
        private readonly AnimationClip _clip;

        private float _accumSeconds;
        private int _frameIndex;
        private bool _finished;

        public ItemAnimator(AnimationClip clip)
        {
            _clip = clip ?? throw new ArgumentNullException(nameof(clip));
            Reset();
        }

        public int FrameIndex => _frameIndex;
        public bool Finished => _finished;

        public void Reset()
        {
            _accumSeconds = 0f;
            _frameIndex = 0;
            _finished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (_finished) return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (dt <= 0f) return;

            float fps = _clip.Fps > 0f ? _clip.Fps : 12f;
            float spf = 1f / fps;

            _accumSeconds += dt;

            while (_accumSeconds >= spf)
            {
                _accumSeconds -= spf;
                StepFrame();
                if (_finished) break;
            }
        }

        private void StepFrame()
        {
            if (_clip.FrameCount <= 0)
            {
                _finished = true;
                _frameIndex = 0;
                return;
            }

            _frameIndex++;

            if (_frameIndex >= _clip.FrameCount)
            {
                if (_clip.Loop)
                {
                    _frameIndex = 0;
                }
                else
                {
                    _frameIndex = _clip.FrameCount - 1;
                    _finished = true;
                }
            }
        }
    }
}
