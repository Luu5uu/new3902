using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Celeste.Sprites
{
    // Per-frame hair anchor deltas from the base head position.
    // Base head = feetPos + (0, -12 * scale). +X = right, +Y = down.
    // Caller negates X for left-facing (Celeste: offset.X * Facing).
    public static class HairOffsetData
    {
        private static readonly Dictionary<string, Vector2[]> Deltas = new()
        {
            // idleD: 9 frames, default standing. Calibrated.
            ["idled"] = new[]
            {
                new Vector2(0, 0),  // f0
                new Vector2(0, 0),  // f1
                new Vector2(0, 0),  // f2
                new Vector2(0, 0),  // f3
                new Vector2(0, 1),  // f4 — head dips 1px (breathing bob)
                new Vector2(0, 1),  // f5
                new Vector2(0, 1),  // f6
                new Vector2(0, 1),  // f7
                new Vector2(0, 1),  // f8
            },

            // idleA: 12 frames, head-turn fidget. Hand-tuned.
            ["idlea"] = new[]
            {
                new Vector2( 0, 0),  // f0
                new Vector2(-1, 0),  // f1
                new Vector2(-2, 0),  // f2
                new Vector2(-1, 0),  // f3
                new Vector2(-1, 0),  // f4
                new Vector2(-1, 1),  // f5
                new Vector2(-1, 1),  // f6
                new Vector2(-1, 1),  // f7
                new Vector2(-2, 1),  // f8
                new Vector2(-1, 1),  // f9
                new Vector2( 0, 1),  // f10
                new Vector2( 0, 1),  // f11
            },

            // run: 12 frames. Y tracks body bob (+1 dip, -1 rise). Hand-tuned.
            ["run"] = new[]
            {
                new Vector2(1, 0),  // f0
                new Vector2(1, 1),  // f1
                new Vector2(1, 1),  // f2
                new Vector2(1, 1),  // f3
                new Vector2(1,-1),  // f4
                new Vector2(1, 0),  // f5
                new Vector2(1, 1),  // f6
                new Vector2(1, 1),  // f7
                new Vector2(1, 1),  // f8
                new Vector2(1, 1),  // f9
                new Vector2(1,-1),  // f10
                new Vector2(1, 0),  // f11
            },

            ["dash"] = new[]
            {
                new Vector2(2, 1), new Vector2(2, 1),
                new Vector2(2, 1), new Vector2(2, 2)
            },

            // jumpfast: 2 frames. Calibrated.
            ["jumpfast"] = new[] { new Vector2(1, -1), new Vector2(1, -1) },

            // fallslow: 2 frames. Calibrated.
            ["fallslow"] = new[] { new Vector2(1, 0), new Vector2(0, 0) },

            // climbup: 6 frames. Calibrated.
            ["climbup"] = new[]
            {
                new Vector2( 0, 0), new Vector2( 0, 0), new Vector2( 0, 0),  // f0–f2
                new Vector2( 0, 0), new Vector2(-1, 0), new Vector2(-1, 0)   // f3–f5
            },

            // dangling: 10 frames. Calibrated.
            ["dangling"] = new[]
            {
                new Vector2( 0, 0), new Vector2( 0, 0), new Vector2( 0, 0), new Vector2( 0, 0),  // f0–f3
                new Vector2(-1, 0), new Vector2(-1, 0), new Vector2(-1, 0), new Vector2(-1, 0),  // f4–f7
                new Vector2(-1, 0), new Vector2(-1, 0)                                           // f8–f9
            },

            // idleB: 24 frames. Calibrated (debug wrapped f23→f0).
            ["idleb"] = new[]
            {
                new Vector2( 0, 0),  // f0
                new Vector2(-1, 0),  // f1
                new Vector2(-1, 0),  // f2
                new Vector2(-1, 0),  // f3
                new Vector2(-1, 0),  // f4
                new Vector2(-1, 0),  // f5
                new Vector2(-1, 0),  // f6
                new Vector2(-1, 0),  // f7
                new Vector2(-1, 0),  // f8
                new Vector2(-2, 0),  // f9
                new Vector2(-2, 0),  // f10
                new Vector2(-2, 0),  // f11
                new Vector2(-2, 0),  // f12
                new Vector2(-1, 0),  // f13
                new Vector2(-1, 0),  // f14
                new Vector2(-1, 0),  // f15
                new Vector2(-1, 0),  // f16
                new Vector2( 0, 0),  // f17
                new Vector2( 0, 0),  // f18
                new Vector2( 0, 1),  // f19
                new Vector2( 0, 1),  // f20
                new Vector2( 0, 1),  // f21
                new Vector2( 0, 1),  // f22
                new Vector2( 0, 1),  // f23
            },

            // idleC: 12 frames. Calibrated (debug wrapped f11→f0).
            ["idlec"] = new[]
            {
                new Vector2(-1, 0),  // f0
                new Vector2(-2, 0),  // f1
                new Vector2(-2, 0),  // f2
                new Vector2(-2, 0),  // f3
                new Vector2(-2, 0),  // f4
                new Vector2(-2, 0),  // f5
                new Vector2( 2, 1),  // f6 — head shifts right (look gesture)
                new Vector2( 3, 2),  // f7
                new Vector2( 0, 2),  // f8
                new Vector2(-1, 1),  // f9
                new Vector2( 0, 1),  // f10
                new Vector2( 0, 0),  // f11
            },

            ["standard"] = new[] { new Vector2(0, 0) }
        };

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
