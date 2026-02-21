General ReadMe for the full project, each person can detail their objectives, struggles, and future To-Dos.

Group name: Strawberry
Group members: Aaron, Henry, Isaac, Albert, Sihao, Zijun

Sprint 2: Due Feb 21
Grader CheckIn: Monday, Feb 16
In Class Code Check: Friday, Feb 20

---

## Controls

- **Move:** A / D or Left / Right arrows
- **Jump:** Space
- **Dash:** Enter, Z, or N
- **Death:** T   (For Test & Show only)
- **Quit:** Escape (or Back on gamepad) or Q
- **Restart:** R
- **Cycle Block Forward & Backward:** Y & T
- **Cycle Item Forward & Backward:** U & I
- **Debug overlay (G):** G toggles; P pause, Tab/Backspace cycle animation, arrows step frame, W/S/A/D nudge hair anchor, C crosshair. See `Celeste/Animation/README.md` for full debug keys.

---

## Known bugs

---

## Tools

- **strip_builder.py** — Builds horizontal sprite strips from frame PNGs (e.g. `Madeline_Assets/Animations/player/idleD/` → `Content/idleD.png`). Requires Pillow; run via venv if system Python is externally managed.
- **Animation system** — `Celeste/Animation/README.md` documents the catalog, loader, and debug mode.

---

## Tasks assigned to individuals

Aaron: Controller functionality and Command functionality to seperate controls from specific objects.

Albert: Finish the death animation 

Henry: 

Flip static hair animations to dynamic (jump, dash, idleA-idleD)


Isaac: 

Draw the blocks and "animate" the moving ones.

Sihao: Finish dashState, climbState and related modification.

Zijun: Finish Character, runState, jumpState, fallState, dangleState classes.

Sprint 3: Due Mar 14
Tasks assigned to individuals: TBD
