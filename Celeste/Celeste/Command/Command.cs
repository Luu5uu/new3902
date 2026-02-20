using Celeste;
using Microsoft.Xna.Framework;
public class QuitCommand : ICommand
{
    private Game1 _game;
    public QuitCommand(Game1 game) => _game = game;
    public void Execute() => _game.Exit();
}

public class ResetCommand : ICommand
{
    private Game1 _game;
    public ResetCommand(Game1 game) => _game = game;
    public void Execute()
    {
        
    }
}

public class CycleBlockCommand : ICommand
{
    private Game1 _game;
    private int _direction;
    public CycleBlockCommand(Game1 game, int direction)
    {
        _game = game;
        _direction = direction;
    }
    public void Execute() => _game.CycleActiveBlock(_direction);
}

public class CycleItemCommand : ICommand
{
    private Game1 _game;
    private int _direction;
    public CycleItemCommand(Game1 game, int direction)
    {
        _game = game;
        _direction = direction;
    }
    public void Execute() => _game.CycleActiveItem(_direction);
}
