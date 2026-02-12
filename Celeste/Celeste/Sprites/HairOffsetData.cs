using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Sprites
{
    /// <summary>
    /// Stores per-animation, per-frame hair anchor offsets.
    ///
    /// Each entry is an (x, y) pixel offset from the top-left of the
    /// 32x32 body frame to the top-center of Madeline's head.
    ///
    /// Derived by pixel bounding-box analysis of the hairless sprite strips.
    /// </summary>
    public static class HairOffsetData
    {
        private static readonly Dictionary<string, Vector2[]> Offsets = new()
        {
            // ----- idle.png: single frame (f9 = basic standing pose) -----
            // Crown top at y=20, x=[15,19], cx=17. Using x=16 to match
            // the run offset that's confirmed working.
            ["idle"] = new[]
            {
                new Vector2(16, 20)
            },

            // ----- runFast.png: 12 frames -----
            // Crown center is at x=15-16 across all frames (pixel analysis).
            ["run"] = new[]
            {
                new Vector2(16, 20), new Vector2(16, 21), new Vector2(16, 21),
                new Vector2(16, 21), new Vector2(16, 19), new Vector2(16, 20),
                new Vector2(15, 21), new Vector2(16, 21), new Vector2(16, 21),
                new Vector2(16, 21), new Vector2(16, 19), new Vector2(16, 20)
            },

            // ----- dash.png: 4 frames -----
            ["dash"] = new[]
            {
                new Vector2(16, 21), new Vector2(16, 21),
                new Vector2(16, 21), new Vector2(16, 22)
            },

            // ----- jumpfast.png: 2 frames -----
            ["jumpfast"] = new[]
            {
                new Vector2(15, 19), new Vector2(15, 19)
            },

            // ----- fallSlow.png: 2 frames -----
            ["fallslow"] = new[]
            {
                new Vector2(15, 20), new Vector2(15, 20)
            },

            // ----- climbup.png: 6 frames -----
            ["climbup"] = new[]
            {
                new Vector2(16, 20), new Vector2(16, 20), new Vector2(16, 20),
                new Vector2(16, 20), new Vector2(15, 20), new Vector2(16, 20)
            },

            // ----- dangling.png: 10 frames -----
            ["dangling"] = new[]
            {
                new Vector2(16, 20), new Vector2(16, 20), new Vector2(16, 20),
                new Vector2(16, 20), new Vector2(16, 20), new Vector2(16, 20),
                new Vector2(16, 20), new Vector2(16, 20), new Vector2(16, 20),
                new Vector2(16, 20)
            },

            ["standard"] = new[]
            {
                new Vector2(14, 20)
            }
        };

        /// <summary>
        /// Gets the hair anchor offset for a given animation and frame.
        /// </summary>
        public static Vector2 GetOffset(string animationName, int frameIndex)
        {
            string key = animationName.ToLowerInvariant();

            if (!Offsets.TryGetValue(key, out Vector2[] offsets))
                return new Vector2(14, 20);

            if (frameIndex >= offsets.Length)
                return offsets[offsets.Length - 1];

            return offsets[frameIndex];
        }
    }
}
