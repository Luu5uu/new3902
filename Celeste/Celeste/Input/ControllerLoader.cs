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
        private readonly List<IController> _controllers = new();
        private readonly Character.Madeline _player;
        private KeyboardState _previousKeyboardState;
        private GamePadState _previousGamepadState;
        public bool RewindPressedThisFrame { get; private set; }
        public ControllerLoader(Game1 game, Character.Madeline player)
        {
            _player = player;
            _previousKeyboardState = Keyboard.GetState();
            _previousGamepadState = GamePad.GetState(PlayerIndex.One);

            _controllers.Add(new KeyboardController());
            _controllers.Add(new MouseController());
            _controllers.Add(new GamepadController());
        }

        public KeyboardController GetKeyboard() => _controllers.OfType<KeyboardController>().FirstOrDefault();

        public MouseController GetMouse() => _controllers.OfType<MouseController>().FirstOrDefault();

        public GamepadController GetGamepad() => _controllers.OfType<GamepadController>().FirstOrDefault();

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
            bool rewindPressed = IsKeyboardPressed(keyboardState, Keys.V) || IsGamepadPressed(gamepadState, Buttons.Y);
            
            RewindPressedThisFrame = rewindPressed;

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

        private static bool IsKeyboardDown(KeyboardState state, Keys primary, Keys alternate)
        {
            return state.IsKeyDown(primary) || state.IsKeyDown(alternate);
        }

        private bool IsKeyboardPressed(KeyboardState state, Keys key)
        {
            return state.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }

        private static bool IsGamepadDown(GamePadState state, Buttons primary, Buttons? alternate = null)
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
        public static void ConfigureMenu(
            KeyboardController keyboard,
            GamepadController gamepad,
            Action onUp,
            Action onDown,
            Action onSelect,
            Action onBack = null,
            Action onQuit = null)
        {
            keyboard.RegisterCommand(Keys.Up, new ActionCommand(onUp));
            keyboard.RegisterCommand(Keys.Down, new ActionCommand(onDown));
            keyboard.RegisterCommand(Keys.Enter, new ActionCommand(onSelect));

            if (onBack != null)
            {
                keyboard.RegisterCommand(Keys.Escape, new ActionCommand(onBack));
            }
            else if (onQuit != null)
            {
                keyboard.RegisterCommand(Keys.Escape, new ActionCommand(onQuit));
            }

            if (onQuit != null)
            {
                keyboard.RegisterCommand(Keys.Q, new ActionCommand(onQuit));
            }

            if (gamepad != null)
            {
                gamepad.RegisterCommand(Buttons.DPadUp, new ActionCommand(onUp));
                gamepad.RegisterCommand(Buttons.DPadDown, new ActionCommand(onDown));
                gamepad.RegisterCommand(Buttons.A, new ActionCommand(onSelect));

                if (onBack != null)
                {
                    gamepad.RegisterCommand(Buttons.B, new ActionCommand(onBack));
                }
                else if (onQuit != null)
                {
                    gamepad.RegisterCommand(Buttons.B, new ActionCommand(onQuit));
                }
            }
        }

        public static void ConfigureGameplay(
            KeyboardController keyboard,
            MouseController mouse,
            GamepadController gamepad,
            Game1 game,
            GameplayScene scene)
        {
            keyboard.RegisterCommand(Keys.Q, new QuitCommand(game));
            keyboard.RegisterCommand(Keys.R, new ResetCommand(game));
            keyboard.RegisterCommand(Keys.Escape, new PauseCommand(game));

            RegisterRoomJumpCommands(keyboard, scene);

           /* if (mouse != null)
            {
                // commenting  this out for debugging

                mouse.RegisterCommand(MouseButton.Left, new CycleGameSceneCommand(game, -1));
                mouse.RegisterCommand(MouseButton.Right, new CycleGameSceneCommand(game, 1));
            }*/

            if (gamepad != null)
            {
                gamepad.RegisterCommand(Buttons.Start, new PauseCommand(game));
            }
        }

        private static void RegisterRoomJumpCommands(KeyboardController keyboard, GameplayScene scene)
        {
            keyboard.RegisterCommand(Keys.D0, new ActionCommand(() => scene.JumpToRoom(0)));
            keyboard.RegisterCommand(Keys.D1, new ActionCommand(() => scene.JumpToRoom(1)));
            keyboard.RegisterCommand(Keys.D2, new ActionCommand(() => scene.JumpToRoom(2)));
            keyboard.RegisterCommand(Keys.D3, new ActionCommand(() => scene.JumpToRoom(3)));
            keyboard.RegisterCommand(Keys.D4, new ActionCommand(() => scene.JumpToRoom(4)));
            keyboard.RegisterCommand(Keys.D5, new ActionCommand(() => scene.JumpToRoom(5)));
            keyboard.RegisterCommand(Keys.NumPad0, new ActionCommand(() => scene.JumpToRoom(0)));
            keyboard.RegisterCommand(Keys.NumPad1, new ActionCommand(() => scene.JumpToRoom(1)));
            keyboard.RegisterCommand(Keys.NumPad2, new ActionCommand(() => scene.JumpToRoom(2)));
            keyboard.RegisterCommand(Keys.NumPad3, new ActionCommand(() => scene.JumpToRoom(3)));
            keyboard.RegisterCommand(Keys.NumPad4, new ActionCommand(() => scene.JumpToRoom(4)));
            keyboard.RegisterCommand(Keys.NumPad5, new ActionCommand(() => scene.JumpToRoom(5)));
        }
    }
}
