using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    public class KeyboardController : IController
    {
        private readonly Dictionary<Keys, ICommand> _controllerMappings = new Dictionary<Keys, ICommand>();
        private KeyboardState _previousState;

        public KeyboardController()
        {
            _previousState = Keyboard.GetState();
        }

        public void RegisterCommand(Keys key, ICommand command)
        {
            _controllerMappings[key] = command;
        }

        public void Update()
        {
            KeyboardState currentState = Keyboard.GetState();
            Keys[] pressedKeys = currentState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (_controllerMappings.ContainsKey(key) && !_previousState.IsKeyDown(key))
                {
                    _controllerMappings[key].Execute();
                }
            }
            _previousState = currentState;
        }
    }

    public class MouseController : IController
    {
        public void Update()
        {
            // Implement mouse input handling
        }
    }
}
