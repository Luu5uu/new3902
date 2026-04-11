using System;
using Celeste.Scenes;

namespace Celeste.Input
{
    public class QuitCommand : ICommand
    {
        private readonly Game1 _game;

        public QuitCommand(Game1 game) => _game = game;

        public void Execute() => _game.Exit();
    }

    public class ResetCommand : ICommand
    {
        private readonly Game1 _game;

        public ResetCommand(Game1 game) => _game = game;

        public void Execute() => _game.Reset();
    }

    public class PauseCommand : ICommand
    {
        private readonly Game1 _game;

        public PauseCommand(Game1 game)
        {
            _game = game;
        }

        public void Execute()
        {
            if (SceneManager.ActiveScene is GameplayScene)
            {
                SceneManager.PushScene(new PauseScene(_game));
            }
        }
    }

    public class ActionCommand : ICommand
    {
        private readonly Action _action;

        public ActionCommand(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action?.Invoke();
        }
    }

    public class CycleGameSceneCommand : ICommand
    {
        private readonly Game1 _game;
        private readonly int _direction;

        public CycleGameSceneCommand(Game1 game, int direction)
        {
            _game = game;
            _direction = direction;
        }

        public void Execute() => _game.CycleGameScene(_direction);
    }

    public class PreviousBgmCommand : ICommand
    {
        private readonly Game1 _game;

        public PreviousBgmCommand(Game1 game) => _game = game;

        public void Execute() => _game.PlayPreviousBgm();
    }

    public class NextBgmCommand : ICommand
    {
        private readonly Game1 _game;

        public NextBgmCommand(Game1 game) => _game = game;

        public void Execute() => _game.PlayNextBgm();
    }

    public class PauseBgmCommand : ICommand
    {
        private readonly Game1 _game;

        public PauseBgmCommand(Game1 game) => _game = game;

        public void Execute() => _game.PauseBgm();
    }

    public class ResumeBgmCommand : ICommand
    {
        private readonly Game1 _game;

        public ResumeBgmCommand(Game1 game) => _game = game;

        public void Execute() => _game.ResumeBgm();
    }

    public class PlayerMoveCommand : ICommand
    {
        private readonly Character.Madeline _player;
        private readonly float _direction;

        public PlayerMoveCommand(Character.Madeline player, float direction)
        {
            _player = player;
            _direction = direction;
        }

        public void Execute() => _player.Move(_direction);
    }

    public class PlayerAimVerticalCommand : ICommand
    {
        private readonly Character.Madeline _player;
        private readonly float _direction;

        public PlayerAimVerticalCommand(Character.Madeline player, float direction)
        {
            _player = player;
            _direction = direction;
        }

        public void Execute() => _player.AimVertical(_direction);
    }

    public class PlayerJumpCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerJumpCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.Jump();
    }

    public class PlayerJumpHoldCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerJumpHoldCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.SetJumpHeld(true);
    }

    public class PlayerDashCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerDashCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.Dash();
    }

    public class PlayerDeathCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerDeathCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.Die();
    }

    public class PlayerGrabCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerGrabCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.SetClimb(true);
    }
}
