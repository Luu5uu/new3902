using System.Collections.Generic;
using Celeste;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Input
{
    public class ControllerLoader
    {
        private readonly List<IController> _controllers = new List<IController>();

        public ControllerLoader(Game1 game)
        {
            var keyboard = new KeyboardController();

            keyboard.RegisterCommand(Keys.Q, new QuitCommand(game));
            keyboard.RegisterCommand(Keys.R, new ResetCommand(game));

            keyboard.RegisterCommand(Keys.T, new CycleBlockCommand(game, -1));
            keyboard.RegisterCommand(Keys.Y, new CycleBlockCommand(game, 1));

            keyboard.RegisterCommand(Keys.U, new CycleItemCommand(game, -1));
            keyboard.RegisterCommand(Keys.I, new CycleItemCommand(game, 1));

            keyboard.RegisterCommand(Keys.B, new ToggleBlockAnimationCommand(game));

            keyboard.RegisterCommand(Keys.V, new ToggleBlockDisplayCommand(game));

            _controllers.Add(keyboard);
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
