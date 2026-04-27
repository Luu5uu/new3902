using Celeste.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Scenes
{
    public class PauseScene : Scene
    {
        private readonly string[] _options = { "RESUME", "RESTART ROOM", "QUIT TO MENU", "EXIT GAME" };
        private SpriteFont _font;
        private Texture2D _pixel;
        private KeyboardController _keyboard;
        private GamepadController _gamepad;
        private int _selectedIndex;

        public PauseScene(Game1 game) : base(game)
        {
            BlocksUpdate = true;
            BlocksDraw = false;
        }

        public override void LoadContent()
        {
            _font = Game.Content.Load<SpriteFont>("MenuFont");
            _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _keyboard = new KeyboardController();
            _gamepad = new GamepadController();

            InputMapper.ConfigureMenu(
                _keyboard,
                _gamepad,
                onUp: () => _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : _options.Length - 1,
                onDown: () => _selectedIndex = (_selectedIndex + 1) % _options.Length,
                onSelect: ExecuteSelection,
                onBack: () => SceneManager.PopScene(),
                onQuit: () => Game.Exit());
        }

        public override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            _gamepad.Update();
            Game.Window.Title = $"Celeste - Paused | BGM: {Game1.GetBgmStatusText()}";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(
                _pixel,
                new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                Color.Black * 0.6f);

            spriteBatch.DrawString(_font, "PAUSED", new Vector2(100, 100), Color.White, 0f, Vector2.Zero, 1.6f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "UP / DOWN TO CHOOSE   ENTER TO CONFIRM", new Vector2(100, 150), Color.LightGray);
            spriteBatch.DrawString(_font, "ESC TO RESUME   Q TO EXIT GAME", new Vector2(100, 180), Color.LightGray);

            for (int i = 0; i < _options.Length; i++)
            {
                bool isSelected = i == _selectedIndex;
                Vector2 position = new Vector2(isSelected ? 130 : 100, 250 + (i * 56));

                if (isSelected)
                {
                    spriteBatch.DrawString(_font, ">", position - new Vector2(30, 0), Color.Yellow);
                }

                spriteBatch.DrawString(_font, _options[i], position, isSelected ? Color.White : Color.Gray);
            }

            spriteBatch.End();
        }

        private void ExecuteSelection()
        {
            switch (_selectedIndex)
            {
                case 0:
                    SceneManager.PopScene();
                    break;
                case 1:
                    SceneManager.PopScene();
                    SceneManager.PushScene(new ScreenWipeScene(Game, () =>
                    {
                        Game.Reset();
                    }));
                    break;
                case 2:
                    SceneManager.PushScene(new ScreenWipeScene(Game, () =>
                    {
                        SceneManager.ChangeScene(new MainMenuScene(Game));
                    }));
                    break;
                case 3:
                    Game.Exit();
                    break;
            }
        }
    }
}
