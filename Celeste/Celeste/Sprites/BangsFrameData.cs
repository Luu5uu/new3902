using System.Collections.Generic;

namespace Celeste.Sprites
{
    // Per-frame bangs frame index: 0=right, 1=center, 2=left.
    // Indices are relative to the native right-facing sprite.
    public static class BangsFrameData
    {
        private static readonly Dictionary<string, int[]> Frames = new()
        {
            ["idled"]    = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            ["idlea"]    = new[] { 0, 0, 1, 2, 2, 2, 2, 2, 1, 0, 0, 0 },
            ["idleb"]    = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            ["idlec"]    = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            ["run"]      = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            ["dash"]     = new[] { 0, 0, 0, 0 },
            ["jumpfast"] = new[] { 0, 0 },
            ["fallslow"] = new[] { 0, 0 },
            ["climbup"]  = new[] { 0, 0, 0, 0, 0, 0 },
            ["dangling"] = new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        public static int GetFrame(string animationName, int frameIndex)
        {
            string key = animationName.ToLowerInvariant();
            // new animation doesnt have bang data yet
            if (!Frames.TryGetValue(key, out int[] frames))
                return 0;
            // if animation has fewer frames than current idx, clamp by returning last idx
            if (frameIndex >= frames.Length)
                return frames[frames.Length - 1];
            return frames[frameIndex];
        }
    }
}
