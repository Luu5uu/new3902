using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    /// <summary>
    /// Pure animation data for a horizontal sprite strip.
    /// This type contains no playback state (no currentFrame, no timers).
    /// Gameplay systems can implement their own animator/player using this data.
    /// </summary>
    /// <author> Albert Liu </author>
    public sealed class AnimationClip
    {
        /// <summary>The underlying sprite strip texture (frames laid out horizontally).</summary>
        public Texture2D Texture { get; init; } = null!;

        /// <summary>Width of one frame in pixels.</summary>
        public int FrameWidth { get; init; }

        /// <summary>Height of one frame in pixels.</summary>
        public int FrameHeight { get; init; }

        /// <summary>Total frames in the strip.</summary>
        public int FrameCount { get; init; }

        /// <summary>Playback FPS metadata (consumer decides how to use it).</summary>
        public float Fps { get; init; }

        /// <summary>Loop metadata (consumer decides how to use it).</summary>
        public bool Loop { get; init; }

        /// <summary>
        /// Gets the source rectangle for a specific frame index.
        /// </summary>
        public Rectangle GetSourceRect(int frameIndex)
        {
            if (FrameCount <= 0)
                throw new System.InvalidOperationException("FrameCount must be > 0.");

            // Allow consumers to mod/clamp externally; here we just validate range.
            if (frameIndex < 0 || frameIndex >= FrameCount)
                throw new System.ArgumentOutOfRangeException(nameof(frameIndex), $"frameIndex {frameIndex} out of range [0, {FrameCount - 1}].");

            return new Rectangle(frameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
        }
    }
}