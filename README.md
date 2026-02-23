General ReadMe for the full project, each person can detail their objectives, struggles, and future To-Dos.

Group name: Strawberry
Group members: Aaron, Henry, Isaac, Albert, Sihao, Zijun

Sprint 2: Due Feb 21
Grader Check-In: Monday, Feb 16
In Class Code Check: (Pushed to Monday, Feb 23rd)

---

## Controls

- **Move:** A / D or Left / Right arrows
- **Jump:** Space
- **Dash:** Enter, Z, or N
- **Death:** E   (For Test & Show only)
- **Quit:** Escape (or Back on gamepad) or Q
- **Restart:** R
- **Cycle Block Forward & Backward:** Y & T
- **Cycle Item Forward & Backward:** U & I
- **Debug overlay (G):** G toggles; P pause, Tab/Backspace cycle animation, arrows step frame, W/S/A/D nudge hair anchor, C crosshair. See `Celeste/Animation/README.md` for full debug keys.

---

## Known Bugs

C:\Users\yuanc\Desktop\CSE 3902\new3902\new3902\Celeste\Celeste\Game1.cs(7,15): error CS0234: The namespace "Celeste" does not contain a type or namespace named "Debug" (is there a missing assembly reference?)
C:\Users\yuanc\Desktop\CSE 3902\new3902\new3902\Celeste\Celeste\Game1.cs(26,17): error CS0246: The type or namespace named "DebugOverlay" could not be found (is there a missing using directive or assembly reference?) 
Generation failed. Please fix the generation error and run again.

General Monogame Content Editor Mac Error:
Ensure the version of Monogame written in **dotnet-tools.json** matches the version installed on your device.

---

## Tools

- **strip_builder.py** — Builds horizontal sprite strips from frame PNGs (e.g. `Madeline_Assets/Animations/player/idleD/` → `Content/idleD.png`). Requires Pillow; run via venv if system Python is externally managed.
- **Animation system** — `Celeste/Animation/README.md` documents the catalog, loader, and debug mode.

---

## Tasks Assigned to Individuals

Aaron: Controller Functionality 

Albert: Death Animations and main animation process.

Henry: Flip static hair animations to dynamic (jump, dash, idleA-idleD)

Isaac:  Draw the blocks and animate the moving ones. Swap through different blocks using T & Y keys.

Sihao: Finish dashSrate, climbState and related modification.

Zijun: Finish Character, standState, jumpState, runState, fallState and dangleState.


Sprint 3: Due Mar 14
Tasks assigned to individuals: TBD
