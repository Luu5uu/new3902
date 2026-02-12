Animation Pack Usage Guide
The animation system is split into two responsibilities:
AnimationLoader --- Loads sprite strips from Content and builds animation data.
AnimationClip --- Stores pure animation metadata (no playback logic).
Gameplay code (YOU) --- Handles animation playback, switching, and drawing.
The animation pack does not manage animation states or drawing automatically.
All playback logic must be implemented in gameplay classes.

0th step - Make sure use the new pack:
    using Celeste.Animation;

1st step - Load the Animation Pack (Once in LoadContent())
In game1.cs:

    using Celeste.Animation;
    private AnimationCatalog _anims = null!;
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load all animation strips from Content
        _anims = AnimationLoader.LoadAll(Content);
    }
This call will:
Loads all sprite strips
Automatically computes frame counts
Stores everything inside a dictionary



2nd step - Pass AnimationCatalog to Your Character Class
Your gameplay class (Player, Enemy, etc.) should receive the catalog.
Eaxmple:
    private readonly AnimationCatalog _catalog;
    public Player(AnimationCatalog catalog)
    {
        _catalog = catalog;
    }

Then create your object from Game1:
    _player = new Player(_anims);



3rd step - Retrieve an AnimationClip
Example:
    AnimationClip runClip = _catalog.Clips[AnimationKeys.PlayerRun];
or directly use string key
    AnimationClip idleClip = _catalog.Clips["Player/Idle"];



4th step - Implement Playback Logic

5th step - Draw the Current Frame