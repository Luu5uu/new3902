using Celeste;
using Microsoft.Xna.Framework;

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

    public class PlayerJumpCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerJumpCommand(Character.Madeline player) => _player = player;
        public void Execute() => _player.Jump();
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

    public class PlayerClimbCommand : ICommand
    {
        private readonly Character.Madeline _player;

        public PlayerClimbCommand(Character.Madeline player) => _player = player;

        public void Execute() => _player.SetClimb(true);
    }

    

}
