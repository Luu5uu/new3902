Animation Pack Usage Guide

---

## Debug Mode (Hair Calibration)

Press **G** to toggle the debug overlay. The window title updates live with animation and anchor info.

| Key | Action |
|-----|--------|
| G | Toggle debug overlay (closing resets hair nudge and unpauses) |
| P | Pause / resume animation |
| → / ← | Step one frame forward / backward (while paused) |
| Tab | Cycle to next animation (while paused) |
| Backspace | Cycle to previous animation (while paused) |
| W / S / A / D | Nudge hair anchor up / down / left / right 1px (while paused) |
| C | Toggle crosshair overlay |

The lime crosshair marks the current hair anchor. The yellow dot marks the character's feet.
Window title format: `[animName fN] stored=(x,y) nudge=(x,y) eff=(x,y) anchor=(x,y) L/R [PAUSED]`

---

## Current setup (what we use)

- **Default path (catalog):** Game1 loads the catalog with `AnimationLoader.LoadAll(Content)` and passes it to `MaddySprite.Build(Content, _catalog, GraphicsDevice)`. All body animations and procedural hair use this **data-driven system** (AnimationLoader + AnimationCatalog + AnimationClip + AnimationKeys).
- **Legacy path (H toggle):** Press **H** to switch to **static hair** — manually drawn strips. Legacy uses the **same catalog**: `PlayerAnimations.Build(_catalog)` registers only clips whose keys end with `*_static_hair` (see `AnimationKeys.PlayerIdleStaticHair`, etc.). If no `*_static_hair` assets are in Content, H still works and the default (Maddy) is drawn.

**Legacy assets:** Rename old manually-drawn-hair strips to `*_static_hair` (e.g. `idle_static_hair.png`, `run_static_hair.png`) and add them to Content. The loader loads them optionally; after this sprint you can remove the `*_static_hair` assets and the legacy path entirely.

---

The animation system is split into two responsibilities:
AnimationLoader --- Loads sprite strips from Content and builds animation data.
AnimationClip --- Stores pure animation metadata (no playback logic).
Gameplay code (YOU) --- Handles animation playback, switching, and drawing.
The animation pack does not manage animation states or drawing automatically.
All playback logic must be implemented in gameplay classes.

0th step - Make sure use the new pack:
    using Celeste.Animation;

1st step - Load the Animation Pack (Once in LoadContent()) in game1.cs:
```csharp
    using Celeste.Animation;
    private AnimationCatalog _anims = null!;
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // Load all animation strips from Content
        _anims = AnimationLoader.LoadAll(Content);
    }
```
This call will:
Loads all sprite strips
Automatically computes frame counts
Stores everything inside a dictionary



2nd step - Pass AnimationCatalog to Your Character Class
Your gameplay class (Player, Enemy, etc.) should receive the catalog.
Example
In your class.cs:
```csharp
    private readonly AnimationCatalog _catalog;
    public Player(AnimationCatalog catalog)
    {
        _catalog = catalog;
    }
```
Then create your object from 
Game1.cs:
```csharp
    _player = new Player(_anims);
```


3rd step - Retrieve an AnimationClip
Use the keys in `AnimationKeys` (preferred) or the same string literals:
```csharp
    AnimationClip runClip = _catalog.Clips[AnimationKeys.PlayerRun];
```

or by string key (same values as in AnimationKeys):
```csharp
    AnimationClip idleClip = _catalog.Clips["Player/Idle"];
```

4th step - Implement Playback Logic

5th step - Draw the Current Frame

Example to show hows the dictionary store the data:
```csharp
Dictionary<string, AnimationClip>
    └── "Player/Run"
            └── AnimationClip
                    ├── Texture → run.png
                    ├── FrameWidth = 32
                    ├── FrameCount = 8
                    ├── Fps = 12
                    └── Loop = true
```