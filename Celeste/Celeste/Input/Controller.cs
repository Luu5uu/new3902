using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace Celeste.Input
{
    public enum MouseButton {Left, Middle, Right}
    public class KeyboardController : IController
    {
        private readonly Dictionary<Keys, ICommand> _pressedMappings = new Dictionary<Keys, ICommand>();
        private readonly Dictionary<Keys, ICommand> _heldMappings = new Dictionary<Keys, ICommand>();
        private KeyboardState _previousState;

        public KeyboardController()
        {
            _previousState = Keyboard.GetState();
        }

        public void RegisterCommand(Keys key, ICommand command, bool continuous = false)
        {
            if (continuous)
            {
                _heldMappings[key] = command;
            }
            else
            {
                _pressedMappings[key] = command;
            }
        }

        public void Update()
        {
            KeyboardState currentState = Keyboard.GetState();

            foreach (var mapping in _heldMappings)
            {
                if (currentState.IsKeyDown(mapping.Key))
                {
                    mapping.Value.Execute();
                }
            }

            foreach (var mapping in _pressedMappings)
            {
                if (currentState.IsKeyDown(mapping.Key) && !_previousState.IsKeyDown(mapping.Key))
                {
                    mapping.Value.Execute();
                }
            }
            _previousState = currentState;
        }
    }

    public class MouseController : IController
    {

        private readonly Dictionary<MouseButton, ICommand> _pressedMappings = new Dictionary<MouseButton, ICommand>();
        private MouseState _previousState;

        public MouseController()
        {
            _previousState = Mouse.GetState();
        }
        public void RegisterCommand(MouseButton button, ICommand command)
        {
            _pressedMappings[button] = command;
        }
        public void Update()
        {
            MouseState currentState = Mouse.GetState();

            foreach (var mapping in _pressedMappings)
            {
                bool isDownNow = IsButtonDown(currentState, mapping.Key);
                bool wasDownBefore = IsButtonDown(_previousState, mapping.Key);

                if (isDownNow && !wasDownBefore)
                {
                    mapping.Value.Execute();
                }
            }
            _previousState = currentState;
        }

        private bool IsButtonDown(MouseState state, MouseButton button)
        {
            return button switch
            {
                MouseButton.Left => state.LeftButton == ButtonState.Pressed,
                MouseButton.Middle => state.MiddleButton == ButtonState.Pressed,
                MouseButton.Right => state.RightButton == ButtonState.Pressed,
                _ => false
            };
        }
    }

    public class GamepadController : IController
    {
        private readonly Dictionary<Buttons, ICommand> _pressedMappings = new Dictionary<Buttons, ICommand>();
        private readonly Dictionary<Buttons, ICommand> _heldMappings = new Dictionary<Buttons, ICommand>();
        private GamePadState _previousState;
        public GamepadController()
        {
            _previousState = GamePad.GetState(PlayerIndex.One);
        }
        public void RegisterCommand(object input, ICommand command, bool continuous = false )
        {
            if (continuous)
            {
                _heldMappings[(Buttons)input] = command;
            }
            else
            {
                _pressedMappings[(Buttons)input] = command;
            }
        }

        public void Update()
        {
            GamePadState currentState = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
            if (!currentState.IsConnected) return;

            foreach (var mapping in _heldMappings)
            {
                if (currentState.IsButtonDown(mapping.Key))
                {
                    mapping.Value.Execute();
                }
            }

            foreach (var mapping in _pressedMappings)
            {
                if (currentState.IsButtonDown(mapping.Key) && !_previousState.IsButtonDown(mapping.Key))
                {
                    mapping.Value.Execute();
                }
            }
            _previousState = currentState;
        }
    }
}
