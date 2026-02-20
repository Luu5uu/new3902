using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Celeste;

public class ControllerLoader
{
    private List<IController> controllers;
    public ControllerLoader(Game1 game)
    {
        controllers = new List<IController>();
        var keyboard = new KeyboardController();

        keyboard.RegisterCommand(Keys.Q, new QuitCommand(game));
        keyboard.RegisterCommand(Keys.R, new ResetCommand(game));

        keyboard.RegisterCommand(Keys.T, new CycleBlockCommand(game, -1));
        keyboard.RegisterCommand(Keys.Y, new CycleBlockCommand(game, 1));

        keyboard.RegisterCommand(Keys.U, new CycleItemCommand(game, -1));
        keyboard.RegisterCommand(Keys.I, new CycleItemCommand(game, 1));

        controllers.Add(keyboard);
    }

    public void Update()
    {
        foreach (var controller in controllers)
        {
            controller.Update();
        }
    }
}
