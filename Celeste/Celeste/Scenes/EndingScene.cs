using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Scenes
{
    public class EndingScene : Scene
    {
        private readonly float _finalTime;
        private readonly int _deathCount;
        private readonly int _strawberryCount;

        private Texture2D _pixelTexture;
        private SpriteFont _font;

        private float _elapsed;
        private bool _returnStarted;
        private KeyboardState _previousKeyboardState;

        private const float FadeInDuration = 1.5f;
        private const float TitleStartTime = 1.0f;
        private const float StatsStartTime = 2.5f;
        private const float PromptStartTime = 4.5f;
        private const float AutoReturnTime = 9.0f;

        public EndingScene(
            Game1 game,
            float finalTime,
            int deathCount,
            int strawberryCount) : base(game)
        {
            _finalTime = finalTime;
            _deathCount = deathCount;
            _strawberryCount = strawberryCount;
            _previousKeyboardState = Keyboard.GetState();

            BlocksUpdate = true;
            BlocksDraw = true;
        }

        public override void LoadContent()
        {
            _font = Game.Content.Load<SpriteFont>("MenuFont");

            _pixelTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        public override void Update(GameTime gameTime)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;

            KeyboardState keyboard = Keyboard.GetState();

            bool enterPressed =
                keyboard.IsKeyDown(Keys.Enter) &&
                _previousKeyboardState.IsKeyUp(Keys.Enter);

            if (!_returnStarted && _elapsed >= PromptStartTime && enterPressed)
            {
                ReturnToMainMenu();
            }

            if (!_returnStarted && _elapsed >= AutoReturnTime)
            {
                ReturnToMainMenu();
            }

            _previousKeyboardState = keyboard;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int width = Game.GraphicsDevice.Viewport.Width;
            int height = Game.GraphicsDevice.Viewport.Height;

            float fadeAlpha = MathHelper.Clamp(_elapsed / FadeInDuration, 0f, 1f);
            float titleAlpha = MathHelper.Clamp((_elapsed - TitleStartTime) / 1.5f, 0f, 1f);
            float statsAlpha = MathHelper.Clamp((_elapsed - StatsStartTime) / 1.5f, 0f, 1f);
            float promptAlpha = MathHelper.Clamp((_elapsed - PromptStartTime) / 1.0f, 0f, 1f);

            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            spriteBatch.Draw(
                _pixelTexture,
                new Rectangle(0, 0, width, height),
                Color.Black * fadeAlpha);

            DrawCenteredText(
                spriteBatch,
                "You reached the summit.",
                new Vector2(width / 2f, height / 2f - 120f),
                Color.White * titleAlpha,
                2f);
            DrawCenteredText(
                spriteBatch,
                "Thanks for playing.",
                new Vector2(width / 2f, height / 2f - 55f),
                Color.White * titleAlpha,
                1f);

            DrawCenteredText(
                spriteBatch,
                $"Time: {FormatTime(_finalTime)}",
                new Vector2(width / 2f, height / 2f + 10f),
                Color.White * statsAlpha,
                1f);

            DrawCenteredText(
                spriteBatch,
                $"Deaths: {_deathCount}",
                new Vector2(width / 2f, height / 2f + 50f),
                Color.White * statsAlpha,
                1f);

            DrawCenteredText(
                spriteBatch,
                $"Strawberries: {_strawberryCount}",
                new Vector2(width / 2f, height / 2f + 90f),
                Color.White * statsAlpha,
                1f);

            DrawCenteredText(
                spriteBatch,
                "Press Enter to return to menu",
                new Vector2(width / 2f, height - 80f),
                Color.White * promptAlpha,
                1f);

            spriteBatch.End();
        }

        private void ReturnToMainMenu()
        {
            _returnStarted = true;

            SceneManager.PushScene(new ScreenWipeScene(Game, () =>
            {
                SceneManager.ChangeScene(new MainMenuScene(Game));
            }));
        }

        private void DrawCenteredText(
            SpriteBatch spriteBatch,
            string text,
            Vector2 center,
            Color color,
            float scale)
        {
            Vector2 textSize = _font.MeasureString(text) * scale;
            Vector2 position = center - textSize / 2f;

            spriteBatch.DrawString(
                _font,
                text,
                position,
                color,
                0f,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0f);
        }

        private string FormatTime(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int milliseconds = (int)((time * 100) % 100);

            return $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";
        }
    }
}
