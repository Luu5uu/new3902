using Celeste.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Scenes
{
    public class PauseScene : Scene
    {
        private SpriteFont _font;
        private Texture2D _pixel;
        private int _selectedIndex = 0;
        private readonly string[] _options = { "RESUME", "RESTART ROOM", "OPTIONS", "QUIT TO MENU" };
        
        private KeyboardController _keyboard;
        private GamepadController _gamepad;

        public PauseScene(Game1 game) : base(game)
        {
            BlocksUpdate = true; // Stop the game logic
            BlocksDraw = false;  // Keep drawing the game in the background
        }

        public override void LoadContent()
        {
            // Use the same font as the Main Menu
            _font = Game.Content.Load<SpriteFont>("MenuFont");
            
            // Create a 1x1 white pixel for the background darkening
            _pixel = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            _keyboard = new KeyboardController();
            _gamepad = new GamepadController();

            InputMapper.ConfigureMenu(_keyboard, null,
                onUp: () => _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : _options.Length - 1,
                onDown: () => _selectedIndex = (_selectedIndex + 1) % _options.Length,
                onSelect: ExecuteSelection,
                onBack: () => SceneManager.PopScene());
        }

        public override void Update(GameTime gameTime)
        {
            _keyboard.Update();
            _gamepad.Update();
        }

        private void ExecuteSelection()
        {
            switch (_selectedIndex)
            {
                case 0: // resume
                    SceneManager.PopScene();
                    break;
                case 1: // restart
                    SceneManager.PopScene();
                    Game.Reset();
                    break;
                case 3: // quit
                    SceneManager.ChangeScene(new MainMenuScene(Game));
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();

            // Darken the background
            spriteBatch.Draw(_pixel, 
                new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height), 
                Color.Black * 0.6f);

            // 2. Draw Menu Title
            spriteBatch.DrawString(_font, "PAUSED", new Vector2(100, 100), Color.Gray * 0.8f);

            // 3. Draw Options
            for (int i = 0; i < _options.Length; i++)
            {
                bool isSelected = (i == _selectedIndex);
                Color color = isSelected ? Color.White : Color.Gray;
                
                // If selected, move it slightly to the right (sliding effect)
                float xPos = isSelected ? 120 : 100;
                Vector2 position = new Vector2(xPos, 200 + (i * 60));

                // Draw a small selection bar next to selected item
                if (isSelected)
                {
                    spriteBatch.DrawString(_font, ">", position - new Vector2(30, 0), Color.Yellow);
                }

                spriteBatch.DrawString(_font, _options[i], position, color);
            }

            spriteBatch.End();
        }
    }
}