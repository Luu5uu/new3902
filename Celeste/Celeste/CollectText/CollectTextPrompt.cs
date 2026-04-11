using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.CollectText
{
    internal sealed class CollectTextPrompt
    {
        private static Texture2D _sharedTexture;
        private static bool _initialized;

        private const string AssetName = "collect_text_prompt_spritesheet_aligned";
        private const int FrameWidth = 18;
        private const int FrameHeight = 16;
        private const int FrameCount = 8;
        private const int FramesPerRow = 8;
        private const float Fps = 12f;

        private float _timer;
        private int _currentFrame;

        public Vector2 Position { get; set; }
        public float Scale { get; set; } = 1f;
        public bool Finished { get; private set; }

        public static void Initialize(ContentManager content)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            if (_initialized)
                return;

            _sharedTexture = content.Load<Texture2D>(AssetName);
            _initialized = true;
        }

        public void Reset()
        {
            _timer = 0f;
            _currentFrame = 0;
            Finished = false;
        }

        public void Update(GameTime gameTime)
        {
            if (!_initialized || Finished)
                return;

            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float secondsPerFrame = 1f / Fps;

            _timer += dt;

            while (_timer >= secondsPerFrame)
            {
                _timer -= secondsPerFrame;
                _currentFrame++;

                if (_currentFrame >= FrameCount)
                {
                    _currentFrame = FrameCount - 1;
                    Finished = true;
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_initialized || Finished)
                return;

            int frameX = (_currentFrame % FramesPerRow) * FrameWidth;
            int frameY = (_currentFrame / FramesPerRow) * FrameHeight;

            Rectangle sourceRect = new Rectangle(frameX, frameY, FrameWidth, FrameHeight);

            spriteBatch.Draw(
                _sharedTexture,
                Position,
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                Scale,
                SpriteEffects.None,
                0f);
        }
    }
}