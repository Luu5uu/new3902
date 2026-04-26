using Celeste.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using BgmAudioPlayer = Celeste.BGMPlayer.BGMPlayer;

namespace Celeste.Scenes
{
    public class MainMenuScene : Scene
    {
        private readonly string[] _menuOptions = { "CLIMB", "EXIT" };
        private SpriteFont _font;
        private Texture2D _background;
        private KeyboardController _keyboard;
        private GamepadController _gamepad;
        private int _selectedIndex;

        public MainMenuScene(Game1 game) : base(game)
        {
        }

        public override void LoadContent()
        {
            _font = Game.Content.Load<SpriteFont>("MenuFont");
            _background = Game.Content.Load<Texture2D>("bg");
            _keyboard = new KeyboardController();
            _gamepad = new GamepadController();
            EnsureMenuBgm();

            InputMapper.ConfigureMenu(
                _keyboard,
                _gamepad,
                onUp: () => _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : _menuOptions.Length - 1,
                onDown: () => _selectedIndex = (_selectedIndex + 1) % _menuOptions.Length,
                onSelect: ExecuteSelection,
                onQuit: () => Game.Exit());
        }

        public override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            _gamepad.Update();
            EnsureMenuBgm();
            Game.Window.Title = $"Celeste - Main Menu | BGM: {Game1.GetBgmStatusText()}";
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(_background, Game.GraphicsDevice.Viewport.Bounds, Color.White);
            spriteBatch.DrawString(_font, "CELESTE", new Vector2(100, 140), Color.White, 0f, Vector2.Zero, 2.2f, SpriteEffects.None, 0f);
            spriteBatch.DrawString(_font, "UP / DOWN TO CHOOSE", new Vector2(100, 220), Color.LightGray);
            spriteBatch.DrawString(_font, "ENTER TO CONFIRM   Q / ESC TO EXIT", new Vector2(100, 250), Color.LightGray);

            Vector2 startPosition = new Vector2(100, 330);
            for (int i = 0; i < _menuOptions.Length; i++)
            {
                bool isSelected = i == _selectedIndex;
                string prefix = isSelected ? "> " : "  ";
                Color color = isSelected ? Color.Yellow : Color.White;
                float scale = isSelected ? 1.2f : 1f;

                spriteBatch.DrawString(
                    _font,
                    prefix + _menuOptions[i],
                    startPosition + new Vector2(0, i * 54),
                    color,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            spriteBatch.End();
        }

        private void ExecuteSelection()
        {
            switch (_selectedIndex)
            {
                case 0:
                    SceneManager.ChangeScene(new GameplayScene(Game));
                    break;
                case 1:
                    Game.Exit();
                    break;
            }
        }

        private static void EnsureMenuBgm()
        {
            MediaPlayer.IsRepeating = true;
            if (!string.Equals(BgmAudioPlayer.CurrentTrackName, "prologue", System.StringComparison.OrdinalIgnoreCase))
            {
                BgmAudioPlayer.bgmSwitchTo("prologue");
            }
            else
            {
                BgmAudioPlayer.bgmPlay();
            }
        }
    }
}
