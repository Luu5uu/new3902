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
    private Game1 game;
    private Dictionary<int, ICommand> quadrantMappings;
    private ICommand rightClickCommand;
    private MouseState previousState;

    public MouseController(Game1 game, ICommand rightClick)
    {
        this.game = game;
        this.rightClickCommand = rightClick;
        this.quadrantMappings = new Dictionary<int, ICommand>();
        this.previousState = Mouse.GetState();
    }

    public void RegisterCommand(int quadrant, ICommand command)
    {
        quadrantMappings[quadrant] = command;
    }

    public void Update()
    {
        MouseState currentState = Mouse.GetState();

        if (currentState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released)
        {
            rightClickCommand.Execute();
        }

        if (currentState.LeftButton == ButtonState.Pressed && previousState.LeftButton == ButtonState.Released)
        {
            int quadrant = GetQuadrant(currentState.Position);
            if (quadrantMappings.ContainsKey(quadrant))
            {
                quadrantMappings[quadrant].Execute();
            }
        }
        previousState = currentState;
    }

        private int GetQuadrant(Point mousePos)
    {
        float midX = game.GraphicsDevice.Viewport.Width * .5f;
        float midY = game.GraphicsDevice.Viewport.Height * .5f;

        if (mousePos.X < midX)
        {
            return (mousePos.Y < midY) ? 1 : 3; // Top-left or Bottom-left
        }
        else
        {
            return (mousePos.Y < midY) ? 2 : 4; // Top-right or Bottom-right
        }
    }
    
}