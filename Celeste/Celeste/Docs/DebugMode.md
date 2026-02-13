# Debug Mode (Hair Anchor Tuning)

Debug mode lets you tune per-frame hair anchor offsets and bangs without recompiling, and inspect any animation.

## Enabling

- Press **G** to toggle the debug overlay (only works in composite mode).
- Press **H** to switch between composite (body + procedural hair) and legacy rendering if needed.

## Keys (when overlay is on)

| Key | Action |
|-----|--------|
| **G** | Toggle debug overlay |
| **C** | Toggle crosshair and feet dot |
| **P** | Pause / resume animation |
| **Left / Right** | Step one frame backward / forward (while paused) |
| **Tab / Shift+Tab** | Cycle to next / previous animation (while paused) |
| **W A S D** | Nudge hair anchor delta by 1 pixel: W/S = Y, A/D = X (while paused) |
| **F** | Toggle idle vs idleA when not moving |

## On-screen info

The window title bar (and console when frame or animation changes) shows:

- Current animation name and frame index  
- Stored delta (from `HairOffsetData`)  
- Live nudge (WASD)  
- Effective delta (stored + nudge)  
- Anchor position and facing (L/R)  
- `PAUSED` when animation is paused  

## Tuning hair on an existing animation

1. Enable debug (**G**).
2. Pause (**P**).
3. Use **Tab** / **Shift+Tab** to cycle to the animation you want (through all animations in the catalog).
4. Step to the desired frame (**Left** / **Right**).
5. Optionally hide the crosshair (**C**) if it blocks the view.
6. Nudge with **WASD** until the hair lines up with the head.
7. Note the **effective (X,Y)** from the console or title bar.
8. Put that value into `HairOffsetData.cs` for that animation and frame.

## Adding a new animation with dynamic hair

1. Add the strip (e.g. via `tools/strip_builder.py`) and get it into Content (and, if using the catalog, into `AnimationLoader` + `AnimationKeys`).
2. Register the animation in `MaddySprite` (and in the debug Tab-order list).
3. Add a placeholder row in `HairOffsetData` (e.g. all `(0,0)`) and a row in `BangsFrameData` (e.g. all `0`).
4. Use debug mode: switch to the new animation (**Tab**), step through each frame (**Left** / **Right**), nudge with **WASD**, and copy the printed effective deltas into `HairOffsetData`.
5. If the head turns (e.g. idle head-turn), adjust bangs frame indices in `BangsFrameData` (0 = right, 1 = center, 2 = left).
