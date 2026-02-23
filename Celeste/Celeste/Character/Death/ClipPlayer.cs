using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.DeathAnimation
{
    public sealed class ClipPlayer
    {
        private readonly AnimationClip _clip;

        private int _currentFrame;
        private float _timer;
        private readonly float _frameDuration;

        public bool Loop { get; set; }
        public bool IsPlaying { get; private set; }

        public int CurrentFrame => _currentFrame;

        public ClipPlayer(AnimationClip clip, bool? overrideLoop = null)
        {
            _clip = clip;
            Loop = overrideLoop ?? clip.Loop;

            _frameDuration = (_clip.Fps > 0f) ? (1f / _clip.Fps) : (1f / 12f);

            _currentFrame = 0;
            _timer = 0f;
            IsPlaying = true;
        }

        public void Update(float dt)
        {
            if (!IsPlaying) return;

            _timer += dt;

            while (_timer >= _frameDuration)
            {
                _timer -= _frameDuration;
                _currentFrame++;

                if (_currentFrame >= _clip.FrameCount)
                {
                    if (Loop)
                    {
                        _currentFrame = 0;
                    }
                    else
                    {
                        _currentFrame = _clip.FrameCount - 1;
                        IsPlaying = false;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, Vector2 origin, Color color)
        {
            Rectangle source = _clip.GetSourceRect(_currentFrame);

            spriteBatch.Draw(
                _clip.Texture,
                position,
                source,
                color,
                0f,
                origin,
                scale,
                SpriteEffects.None,
                0f
            );
        }
    }
}
