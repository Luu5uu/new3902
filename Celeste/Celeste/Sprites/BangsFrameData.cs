using System;
using System.Collections.Generic;
using Celeste.Animation;
using Microsoft.Xna.Framework.Content;

namespace Celeste.Sprites
{
    public static class BangsFrameData
    {
        private static Dictionary<string, int[]> _frames = new(StringComparer.OrdinalIgnoreCase);
        private static bool _loaded = false;

        /// <summary>
        /// Load bangs frame indices from Content/BangsFrames.xml at runtime.
        /// </summary>
        public static void LoadFromXml(ContentManager content, string xmlFileName = "BangsFrames.xml")
        {
            _frames = BangsFrameConfigLoader.Load(content, xmlFileName);
            _loaded = true;
        }

        public static void AssertMatchesClipFrames(AnimationCatalog catalog)
        {
            if (catalog == null) throw new ArgumentNullException(nameof(catalog));
            if (_frames.Count == 0)
                throw new InvalidOperationException("BangsFrameData: no frames loaded. Check Content/BangsFrames.xml copy path and LoadFromXml timing.");

            foreach (var (key, clip) in catalog.Clips)
            {
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
                    _ => ""
                };

                if (string.IsNullOrEmpty(animName))
                    continue;

                if (!_frames.TryGetValue(animName, out var frames) || frames.Length == 0)
                    throw new InvalidOperationException($"BangsFrameData missing animation '{animName}' (from clip key '{key}').");

                if (frames.Length != clip.FrameCount)
                    throw new InvalidOperationException(
                        $"BangsFrameData length mismatch for '{animName}': bangs={frames.Length}, clipFrames={clip.FrameCount} (clip key '{key}').");
            }
        }

        public static int GetFrame(string animationName, int frameIndex)
        {
            if (string.IsNullOrWhiteSpace(animationName)) return 0;

            string key = animationName.ToLowerInvariant();

            if (!_frames.TryGetValue(key, out int[] frames) || frames.Length == 0)
                return 0;

            if (frameIndex < 0) return frames[0];
            if (frameIndex >= frames.Length) return frames[^1];
            return frames[frameIndex];
        }

        public static bool IsLoaded => _loaded;
    }
}
