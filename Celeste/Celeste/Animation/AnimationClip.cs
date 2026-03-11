using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    /// <summary>
    /// Pure animation data for a clip inside a sprite sheet (atlas).
    /// No playback state.
    /// </summary>
    public sealed class AnimationClip
    {
        public Texture2D Texture { get; init; } = null!;

        public int StartX { get; init; }
        public int StartY { get; init; }

        public int FrameWidth { get; init; }
        public int FrameHeight { get; init; }
        public int FrameCount { get; init; }

        public int FramesPerRow { get; init; }  // must be >= 1

        public float Fps { get; init; }
        public bool Loop { get; init; }

        public Rectangle GetSourceRect(int frameIndex)
        {
            if (FrameCount <= 0) throw new InvalidOperationException("FrameCount must be > 0.");
            if (frameIndex < 0 || frameIndex >= FrameCount)
                throw new ArgumentOutOfRangeException(nameof(frameIndex));

            int fpr = FramesPerRow > 0 ? FramesPerRow : FrameCount;

            int col = frameIndex % fpr;
            int row = frameIndex / fpr;

            int x = StartX + col * FrameWidth;
            int y = StartY + row * FrameHeight;

            return new Rectangle(x, y, FrameWidth, FrameHeight);
        }
    }
}