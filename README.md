# Team Strawberry — Celeste

**Group:** Strawberry
**Members:** Aaron, Henry, Isaac, Albert, Sihao, Zijun
**Framework:** C# · MonoGame 3.8 · .NET 8.0

---

## Overview

This project is a Celeste-inspired MonoGame platformer focused on tight movement, room-based progression, collectibles, rewind, and polished feedback effects. Sprint 5 finalized the main movement polish pass: wall jump/kick behavior, dash particles, climb stamina feedback, room transitions, spring activation, landing/wall-kick dust, and FlyFeather / StarFly movement.

The final gameplay rules emphasize Celeste-like readability:

- Dash is spent when used and refills from landing, collectibles/refill items, and feather collection.
- Grabbing or touching a wall does not refill dash.
- Holding grab while touching a wall blocks dashes aimed directly into that wall to avoid clipping.
- Same-wall jump spam is blocked, while alternating between left and right wall surfaces remains valid for climbing.
- Rewind restores player state, movement timers, dash state, and visual trail context.

---

## Controls

### Player Controls

| Key(s) | Action |
|--------|--------|
| `←` / `→` | Move left / right |
| `W` / `A` / `S` / `D` | Alternate movement / aim input |
| `↑` / `↓` | Aim dash vertically / climb up or down while grabbing / crouch while grounded |
| `Z` (hold) | Grab / climb |
| `X` | Dash |
| `C` | Jump |
| `C` while sliding on wall | Wall jump / kick off the wall |
| `V` | Rewind |
| `E` | Trigger death sequence (test / demo only) |

### Gamepad Controls

| Input | Action |
|-------|--------|
| Left stick / D-pad | Move and aim |
| `A` | Jump |
| `B` | Dash |
| Right shoulder | Grab / climb |
| `Y` | Rewind |
| Start | Pause |

### Room / Testing Controls

| Input | Action |
|-------|--------|
| `0`-`6` / numpad `0`-`6` | Jump directly to a room for testing |

### BGM Controls

| Key(s) | Action |
|--------|--------|
| `F5` | Previous BGM track |
| `F6` | Next BGM track |
| `F7` | Pause BGM |
| `F8` | Resume BGM |

### Other Controls

| Key(s) | Action |
|--------|--------|
| `Q` | Quit |
| `Escape` | Open pause menu during gameplay / go back in menus |
| `R` | Reset the current room |
| `T` | Toggle HUD elements |

### Menu Controls

| Key(s) | Action |
|--------|--------|
| `Up` / `Down` | Move menu selection |
| `Enter` | Confirm current menu option |
| `Escape` | Back out of pause menu / exit from main menu |

### Debug Mode Controls

Debug mode is for developer use only (hair anchor tuning, animation inspection).

| Key | Action |
|-----|--------|
| `G` | Toggle debug overlay |
| `P` | Pause / resume animation |
| `→` / `←` | Step one frame forward / backward (while paused) |
| `Tab` | Cycle to next animation (while paused) |
| `Backspace` | Cycle to previous animation (while paused) |
| `W` / `S` / `A` / `D` | Nudge hair anchor up / down / left / right 1 px (while paused) |
| `C` | Toggle crosshair overlay |

> Full debug workflow documented in [Celeste/Celeste/Animation/README.md](Celeste/Celeste/Animation/README.md) and [Celeste/Celeste/DevTools/DebugMode.md](Celeste/Celeste/DevTools/DebugMode.md).

---

## Known Bugs

Known bugs, playtest notes, task delegation, and review follow-ups are tracked in the team Discord server.

---

## Tools

- **[tools/strip_builder.py](tools/strip_builder.py)** — Converts per-frame PNG folders into horizontal sprite strips for MonoGame's animation system (e.g. `Madeline_Assets/Animations/player/idleD/` → `Content/idleD.png`). Requires Pillow; see [tools/README.md](tools/README.md) for usage.
- **Animation system** — Data-driven catalog + clip + controller. See [Celeste/Celeste/Animation/README.md](Celeste/Celeste/Animation/README.md) for full documentation and debug mode workflow.

---

## File Structure

Current layout after the organizational refactor (moves/renames only; no functionality changes):

```
Celeste/Celeste/
  Core/
    Constants/
      GlobalConstants.cs     # Global scale and display dimensions
      PlayerConstants.cs     # Player frame size, movement, physics
      HairConstants.cs       # Hair rendering, colors, and anchor tuning
      ItemConstants.cs       # Item display positions
      BlockConstants.cs      # Block display position and spring trigger tuning
      LevelConstants.cs      # Room ranges, transitions, respawns, rewind timings
      BackgroundConstants.cs # Snow/background tuning
      DeathConstants.cs      # Death animation and particle tuning
    GameLoopInterfaces.cs    # IUpdateable, IDrawable
  Character/
    Madeline.cs              # Player entity (sprite, state machine, physics, death)
    Death/                   # Death sequence (sprite + particles + orbit ring)
      DeathEffect.cs, DeathSpritePlayer.cs, ClipPlayer.cs, OrbitRingEffect.cs
      Utils/Easings.cs
      Particles/             # ProceduralParticleTexture, ParticleSystem, Particle, Emitters/
  MadelineStates/            # Player state machine (stand, run, jump, fall, dash, climb, dangle, death)
  Input/
    Controller.cs            # Keyboard, mouse, and gamepad controller implementations
    ICommand.cs, GameCommands.cs, IController.cs, ControllerLoader.cs
  Animation/                 # Catalog, clips, controller, loader, keys
  Sprites/                   # MaddySprite, BodySprite, HairRenderer, hair/bangs data
  Items/                     # ItemAnimation, ItemAnimator, ItemAnimationFactory, CollectibleItem
  Blocks/                    # IBlocks, Blocks, CrushBlock, MoveBlock, Spring, BlockFactory, AllBlocks
  DevTools/                  # DebugOverlay.cs
  Game1.cs, Program.cs
tools/
  strip_builder.py
  README.md
```

**Dependency overview:**

```mermaid
flowchart TB
  Game1[Game1]
  Core[Core]
  Character[Character]
  Input[Input]
  MadelineStates[MadelineStates]
  Animation[Animation]
  Sprites[Sprites]
  Death[Character/Death]
  Items[Items]
  Blocks[Blocks]
  DevTools[DevTools]

  Game1 --> Core
  Game1 --> Character
  Game1 --> Input
  Game1 --> Animation
  Game1 --> Items
  Game1 --> Blocks
  Game1 --> DevTools

  Character --> Core
  Character --> Input
  Character --> MadelineStates
  Character --> Animation
  Character --> Sprites
  Character --> Death

  MadelineStates --> Character

  Input --> Game1
```
