using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation{
    // Single sprite animation: horizontal strip playback with frame-rate independence.
    // Used by AnimationController, not directly by gameplay code.
    // @author Albert Liu
    public class AutoAnimation{
        public Texture2D Texture { get; private set; } = null!;
        public int FrameWidth { get; private set; } = 32;
        public int FrameHeight { get; private set; } = 32;
        public int FrameCount { get; private set; } = 0;

        public bool Loop { get; set; } = true;
        public bool IsPlaying { get; private set; } = false;
        public int CurrentFrame { get; private set; } = 0;

        // Drawing origin within the frame (source-pixel space).
        // (0,0)=top-left, (W/2,H)=center-bottom (feet).
        public Vector2 Origin { get; set; } = Vector2.Zero;

        public float FrameTime { get; private set; } = 1f / 12f;
        private float _accumulator = 0f;

        // Factory: create from an AnimationClip (data-driven).
        public static AutoAnimation FromClip(AnimationClip clip)
        {
            var anim = new AutoAnimation();
            anim.Texture = clip.Texture;
            anim.FrameWidth = clip.FrameWidth;
            anim.FrameHeight = clip.FrameHeight;
            anim.FrameCount = clip.FrameCount;
            anim.Loop = clip.Loop;
            anim.FrameTime = clip.Fps > 0f ? 1f / clip.Fps : 1f / 12f;
            anim.CurrentFrame = 0;
            anim._accumulator = 0f;
            anim.IsPlaying = true;
            return anim;
        }

        // Initialize from a horizontal sprite strip texture.
        public void Detect(Texture2D texture, int frameWidth = 32, int frameHeight = 32, float fps = 12f, bool loop = true){
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Loop = loop;

            if (fps <= 0f) fps = 12f;
            FrameTime = 1f / fps;

            if (Texture.Height < FrameHeight)
                throw new System.ArgumentException($"Texture height {Texture.Height} < frameHeight {FrameHeight}.");
            if (Texture.Width % FrameWidth != 0)
                throw new System.ArgumentException($"Texture width {Texture.Width} is not divisible by frameWidth {FrameWidth}.");

            FrameCount = Texture.Width / FrameWidth;
            CurrentFrame = 0;
            _accumulator = 0f;
            IsPlaying = true;
        }

        public void Play(){
            if (FrameCount <= 0) return;
            IsPlaying = true;
        }

        public void Pause(){
            IsPlaying = false;
        }

        // Set frame directly (wraps around). For debug stepping.
        public void SetFrame(int frame){
            if (FrameCount <= 0) return;
            CurrentFrame = ((frame % FrameCount) + FrameCount) % FrameCount;
            _accumulator = 0f;
        }

        public void Stop(){
            IsPlaying = false;
            CurrentFrame = 0;
            _accumulator = 0f;
        }

        public void Update(GameTime gameTime){
            if (!IsPlaying || FrameCount <= 0) return;

            _accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_accumulator >= FrameTime){
                _accumulator -= FrameTime;
                CurrentFrame++;

                if (CurrentFrame >= FrameCount){
                    if (Loop) CurrentFrame = 0;
                    else{
                        CurrentFrame = FrameCount - 1;
                        IsPlaying = false;
                        break;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f){
            if (FrameCount <= 0) return;
            Rectangle src = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, SpriteEffects.None, 0f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale, SpriteEffects effects){
            if (FrameCount <= 0) return;
            Rectangle src = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, effects, 0f);
        }

        // Vec2 scale overload: negative X for facing (avoids 1px SpriteEffects shift).
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, Vector2 scale){
            if (FrameCount <= 0) return;
            Rectangle src = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, position, src, color, 0f, Origin, scale, SpriteEffects.None, 0f);
        }
    }
}
