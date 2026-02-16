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
            ["idle"] = new[] { new Vector2(0, 0) },

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

            ["jumpfast"] = new[] { new Vector2(2, -1), new Vector2(2, -1) },

            ["fallslow"] = new[] { new Vector2(0, 0), new Vector2(0, 0) },

            ["climbup"] = new[]
            {
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 0)
            },

            ["dangling"] = new[]
            {
                new Vector2(2, 0), new Vector2(2, 0), new Vector2(2, 0),
                new Vector2(2, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0), new Vector2(1, 0), new Vector2(1, 0),
                new Vector2(1, 0)
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
