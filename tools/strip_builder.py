#!/usr/bin/env python3
"""
strip_builder.py - Combine individual frame PNGs into horizontal sprite strips.

Reads animation folders from Spriters Resource dumps (or any folder of
individually numbered frame PNGs) and combines them into horizontal strips
compatible with MonoGame's AutoAnimation.Detect() system.

Usage:
    python tools/strip_builder.py <input_dir> <output_dir> [--mgcb <mgcb_file>]

Examples:
    # Build Madeline body strips from the downloaded sprite dump:
    python tools/strip_builder.py \
        Celeste/Celeste/Madeline_Assets/Animations/player \
        Celeste/Celeste/Content

    # Build enemy strips (when someone downloads those):
    python tools/strip_builder.py \
        Celeste/Celeste/Enemy_Assets/Animations/seeker \
        Celeste/Celeste/Content

    # Also update Content.mgcb automatically:
    python tools/strip_builder.py \
        Celeste/Celeste/Madeline_Assets/Animations/player \
        Celeste/Celeste/Content \
        --mgcb Celeste/Celeste/Content/Content.mgcb

The input_dir should contain subdirectories, each representing one animation.
Each subdirectory should contain frames named f0.png, f1.png, ... or
name00.png, name01.png, etc. GIF files are ignored.

The output is one horizontal-strip PNG per animation, named after the
subdirectory (e.g., idle.png, runFast.png, dash.png).
"""

import argparse
import os
import re
import sys

try:
    from PIL import Image
except ImportError:
    print("ERROR: Pillow is required. Install it with:  pip install Pillow")
    sys.exit(1)


def natural_sort_key(filename):
    """Sort filenames with embedded numbers in natural order.
    e.g., f0, f1, f2, ..., f9, f10, f11 (not f0, f1, f10, f11, f2, ...)
    """
    return [
        int(part) if part.isdigit() else part.lower()
        for part in re.split(r'(\d+)', filename)
    ]


def collect_frames(anim_dir):
    """Collect and sort all PNG frame files from an animation directory."""
    frames = []
    for f in os.listdir(anim_dir):
        if f.lower().endswith('.png'):
            frames.append(f)

    # Sort naturally so f2 comes before f10
    frames.sort(key=natural_sort_key)
    return frames


def build_strip(anim_dir, frame_files):
    """Open all frame PNGs and combine them into a single horizontal strip."""
    images = []
    for f in frame_files:
        path = os.path.join(anim_dir, f)
        img = Image.open(path).convert("RGBA")
        images.append(img)

    if not images:
        return None

    # Use the maximum dimensions across all frames so no frame is clipped.
    # (e.g., bangs00 is 8x7 but bangs02 is 8x8 -- the strip should be 8-tall.)
    frame_w = max(img.width for img in images)
    frame_h = max(img.height for img in images)

    # Create the horizontal strip.
    strip = Image.new("RGBA", (frame_w * len(images), frame_h), (0, 0, 0, 0))

    for i, img in enumerate(images):
        # Paste at top-left of the frame slot; shorter/narrower frames
        # are padded with transparent pixels (no resizing needed).
        strip.paste(img, (i * frame_w, 0))

    return strip


def build_mgcb_entry(filename):
    """Generate a Content.mgcb entry for a texture PNG."""
    return f"""
#begin {filename}
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:{filename}
"""


def update_mgcb(mgcb_path, new_files):
    """Add entries for new files to Content.mgcb if they don't already exist."""
    with open(mgcb_path, 'r') as f:
        content = f.read()

    added = []
    for filename in sorted(new_files):
        marker = f"#begin {filename}"
        if marker not in content:
            content += build_mgcb_entry(filename)
            added.append(filename)

    if added:
        with open(mgcb_path, 'w') as f:
            f.write(content)

    return added


def main():
    parser = argparse.ArgumentParser(
        description="Combine individual frame PNGs into horizontal sprite strips."
    )
    parser.add_argument(
        "input_dir",
        help="Directory containing animation subdirectories (e.g., Animations/player)"
    )
    parser.add_argument(
        "output_dir",
        help="Directory to write horizontal strip PNGs to (e.g., Content/)"
    )
    parser.add_argument(
        "--mgcb",
        help="Path to Content.mgcb to auto-update with new texture entries",
        default=None
    )
    parser.add_argument(
        "--prefix",
        help="Optional prefix for output filenames (e.g., 'player_' -> player_idle.png)",
        default=""
    )
    parser.add_argument(
        "--only",
        help="Comma-separated list of animation names to process (default: all)",
        default=None
    )

    args = parser.parse_args()

    if not os.path.isdir(args.input_dir):
        print(f"ERROR: Input directory not found: {args.input_dir}")
        sys.exit(1)

    os.makedirs(args.output_dir, exist_ok=True)

    # Determine which animations to process.
    only_set = None
    if args.only:
        only_set = set(name.strip() for name in args.only.split(","))

    new_files = []
    total_frames = 0

    # Iterate over subdirectories in the input directory.
    entries = sorted(os.listdir(args.input_dir))
    for entry in entries:
        anim_dir = os.path.join(args.input_dir, entry)
        if not os.path.isdir(anim_dir):
            continue

        if only_set and entry not in only_set:
            continue

        frames = collect_frames(anim_dir)
        if not frames:
            print(f"  SKIP {entry:30s}  (no PNG frames found)")
            continue

        strip = build_strip(anim_dir, frames)
        if strip is None:
            continue

        out_name = f"{args.prefix}{entry}.png"
        out_path = os.path.join(args.output_dir, out_name)
        strip.save(out_path, "PNG")

        frame_w = strip.height  # Single frame is square-ish, height = frame height
        n_frames = len(frames)
        total_frames += n_frames
        new_files.append(out_name)

        print(f"  OK   {entry:30s}  {n_frames:3d} frames  "
              f"({strip.width}x{strip.height})  -> {out_name}")

    print(f"\nGenerated {len(new_files)} strips ({total_frames} total frames) "
          f"in {args.output_dir}")

    # Optionally update Content.mgcb.
    if args.mgcb and new_files:
        added = update_mgcb(args.mgcb, new_files)
        if added:
            print(f"Added {len(added)} new entries to {args.mgcb}:")
            for f in added:
                print(f"  + {f}")
        else:
            print(f"Content.mgcb already up to date.")


if __name__ == "__main__":
    main()
