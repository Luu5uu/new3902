using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Celeste;

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
            catalog.Clips[AnimationKeys.PlayerStandard]    = BuildClip(content, "standard",   GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  1f,     true);
            catalog.Clips[AnimationKeys.PlayerIdle]        = BuildClip(content, "idleD",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  6f,     true);
            catalog.Clips[AnimationKeys.PlayerIdleFidgetA] = BuildClip(content, "idleA",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  6f,     false);
            catalog.Clips[AnimationKeys.PlayerIdleFidgetB] = BuildClip(content, "idleB",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  6f,     false);
            catalog.Clips[AnimationKeys.PlayerIdleFidgetC] = BuildClip(content, "idleC",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  6f,     false);
            catalog.Clips[AnimationKeys.PlayerRun]         = BuildClip(content, "runFast",   GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight, 12f,     true);
            catalog.Clips[AnimationKeys.PlayerJumpFast]    = BuildClip(content, "jumpfast",   GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  4f,     false);
            catalog.Clips[AnimationKeys.PlayerFallSlow]    = BuildClip(content, "fallSlow",   GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  4f,     true);
            catalog.Clips[AnimationKeys.PlayerDash]        = BuildClip(content, "dash",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  8f,     false);
            catalog.Clips[AnimationKeys.PlayerClimbUp]     = BuildClip(content, "climbup",    GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight, 12f,     true);
            catalog.Clips[AnimationKeys.PlayerDangling]    = BuildClip(content, "dangling",   GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  8f,     true);
            catalog.Clips[AnimationKeys.PlayerDeath]       = BuildClip(content, "death",      GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  8f,     false);

            // ---- Items ----
            catalog.Clips[AnimationKeys.ItemNormalStaw] = BuildClip(content, "normalStaw",    GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight, 12f,     true);
            catalog.Clips[AnimationKeys.ItemFlyStaw]    = BuildClip(content, "flyStaw",       40, 40, 12f,     true);
            catalog.Clips[AnimationKeys.ItemCrystal]    = BuildClip(content, "crystal",       20, 20, 12f,     true);

            // ---- Devices ----
            catalog.Clips[AnimationKeys.DevicesSpring]     = BuildClip(content, "spring",     16, 16,  8f,     true);
            catalog.Clips[AnimationKeys.DevicesMoveBlock]  = BuildClip(content, "moveBlock",  24, 24,  1f,     true);
            catalog.Clips[AnimationKeys.DevicesCrushBlock] = BuildClip(content, "crushBlock", GameConstants.PlayerBodyFrameWidth, GameConstants.PlayerBodyFrameHeight,  1f,     true);

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