using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    public class ControllerLoader
    {
        private readonly List<IController> _controllers = new List<IController>();
        private readonly Character.Madeline _player;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamepadState;

        public ControllerLoader(Game1 game, Character.Madeline player)
        {
            _player = player;
            _previousKeyboardState = Keyboard.GetState();
            _previousGamepadState = GamePad.GetState(PlayerIndex.One);

            var keyboard = new KeyboardController();
            var mouse = new MouseController();

            //keyboard commands registration
            keyboard.RegisterCommand(Keys.Q, new QuitCommand(game));
            keyboard.RegisterCommand(Keys.Escape, new QuitCommand(game));
            keyboard.RegisterCommand(Keys.R, new ResetCommand(game));

            //mouse commands registration
            mouse.RegisterCommand(MouseButton.Left, new CycleGameSceneCommand(game, -1));
            mouse.RegisterCommand(MouseButton.Right, new CycleGameSceneCommand(game, 1));

            _controllers.Add(keyboard);
            _controllers.Add(mouse);
        }

        public void Update()
        {
            UpdatePlayerInput();
            foreach (var controller in _controllers)
            {
                controller.Update();
            }
        }

        private void UpdatePlayerInput()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);

            float horizontal = 0f;
            if (IsKeyboardDown(keyboardState, Keys.Left, Keys.A) || IsGamepadDown(gamepadState, Buttons.DPadLeft, Buttons.LeftThumbstickLeft))
            {
                horizontal -= 1f;
            }
            if (IsKeyboardDown(keyboardState, Keys.Right, Keys.D) || IsGamepadDown(gamepadState, Buttons.DPadRight, Buttons.LeftThumbstickRight))
            {
                horizontal += 1f;
            }

            float vertical = 0f;
            if (IsKeyboardDown(keyboardState, Keys.Up, Keys.W) || IsGamepadDown(gamepadState, Buttons.DPadUp))
            {
                vertical -= 1f;
            }
            if (IsKeyboardDown(keyboardState, Keys.Down, Keys.S) || IsGamepadDown(gamepadState, Buttons.DPadDown))
            {
                vertical += 1f;
            }

            bool jumpHeld = keyboardState.IsKeyDown(Keys.C) || gamepadState.IsButtonDown(Buttons.A);
            bool jumpPressed = IsKeyboardPressed(keyboardState, Keys.C) || IsGamepadPressed(gamepadState, Buttons.A);
            bool dashPressed = IsKeyboardPressed(keyboardState, Keys.X) || IsGamepadPressed(gamepadState, Buttons.B);
            bool grabHeld = keyboardState.IsKeyDown(Keys.Z) || gamepadState.IsButtonDown(Buttons.RightShoulder);
            bool deathPressed = IsKeyboardPressed(keyboardState, Keys.E);

            _player.Move(MathHelper.Clamp(horizontal, -1f, 1f));
            _player.AimVertical(MathHelper.Clamp(vertical, -1f, 1f));
            _player.SetJumpHeld(jumpHeld);
            _player.SetClimb(grabHeld);

            if (jumpPressed)
            {
                _player.Jump();
            }

            if (dashPressed)
            {
                _player.Dash();
            }

            if (deathPressed)
            {
                _player.Die();
            }

            _previousKeyboardState = keyboardState;
            _previousGamepadState = gamepadState;
        }

        private bool IsKeyboardDown(KeyboardState state, Keys primary, Keys alternate)
        {
            return state.IsKeyDown(primary) || state.IsKeyDown(alternate);
        }

        private bool IsKeyboardPressed(KeyboardState state, Keys key)
        {
            return state.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        private bool IsGamepadDown(GamePadState state, Buttons primary, Buttons? alternate = null)
        {
            return state.IsButtonDown(primary) || (alternate.HasValue && state.IsButtonDown(alternate.Value));
        }

        private bool IsGamepadPressed(GamePadState state, Buttons button)
        {
            return state.IsButtonDown(button) && !_previousGamepadState.IsButtonDown(button);
        }
    }
}
