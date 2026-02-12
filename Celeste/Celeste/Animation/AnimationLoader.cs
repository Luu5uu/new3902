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
        /// </summary>
        public static AnimationCatalog LoadAll(ContentManager content)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            var catalog = new AnimationCatalog();

            // ---- Player ----
            catalog.Clips[AnimationKeys.PlayerStandard] = BuildClip(content, "standard",  32, 32,  1f,  true);
            catalog.Clips[AnimationKeys.PlayerIdle]     = BuildClip(content, "idelA",     32, 32,  8f,  true);
            catalog.Clips[AnimationKeys.PlayerRun]      = BuildClip(content, "run",       32, 32, 12f,  true);
            catalog.Clips[AnimationKeys.PlayerJumpFast] = BuildClip(content, "jumpfast",  32, 32,  4f,  false);
            catalog.Clips[AnimationKeys.PlayerFallSlow] = BuildClip(content, "fallSlow",  32, 32,  4f,  true);
            catalog.Clips[AnimationKeys.PlayerDash]     = BuildClip(content, "dash",      32, 32,  8f,  false);
            catalog.Clips[AnimationKeys.PlayerClimbUp]  = BuildClip(content, "climbup",   32, 32, 12f,  true);
            catalog.Clips[AnimationKeys.PlayerDangling] = BuildClip(content, "dangling",  32, 32,  8f,  true);

            // ---- Items ----
            catalog.Clips[AnimationKeys.ItemNormalStaw] = BuildClip(content, "normalStaw", 32, 32, 12f, true);
            catalog.Clips[AnimationKeys.ItemFlyStaw]    = BuildClip(content, "flyStaw",    40, 40, 12f, true);

            return catalog;
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