
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Character
{
    public class AnimationPlayer
    {
        public AnimationClip _clip;
        public int frame;
        private float timer;

        public void setCurrentAnimation(AnimationClip clip)
        {
            _clip = clip;
            frame = 0;
            timer = 0;

        }

        // Update frames to draw
        public void update(float dt)
        {
            // Time one frame should play
            float frameDuration = 1f / _clip.Fps;
            // Calculate time passes
            timer += dt;

            while(timer >= frameDuration)
            {
                frame++;
                timer -= frameDuration;
            }

            if(frame >= _clip.FrameCount)
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
