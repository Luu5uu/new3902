using System.Collections.Generic;

namespace Celeste.Sprites
{
    /// <summary>
    /// Stores per-animation, per-frame bangs frame indices.
    ///
    /// The bangs texture strip has three frames:
    ///   0 = right-facing bangs (default when sprite faces right)
    ///   1 = center / neutral bangs (transition frame)
    ///   2 = left-facing bangs (head turned left in native sprite)
    ///
    /// When the global facing direction flips (faceLeft = true), the
    /// bangs sprite is drawn with SpriteEffects.FlipHorizontally,
    /// so the frame indices remain relative to the animation's native
    /// facing direction (right).
    ///
    /// Derived from pixel analysis of head direction in each animation.
    /// </summary>
    public static class BangsFrameData
    {
        private static readonly Dictionary<string, int[]> Frames = new()
        {
            // ----- idle.png: single frame (basic standing pose) -----
            ["idle"] = new[] { 0 },

            // ----- runFast.png: 12 frames -----
            // Head stays in default forward-facing position throughout.
            ["run"] = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },

            // ----- dash.png: 4 frames -----
            ["dash"] = new[] { 0, 0, 0, 0 },

            // ----- jumpfast.png: 2 frames -----
            ["jumpfast"] = new[] { 0, 0 },

            // ----- fallSlow.png: 2 frames -----
            ["fallslow"] = new[] { 0, 0 },

            // ----- climbup.png: 6 frames -----
            ["climbup"] = new[] { 0, 0, 0, 0, 0, 0 },

            // ----- dangling.png: 10 frames -----
            ["dangling"] = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        /// <summary>
        /// Gets the bangs frame index for a given animation and frame.
        /// Returns 0 (default right-facing) if the animation is unknown.
        /// </summary>
        public static int GetFrame(string animationName, int frameIndex)
        {
            string key = animationName.ToLowerInvariant();

            if (!Frames.TryGetValue(key, out int[] frames))
                return 0;

            if (frameIndex >= frames.Length)
                return frames[frames.Length - 1];

            return frames[frameIndex];
        }
    }
}
