using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Sprites
{
    /// <summary>
    /// Stores per-animation, per-frame hair anchor deltas.
    ///
    /// Each entry is a small (x, y) delta from the base head position.
    /// The base head position is computed as:
    ///     feetPos + (0, -BaseHeadY * scale)
    /// where BaseHeadY = 12 native pixels up from the center-bottom origin.
    ///
    /// Positive X = rightward, Positive Y = downward.
    /// For left-facing, the caller negates X (matching Celeste's
    /// HairOffset * (Facing, 1) approach).
    ///
    /// Format matches the original Celeste metadata:
    ///     hair="0,-2|0,-2|0,-2|0,-2|0,-1|0,-1|0,-1|0,-1|0,-1"
    /// </summary>
    public static class HairOffsetData
    {
        private static readonly Dictionary<string, Vector2[]> Deltas = new()
        {
            // Deltas derived from top-2-row crown center analysis of each strip.
            // Positive X = rightward (toward face in the right-facing sprite).
            // When facing left, the caller negates X automatically.

            // ----- idle.png: single frame (f9 = basic standing pose) -----
            // Crown cx=16.5 → delta 0
            ["idle"] = new[]
            {
                new Vector2(0, 0)
            },

            // ----- idleA.png: 12 frames (head-turn fidget) -----
            // Hand-tuned with debug frame-stepping. Head turns left then
            // back: negative X = hair shifts left as head turns.
            ["idlea"] = new[]
            {
                new Vector2( 0, 0),  // f0  neutral
                new Vector2(-1, 0),  // f1  head starting to turn
                new Vector2(-2, 0),  // f2  head turned further
                new Vector2(-1, 0),  // f3  head easing back
                new Vector2(-1, 0),  // f4  hold
                new Vector2(-1, 1),  // f5  slight dip
                new Vector2(-1, 1),  // f6  hold dip
                new Vector2(-1, 1),  // f7  hold dip
                new Vector2(-2, 1),  // f8  head turned + dip
                new Vector2(-1, 1),  // f9  easing back
                new Vector2( 0, 1),  // f10 nearly neutral + dip
                new Vector2( 0, 1),  // f11 nearly neutral + dip
            },

            // ----- runFast.png: 12 frames -----
            // Hand-tuned with debug frame-stepping. Madeline's body
            // bobs vertically during the run cycle: Y=+1 when the body
            // dips, Y=-1 when it rises, Y=0 at neutral.
            ["run"] = new[]
            {
                new Vector2(1, 0),  // f0  neutral
                new Vector2(1, 1),  // f1  body dips
                new Vector2(1, 1),  // f2  body dips
                new Vector2(1, 1),  // f3  body dips
                new Vector2(1,-1),  // f4  body rises
                new Vector2(1, 0),  // f5  neutral
                new Vector2(1, 1),  // f6  body dips
                new Vector2(1, 1),  // f7  body dips
                new Vector2(1, 1),  // f8  body dips
                new Vector2(1, 1),  // f9  body dips
                new Vector2(1,-1),  // f10 body rises
                new Vector2(1, 0),  // f11 neutral
            },

            // ----- dash.png: 4 frames -----
            // Crown cx=18.5, delta +2
            ["dash"] = new[]
            {
                new Vector2(2, 1), new Vector2(2, 1),
                new Vector2(2, 1), new Vector2(2, 2)
            },

            // ----- jumpfast.png: 2 frames -----
            // Crown cx=17.5, delta +2
            ["jumpfast"] = new[]
            {
                new Vector2(2, -1), new Vector2(2, -1)
            },

            // ----- fallSlow.png: 2 frames -----
            // Crown cx=15.5-16.5, delta 0
            ["fallslow"] = new[]
            {
                new Vector2(0, 0), new Vector2(0, 0)
            },

            // ----- climbup.png: 6 frames -----
            // Crown cx=17.0 first 4 frames, 16.0 last 2
            ["climbup"] = new[]
            {
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 0)
            },

            // ----- dangling.png: 10 frames -----
            // Crown cx=17.5 first 4 frames, 17.0 last 6
            ["dangling"] = new[]
            {
                new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0),
                new Vector2(2, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0)
            },

            ["standard"] = new[]
            {
                new Vector2(0, 0)
            }
        };

        /// <summary>
        /// Gets the hair delta for a given animation and frame.
        /// Returns a small offset from the base head position.
        /// </summary>
        public static Vector2 GetOffset(string animationName, int frameIndex)
        {
            string key = animationName.ToLowerInvariant();

            if (!Deltas.TryGetValue(key, out Vector2[] deltas))
                return Vector2.Zero;

            if (frameIndex >= deltas.Length)
                return deltas[deltas.Length - 1];

            return deltas[frameIndex];
        }
    }
}
