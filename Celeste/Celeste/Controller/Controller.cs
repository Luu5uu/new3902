using System.Collections.Generic;
using System.Drawing;
using System.Windows.Input;
using Celeste;
using Microsoft.Xna.Framework.Input;

public class KeyboardController: IController
{
    private Dictionary<Keys, ICommand> controllerMappings;
    private KeyboardState previousState;

    public KeyboardController()
    {
        controllerMappings = new Dictionary<Keys, ICommand>();
        previousState = Keyboard.GetState();
    }

    public void RegisterCommand(Keys key, ICommand command)
    {
        controllerMappings[key] = command;
    }

    public void Update()
    {
        KeyboardState currentState = Keyboard.GetState();
        Keys[] pressedKeys = currentState.GetPressedKeys();

        foreach (Keys key in pressedKeys)
        {
            if (controllerMappings.ContainsKey(key) && !previousState.IsKeyDown(key))
            {
                controllerMappings[key].Execute();
            }
        }
        previousState = currentState;
    }
}

public class MouseController : IController
{
   public void Update()
    {
        // Implement mouse input handling
    }
}