using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation{
    /// <summary>
    /// Represents a single automatically-playing sprite animation.
    /// 
    /// Responsibilities:
    /// - Automatically detect frame count from a horizontal sprite strip
    /// - Advance frames based on elapsed time (frame-rate independent)
    /// - Draw the current frame with optional scaling and flipping
    ///
    /// This class does NOT:
    /// - Handle input
    /// - Decide which animation should play
    /// - Manage SpriteBatch.Begin / End
    ///
    /// It is intended to be used by AnimationController, not directly by gameplay code.
    /// </summary>
    /// <author> Albert Liu </author>
    public class AutoAnimation{
        public Texture2D Texture { get; private set; } = null!;
        public int FrameWidth { get; private set; } = 32;
        public int FrameHeight { get; private set; } = 32;
        public int FrameCount { get; private set; } = 0;

        public bool Loop { get; set; } = true;
        public bool IsPlaying { get; private set; } = false;

        public int CurrentFrame { get; private set; } = 0;

        // seconds per frame
        public float FrameTime { get; private set; } = 1f / 12f;

        private float _accumulator = 0f;

        /// <summary>
        /// Initializes the animation by detecting frame count and configuring playback parameters.
        /// This method must be called before Update or Draw.
        /// </summary>
        /// <param name="texture">Sprite strip texture containing frames arranged horizontally.</param>
        /// <param name="frameWidth">Width of a single frame in pixels.</param>
        /// <param name="frameHeight">Height of a single frame in pixels.</param>
        /// <param name="fps">Frames per second for playback.</param>
        /// <param name="loop">Whether the animation should loop.</param>
        public void Detect(Texture2D texture, int frameWidth = 32, int frameHeight = 32, float fps = 12f, bool loop = true){
            Texture = texture;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Loop = loop;

            if (fps <= 0f) fps = 12f;
            FrameTime = 1f / fps;

            // validation if the content not following with 32 x 32
            if (Texture.Height < FrameHeight)
                throw new System.ArgumentException($"Texture height {Texture.Height} < frameHeight {FrameHeight}.");

            if (Texture.Width % FrameWidth != 0)
                throw new System.ArgumentException($"Texture width {Texture.Width} is not divisible by frameWidth {FrameWidth}.");

            FrameCount = Texture.Width / FrameWidth;

            CurrentFrame = 0;
            _accumulator = 0f;
            IsPlaying = true;
        }

        /// <summary>
        /// Starts or resumes playback without resetting the current frame.
        /// </summary>        
        public void Play(){
            if (FrameCount <= 0) return;
            IsPlaying = true;
        }

        /// <summary>
        /// Pauses playback while keeping the current frame.
        /// </summary>
        public void Pause(){
            IsPlaying = false;
        } 

        /// <summary>
        /// Stops playback and resets the animation to the first frame.
        /// </summary>
        public void Stop(){
            IsPlaying = false;
            CurrentFrame = 0;
            _accumulator = 0f;
        }

        /// <summary>
        /// Advances the animation based on elapsed game time.
        /// Should be called once per frame.
        /// </summary>
        /// <param name="gameTime">Provides elapsed time since last update.</param>
        public void Update(GameTime gameTime){
            if (!IsPlaying || FrameCount <= 0) return;

            _accumulator += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (_accumulator >= FrameTime){
                _accumulator -= FrameTime;
                CurrentFrame++;

                if (CurrentFrame >= FrameCount){
                    //Loop animation, back to the beginning; Non-loop animation, stop at the last frame and stop
                    if (Loop) CurrentFrame = 0;
                    else{
                        CurrentFrame = FrameCount - 1;
                        IsPlaying = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Draws the current frame at the given position using no sprite effects.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f){
            if (FrameCount <= 0) return;

            Rectangle src = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, position, src, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        
        /// <summary>
        /// Draws the current frame at the given position using no sprite effects.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale, SpriteEffects effects){
            if (FrameCount <= 0) return;

            Rectangle src = new Rectangle(CurrentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            spriteBatch.Draw(Texture, position, src, color, 0f, Vector2.Zero, scale, effects, 0f);
        }
        

    }
}
