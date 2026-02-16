# strip_builder — Animation strip generator

Converts per-frame PNGs (e.g. from [Spriters Resource](https://www.spriters-resource.com/)) into **one horizontal strip PNG per animation** for MonoGame’s animation system. Single strip per animation is the current, scalable approach.

## Requirements

- Python 3  
- Pillow: `pip install Pillow`

## Input layout

- **input_dir**: A directory that contains **one subdirectory per animation** (e.g. `idle`, `runFast`, `idleA`).
- Inside each subdir: frame images named `f0.png`, `f1.png`, … or `name00.png`, `name01.png`, etc.  
- GIF files are ignored.  
- Frames are sorted in **natural order** (e.g. f2 before f10).

## Output

- One PNG per animation in **output_dir**, named after the subdir (e.g. `idle/` → `idle.png`).
- Each strip = frames laid out left to right. Frame size = max width and max height over all frames in that animation (so nothing is clipped).

## Usage

```bash
python tools/strip_builder.py <input_dir> <output_dir>
```

### Examples

Build Madeline body strips from the sprite dump into Content:

```bash
python tools/strip_builder.py \
  Celeste/Celeste/Madeline_Assets/Animations/player \
  Celeste/Celeste/Content
```

Also update `Content.mgcb` with new texture entries:

```bash
python tools/strip_builder.py \
  Celeste/Celeste/Madeline_Assets/Animations/player \
  Celeste/Celeste/Content \
  --mgcb Celeste/Celeste/Content/Content.mgcb
```

### Options

| Option | Description |
|--------|-------------|
| `--mgcb <path>` | Append `#begin`/`/build` entries to Content.mgcb for newly written PNGs. |
| `--prefix <string>` | Prefix for output filenames (e.g. `player_` → `player_idle.png`). |
| `--only <list>` | Comma-separated list of animation names to process (default: all). |

## Pipeline

1. **Raw frames** (one folder per animation, e.g. `f0.png`, `f1.png`, …).  
2. **strip_builder** → one horizontal strip PNG per animation.  
3. **Content** — add strips to MonoGame Content (and `Content.mgcb` if needed).  
4. **AnimationLoader / AnimationKeys** (optional) — register strips in the catalog.  
5. **Game** — use in MaddySprite and tune hair with debug mode (see `Celeste/Celeste/Docs/DebugMode.md`).
