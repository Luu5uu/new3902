using System;
using System.Collections.Generic;
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Celeste.Sprites
{
    public static class HairOffsetData
    {
        private static Dictionary<string, Vector2[]> _offsets = new(StringComparer.OrdinalIgnoreCase);
        private static bool _loaded = false;

        /// <summary>
        /// Load hair per-frame offsets from Content/HairOffsets.xml at runtime.
        /// </summary>
        public static void LoadFromXml(ContentManager content, string xmlFileName = "HairOffsets.xml")
        {
            _offsets = HairOffsetConfigLoader.Load(content, xmlFileName);
            _loaded = true;

            // HARD FAIL: if nothing loaded, you are definitely not reading the XML you think you are.
            if (_offsets.Count == 0)
                throw new InvalidOperationException(
                    $"HairOffsetData: loaded 0 animations from '{xmlFileName}'. " +
                    "Check XML tag names (<HairOffsets>, <Anim>, <F>) and that the file is copied to bin/.../Content/.");
        }

        public static Vector2 GetOffset(string animationName, int frameIndex)
        {
            if (string.IsNullOrWhiteSpace(animationName)) return Vector2.Zero;

            string key = animationName.ToLowerInvariant();

            if (!_offsets.TryGetValue(key, out Vector2[] frames) || frames.Length == 0)
                return Vector2.Zero;

            if (frameIndex < 0) return frames[0];
            if (frameIndex >= frames.Length) return frames[^1];
            return frames[frameIndex];
        }

        public static void AssertMatchesClipFrames(AnimationCatalog catalog)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (_offsets.Count == 0)
                throw new InvalidOperationException("HairOffsetData: no offsets loaded. Check Content/HairOffsets.xml copy path and LoadFromXml timing.");

            foreach (var (key, clip) in catalog.Clips)
            {
                // Only validate Player clips that use hair offsets (by your naming scheme).
                // Your _currentAnimName keys are like "idled", "run", etc.
                // Map AnimationKeys -> animName used by HairOffsetData:
                string animName = key switch
                {
                    AnimationKeys.PlayerIdle         => "idled",
                    AnimationKeys.PlayerIdleFidgetA  => "idlea",
                    AnimationKeys.PlayerIdleFidgetB  => "idleb",
                    AnimationKeys.PlayerIdleFidgetC  => "idlec",
                    AnimationKeys.PlayerRun          => "run",
                    AnimationKeys.PlayerDash         => "dash",
                    AnimationKeys.PlayerJumpFast     => "jumpfast",
                    AnimationKeys.PlayerFallSlow     => "fallslow",
                    AnimationKeys.PlayerClimbUp      => "climbup",
                    AnimationKeys.PlayerDangling     => "dangling",
                    AnimationKeys.PlayerWallSlide    => "wallslide",
                    AnimationKeys.PlayerTired        => "tired",
                    AnimationKeys.PlayerTiredStill   => "tiredstill",
                    AnimationKeys.PlayerClimbPull    => "climbpull",
                    AnimationKeys.PlayerDuck         => "duck",
                    _ => ""
                };

                if (string.IsNullOrEmpty(animName))
                    continue;

                if (!_offsets.TryGetValue(animName, out var frames) || frames.Length == 0)
                    throw new InvalidOperationException($"HairOffsetData missing animation '{animName}' (from clip key '{key}').");

                if (frames.Length != clip.FrameCount)
                    throw new InvalidOperationException(
                        $"HairOffsetData length mismatch for '{animName}': offsets={frames.Length}, clipFrames={clip.FrameCount} (clip key '{key}').");
            }
        }

        public static bool IsLoaded => _loaded;
    }
}
