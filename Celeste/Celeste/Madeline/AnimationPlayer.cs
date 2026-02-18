
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Character
{
    public class AnimationPlayer
    {
        public AnimationClip _clip;
        public int frame; // Index used to indicate the frame in texture
        private float timer; // Used to get the time and decide to change frame

        public void setCurrentAnimation(AnimationClip clip)
        {
            _clip = clip;
            frame = 0;
            timer = 0;

        }

        // Update frames to draw
        public void update(float dt)
        {
            // Duration (in seconds) that a single animation frame should be shown
            float frameDuration = 1f / _clip.Fps;
            // Accumulate elapsed time since last frame advance
            timer += dt;

            // If dt is large (lag spike), advance multiple frames so animation stays time-correct
            while (timer >= frameDuration)
            {
                frame++;
                timer -= frameDuration;
            }
            // Loop animation back to the start when reaching the end
            if (frame >= _clip.FrameCount)
            {
                frame = 0;
            }
        }

        public void draw(SpriteBatch spriteBatch, Vector2 pos, SpriteEffects side)
        {
            Rectangle src = _clip.GetSourceRect(frame);
            spriteBatch.Draw(_clip.Texture, pos, src,Color.White, 0f, Vector2.Zero,2f,side,0f);
        }
    }
}
