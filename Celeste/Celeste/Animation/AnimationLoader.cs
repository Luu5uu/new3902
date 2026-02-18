using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    /// <summary>
    /// Loads all animation sprite strips from Content and returns pure data clips.
    /// This loader performs validation (frame sizes, divisibility) and builds AnimationClip objects.
    /// </summary>
    /// <author> Albert Liu </author>
    public static class AnimationLoader
    {
        /// <summary>
        /// Loads the full animation catalog once during LoadContent.
        /// Player body strips (idle, runFast, etc.) are required. Legacy *_static_hair strips are optional.
        /// </summary>
        public static AnimationCatalog LoadAll(ContentManager content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));
            var catalog = new AnimationCatalog();

            // ---- Player (strips used by MaddySprite: idle, idleA, runFast, etc.) ----
            TryAddClip(catalog, content, AnimationKeys.PlayerStandard,   "standard",  32, 32,  1f, true);
            catalog.Clips[AnimationKeys.PlayerIdle]        = BuildClip(content, "idle",      32, 32,  0.001f, false);
            catalog.Clips[AnimationKeys.PlayerIdleFidgetA] = BuildClip(content, "idleA",     32, 32,  6f, true);
            catalog.Clips[AnimationKeys.PlayerRun]         = BuildClip(content, "runFast",   32, 32, 12f, true);
            catalog.Clips[AnimationKeys.PlayerJumpFast]    = BuildClip(content, "jumpfast",  32, 32,  4f, false);
            catalog.Clips[AnimationKeys.PlayerFallSlow]    = BuildClip(content, "fallSlow",  32, 32,  4f, true);
            catalog.Clips[AnimationKeys.PlayerDash]        = BuildClip(content, "dash",      32, 32,  8f, false);
            catalog.Clips[AnimationKeys.PlayerClimbUp]     = BuildClip(content, "climbup",   32, 32, 12f, true);
            catalog.Clips[AnimationKeys.PlayerDangling]    = BuildClip(content, "dangling",  32, 32,  8f, true);

            // ---- Player legacy (H): manually-drawn hair strips. Optional; add to Content as *_static_hair. ----
            TryAddClip(catalog, content, AnimationKeys.PlayerStandardStaticHair,  "standard_static_hair",  32, 32,  1f, true);
            TryAddClip(catalog, content, AnimationKeys.PlayerIdleStaticHair,      "idelA_static_hair",    32, 32,  8f, true);
            TryAddClip(catalog, content, AnimationKeys.PlayerRunStaticHair,       "run_static_hair",      32, 32, 12f, true);
            TryAddClip(catalog, content, AnimationKeys.PlayerJumpFastStaticHair, "jumpfast_static_hair", 32, 32,  4f, false);
            TryAddClip(catalog, content, AnimationKeys.PlayerFallSlowStaticHair,  "fallSlow_static_hair", 32, 32,  4f, true);
            TryAddClip(catalog, content, AnimationKeys.PlayerDashStaticHair,      "dash_static_hair",     32, 32,  8f, false);
            TryAddClip(catalog, content, AnimationKeys.PlayerClimbUpStaticHair,   "climbup_static_hair",  32, 32, 12f, true);
            TryAddClip(catalog, content, AnimationKeys.PlayerDanglingStaticHair,   "dangling_static_hair", 32, 32,  8f, true);

            // ---- Items ----
            catalog.Clips[AnimationKeys.ItemNormalStaw] = BuildClip(content, "normalStaw", 32, 32, 12f, true);
            catalog.Clips[AnimationKeys.ItemFlyStaw]    = BuildClip(content, "flyStaw",    40, 40, 12f, true);

            return catalog;
        }

        /// <summary>
        /// Tries to load a clip and add it to the catalog. If the asset is missing, the clip is not added.
        /// </summary>
        private static void TryAddClip(AnimationCatalog catalog, ContentManager content,
            string key, string assetName, int frameWidth, int frameHeight, float fps, bool loop)
        {
            try
            {
                catalog.Clips[key] = BuildClip(content, assetName, frameWidth, frameHeight, fps, loop);
            }
            catch (ContentLoadException)
            {
                // Asset not in Content; legacy (H) will have fewer or no animations until *_static_hair strips are added.
            }
        }

        /// <summary>
        /// Loads a sprite strip texture and converts it to a pure AnimationClip.
        /// Frames are assumed to be arranged horizontally.
        /// </summary>
        private static AnimationClip BuildClip(
            ContentManager content,
            string assetName,
            int frameWidth,
            int frameHeight,
            float fps,
            bool loop)
        {
            Texture2D tex = content.Load<Texture2D>(assetName);

            if (frameWidth <= 0)  throw new ArgumentOutOfRangeException(nameof(frameWidth));
            if (frameHeight <= 0) throw new ArgumentOutOfRangeException(nameof(frameHeight));

            if (tex.Height < frameHeight)
                throw new ArgumentException($"Texture '{assetName}' height {tex.Height} < frameHeight {frameHeight}.");

            if (tex.Width % frameWidth != 0)
                throw new ArgumentException($"Texture '{assetName}' width {tex.Width} is not divisible by frameWidth {frameWidth}.");

            int frameCount = tex.Width / frameWidth;
            float safeFps = fps > 0f ? fps : 12f;

            return new AnimationClip
            {
                Texture = tex,
                FrameWidth = frameWidth,
                FrameHeight = frameHeight,
                FrameCount = frameCount,
                Fps = safeFps,
                Loop = loop
            };
        }
    }
}