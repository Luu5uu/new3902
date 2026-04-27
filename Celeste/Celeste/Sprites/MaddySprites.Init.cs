using System;
using Celeste.Animation;
using Microsoft.Xna.Framework;

namespace Celeste.Sprites
{
    // Separation of concerns: initialization + config verification for Task K.
    public sealed partial class MaddySprite
    {
        // Prevent multiple loads/resets if Build is called again.
        private static bool _configLoaded = false;

        /// <summary>
        /// Loads HairOffsets/BangsFrames once and validates lengths against the registered animations.
        /// Also initializes hair nodes to the current anchor to prevent "pinned hair" visuals.
        /// </summary>
        internal void LoadHairConfigAndInit(AnimationCatalog catalog)
        {
            if (!_configLoaded)
            {
                // Load XML configs (Content/HairOffsets.xml, Content/BangsFrames.xml)
                HairOffsetData.LoadFromXml(_hairContent!, "HairOffsets.xml");
                BangsFrameData.LoadFromXml(_hairContent!, "BangsFrames.xml");

                // Validate: offsets/bangs length should match animation frame counts.
                // If your data is correct, this guarantees runtime is actually using it.
                HairOffsetData.AssertMatchesClipFrames(catalog);

                // Bangs frames may intentionally be shorter; still enforce if you want strictness.
                // BangsFrameData.AssertMatchesClipFrames(catalog); // optional

                _configLoaded = true;
            }

            // Ensure hair nodes start at the correct place (eliminates "stuck hair" look).
            ResetHairToCurrentAnchor();
        }

        // Store ContentManager reference for loading config once from Build().
        private Microsoft.Xna.Framework.Content.ContentManager _hairContent;

        internal void SetHairContent(Microsoft.Xna.Framework.Content.ContentManager content)
        {
            _hairContent = content;
        }

        /// <summary>
        /// Hard reset hair node positions to current anchor.
        /// </summary>
        public void ResetHairToCurrentAnchor()
        {
            Vector2 anchor = GetCurrentHairAnchor();
            LastHairAnchor = anchor;
            _hair.BangsFrame = BangsFrameData.GetFrame(_currentAnimName, _body.CurrentFrame);
            _hair.Reset(anchor, _faceLeft);
        }

        public void MoveHairToCurrentAnchor()
        {
            Vector2 anchor = GetCurrentHairAnchor();
            if (LastHairAnchor == Vector2.Zero)
            {
                ResetHairToCurrentAnchor();
                return;
            }

            Vector2 delta = anchor - LastHairAnchor;
            LastHairAnchor = anchor;
            _hair.BangsFrame = BangsFrameData.GetFrame(_currentAnimName, _body.CurrentFrame);
            _hair.MoveBy(delta);
        }
    }
}
