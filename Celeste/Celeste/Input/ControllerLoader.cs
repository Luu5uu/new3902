using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    public class ControllerLoader
    {
        private readonly List<IController> _controllers = new List<IController>();

        public ControllerLoader(Game1 game, Character.Madeline player)
        {
            var keyboard = new KeyboardController();
            var mouse = new MouseController();
            var gamepad = new GamepadController();
            
            //keyboard commands registration
            keyboard.RegisterCommand(Keys.Q,      new QuitCommand(game));
            keyboard.RegisterCommand(Keys.Escape, new QuitCommand(game));
            keyboard.RegisterCommand(Keys.R, new ResetCommand(game));
            keyboard.RegisterCommand(Keys.A, new PlayerMoveCommand(player, -1f), continuous: true);
            keyboard.RegisterCommand(Keys.D, new PlayerMoveCommand(player, 1f), continuous: true);
            keyboard.RegisterCommand(Keys.Space, new PlayerJumpCommand(player));
            keyboard.RegisterCommand(Keys.Enter, new PlayerDashCommand(player));
            keyboard.RegisterCommand(Keys.Z, new PlayerDashCommand(player));
            keyboard.RegisterCommand(Keys.N, new PlayerDashCommand(player));
            keyboard.RegisterCommand(Keys.E, new PlayerDeathCommand(player));
            keyboard.RegisterCommand(Keys.W, new PlayerClimbCommand(player), continuous: true);

            //mouse commands registration
            mouse.RegisterCommand(MouseButton.Left , new CycleGameSceneCommand(game, -1));
            mouse.RegisterCommand(MouseButton.Right, new CycleGameSceneCommand(game, 1));

            //gamepad commands registration
            gamepad.RegisterCommand(Buttons.DPadLeft, new PlayerMoveCommand(player, -1f), continuous: true);
            gamepad.RegisterCommand(Buttons.LeftThumbstickLeft, new PlayerMoveCommand(player, -1f), continuous: true);
            gamepad.RegisterCommand(Buttons.DPadRight, new PlayerMoveCommand(player, 1f), continuous: true);
            gamepad.RegisterCommand(Buttons.LeftThumbstickRight, new PlayerMoveCommand(player, 1f), continuous: true);
            gamepad.RegisterCommand(Buttons.A, new PlayerJumpCommand(player));
            gamepad.RegisterCommand(Buttons.B, new PlayerDashCommand(player));
            
            _controllers.Add(keyboard);
            _controllers.Add(mouse);
            _controllers.Add(gamepad);
        }

        public void Update()
        {
            foreach (var controller in _controllers)
            {
                controller.Update();
            }
        }
    }
}
