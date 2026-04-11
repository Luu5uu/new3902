using System;
using System.Collections.Generic;
using System.Linq;
using Celeste.Scenes;
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

        public KeyboardController GetKeyboard() => _controllers.OfType<KeyboardController>().FirstOrDefault();
        public GamepadController GetGamepad() => _controllers.OfType<GamepadController>().FirstOrDefault();

        public ControllerLoader(Game1 game, Character.Madeline player)
        {
            _player = player;
           // _previousKeyboardState = Keyboard.GetState();
           // _previousGamepadState = GamePad.GetState(PlayerIndex.One);

            var keyboard = new KeyboardController();
            var mouse = new MouseController();
            var gamepad = new GamepadController();
            
            _controllers.Add(keyboard);
            _controllers.Add(mouse);
            _controllers.Add(gamepad);
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

    public static class InputMapper
    {
        public static void ConfigureMenu(KeyboardController kb, GamepadController gp, Action onUp, Action onDown, Action onSelect, Action onBack = null)
        {
            kb.RegisterCommand(Keys.Up, new ActionCommand(onUp));
            kb.RegisterCommand(Keys.Down, new ActionCommand(onDown));
            kb.RegisterCommand(Keys.Enter, new ActionCommand(onSelect));
            if (onBack != null) kb.RegisterCommand(Keys.Escape, new ActionCommand(onBack));

            if (gp != null)
            {
                gp.RegisterCommand(Buttons.DPadUp, new ActionCommand(onUp));
                gp.RegisterCommand(Buttons.DPadDown, new ActionCommand(onDown));
                gp.RegisterCommand(Buttons.A, new ActionCommand(onSelect));
            }
        }

        public static void ConfigureGameplay(KeyboardController kb, Game1 game, GameplayScene scene)
        {
            kb.RegisterCommand(Keys.R, new ResetCommand(game));
            kb.RegisterCommand(Keys.Escape, new PauseCommand(game));
            kb.RegisterCommand(Keys.Q, new QuitCommand(game));

            // Room Hotkeys
            kb.RegisterCommand(Keys.D1, new ActionCommand(() => scene.JumpToRoom(1)));
            kb.RegisterCommand(Keys.D2, new ActionCommand(() => scene.JumpToRoom(2)));
            kb.RegisterCommand(Keys.D3, new ActionCommand(() => scene.JumpToRoom(3)));
        }
    }
}
