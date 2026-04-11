using System;
using Microsoft.Xna.Framework.Input;
using Celeste.Scenes;

namespace Celeste.Input
{
    public static class InputConfig
    {
        
        public static void ConfigureMenu(KeyboardController kb, GamepadController gp, 
            Action onUp, Action onDown, Action onSelect, Action onBack = null)
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
                if (onBack != null) gp.RegisterCommand(Buttons.B, new ActionCommand(onBack));
            }
        }

        
        public static void ConfigureGameplay(KeyboardController kb, Game1 game)
        {
            // Core System Commands
            kb.RegisterCommand(Keys.R, new ResetCommand(game));
            kb.RegisterCommand(Keys.Escape, new PauseCommand(game));
            kb.RegisterCommand(Keys.Q, new QuitCommand(game));

            // Room Hotkeys (Moving logic out of GameplayScene.Update)
            kb.RegisterCommand(Keys.D1, new ActionCommand(() => LoadRoom(1)));
            kb.RegisterCommand(Keys.D2, new ActionCommand(() => LoadRoom(2)));
            kb.RegisterCommand(Keys.D3, new ActionCommand(() => LoadRoom(3)));
        }

        private static void LoadRoom(int roomNumber)
        {
            if (SceneManager.ActiveScene is GameplayScene gameplay)
            {
                gameplay.JumpToRoom(roomNumber);
            }
        }
    }
}