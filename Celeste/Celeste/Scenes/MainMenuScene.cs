using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Celeste.Input;

namespace Celeste.Scenes
{
    public class MainMenuScene : Scene
    {
        private SpriteFont _font;
        private int _selectedIndex = 0;
        private readonly string[] _menuOptions = { "CLIMB", "OPTIONS", "CREDITS", "EXIT" };
        
        private KeyboardController _keyboard;
        private GamepadController _gamepad;
       // private Texture2D _background; // need the main menu background
        
        public MainMenuScene(Game1 game) : base(game) { }

        public override void LoadContent()
        {
            
            _font = Game.Content.Load<SpriteFont>("MenuFont");
            
            // todo _background = Game.Content.Load<Texture2D>("MenuBackground");
            _keyboard = new KeyboardController();
            _gamepad = new GamepadController();

            InputMapper.ConfigureMenu(_keyboard, null, 
                onUp: () => _selectedIndex = (_selectedIndex > 0) ? _selectedIndex - 1 : _menuOptions.Length - 1,
                onDown: () => _selectedIndex = (_selectedIndex + 1) % _menuOptions.Length,
                onSelect: ExecuteSelection);
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
                case 0: // CLIMB
                    SceneManager.ChangeScene(new GameplayScene(Game));
                    break;
                case 1: // OPTIONS
                    // Todo: SceneManager.PushScene(new OptionsScene(Game));
                    break;
                case 3: // EXIT
                    Game.Exit();
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // 1. Draw Background (Celeste often uses a dark blue gradient or mountain)
            // spriteBatch.Draw(_background, Vector2.Zero, Color.White);

            // 2. Draw Menu Options
            Vector2 startPos = new Vector2(100, 300); // Todo: Adjust based on resolution
            
            for (int i = 0; i < _menuOptions.Length; i++)
            {
                Color textColor = (i == _selectedIndex) ? Color.Yellow : Color.White;
                float scale = (i == _selectedIndex) ? 1.2f : 1.0f; // Slight pop for selected item
                
                // Add small selection indicator like the Celeste strawberry or bird
                string prefix = (i == _selectedIndex) ? "> " : "  ";

                spriteBatch.DrawString(
                    _font, 
                    prefix + _menuOptions[i], 
                    startPos + new Vector2(0, i * 50), 
                    textColor,
                    0f,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            spriteBatch.End();
        }
    }
}