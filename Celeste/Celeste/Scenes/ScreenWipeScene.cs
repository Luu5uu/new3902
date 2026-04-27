using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Scenes
{
    public class ScreenWipeScene : Scene
    {
        private Action _onScreenCovered;
        private Texture2D _pixel;
        private float _progress;
        private const float WipeDuration = .4f;
        private const int Columns = 12;

        public ScreenWipeScene(Game1 game, Action onScreenCovered) : base(game)
        {
            _onScreenCovered = onScreenCovered;
            BlocksUpdate = true;
            BlocksDraw = false;
        }

        public override void LoadContent()
        {
            _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.Black });
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _progress += dt / WipeDuration;

            if (_progress >= 1.5f)
            {
                    _onScreenCovered?.Invoke();

                    if (SceneManager.ActiveScene == this)
                    {
                        SceneManager.PopScene();
                    }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            Rectangle bounds = Game.GraphicsDevice.Viewport.Bounds;
            float columnWidth = (float)bounds.Width / Columns;

            for (int i = 0; i < Columns; i++)
            {
                float delay = (float)i / Columns * .5f;
                float localProgress = MathHelper.Clamp((_progress - delay) / .5f, 0f, 1f);

                int height = (int)(bounds.Height * localProgress);
                spriteBatch.Draw(_pixel, new Rectangle((int)(i * columnWidth), 0, (int)columnWidth + 1, height), Color.Black);
            }

            spriteBatch.End();
        }
    }
}