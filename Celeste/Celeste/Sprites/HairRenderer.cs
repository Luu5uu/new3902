using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Procedural hair renderer that replicates the full Celeste game's
    /// PlayerHair system.
    ///
    /// Algorithm (from the decompiled PlayerHair.cs):
    ///   - Node 0 is pinned to the head anchor every frame (never lags).
    ///   - Nodes 1+ use Calc.Approach toward a target derived from the
    ///     previous node, plus a gravity/facing bias per segment.
    ///   - A hard distance constraint (3px native) prevents over-stretching.
    ///   - Approach speed decreases for tail nodes (stiff near head,
    ///     loose at tail).
    ///   - Node 0 draws the bangs sprite; nodes 1+ draw the hair00
    ///     circle texture, tinted to HairColor and tapered from 100%
    ///     to 25% scale.
    ///
    /// IMPORTANT: The anchor position passed to Update is in screen space
    /// (already multiplied by the draw scale). All physics constants are
    /// defined in native pixel space and are scaled internally by
    /// <see cref="DrawScale"/> so they work correctly at any zoom level.
    ///
    /// References:
    ///   - PlayerHair.cs from the decompiled Celeste source
    ///   - Classic.player_hair.draw_hair() from NoelFB/Celeste (PICO-8)
    /// </summary>
    public sealed class HairRenderer : IHairSprite
    {
        // ----- Configuration (native pixel values, scaled by DrawScale) -----

        /// <summary>Number of nodes in the chain.</summary>
        private const int DefaultNodeCount = 5;

        /// <summary>
        /// Hard max distance between consecutive nodes (native pixels).
        /// From PlayerHair.cs: clamped to 3px.
        /// </summary>
        private const float MaxNodeDistanceNative = 3f;

        /// <summary>
        /// Base approach speed in native pixels per second.
        /// From PlayerHair.cs: StepApproach = 64f.
        /// </summary>
        private const float StepApproachNative = 64f;

        /// <summary>
        /// Per-segment downward gravity bias (native pixels).
        /// From PlayerHair.cs: StepPerSegment = (0, 2).
        /// </summary>
        private static readonly Vector2 StepPerSegmentNative = new Vector2(0f, 2f);

        /// <summary>
        /// Per-segment horizontal bias opposite the facing direction (native).
        /// From PlayerHair.cs: StepInFacingPerSegment = 0.5f.
        /// </summary>
        private const float StepInFacingNative = 0.5f;

        // ----- Scale -----

        /// <summary>
        /// The rendering scale factor (e.g. 2f when drawing at 2x).
        /// Set this before calling Update each frame so that physics
        /// constants are correctly scaled from native to screen space.
        /// </summary>
        public float DrawScale { get; set; } = 1f;

        // ----- Textures -----

        /// <summary>
        /// The bangs/fringe texture strip (3 frames, 8x8 each).
        /// White sprite tinted to HairColor at draw time.
        /// Drawn at node 0 to cover where the chain meets the head.
        /// </summary>
        private Texture2D _bangsTexture;
        private int _bangsFrameWidth = 8;
        private int _bangsFrameHeight = 8;
        private int _bangsFrameCount = 3;

        /// <summary>
        /// The hair circle texture (8x8 white filled circle).
        /// From the original game: "characters/player/hair00".
        /// Drawn at nodes 1+ and tinted to HairColor at draw time.
        /// </summary>
        private Texture2D _circleTexture;
        private int _circleWidth = 8;
        private int _circleHeight = 8;

        /// <summary>
        /// Which bangs frame to show (0 = right, 1 = center, 2 = left).
        /// Set by MaddySprite each frame via BangsFrameData.
        /// </summary>
        public int BangsFrame { get; set; } = 0;

        // ----- State -----

        private readonly Vector2[] _nodes;
        private bool _faceLeft;

        // ----- IHairSprite implementation -----

        /// <summary>
        /// Hair color. Default is Madeline's red (#AC3232).
        /// </summary>
        public Color HairColor { get; set; } = new Color(172, 50, 50);

        public int NodeCount => _nodes.Length;

        // ----- Construction -----

        public HairRenderer(int nodeCount = DefaultNodeCount)
        {
            _nodes = new Vector2[nodeCount];
        }

        /// <summary>
        /// Loads the hair circle texture and bangs strip.
        /// Must be called once during LoadContent.
        /// </summary>
        public void LoadContent(ContentManager content)
        {
            _bangsTexture = content.Load<Texture2D>("bangs");
            _bangsFrameWidth = _bangsTexture.Width / _bangsFrameCount;
            _bangsFrameHeight = _bangsTexture.Height;

            _circleTexture = content.Load<Texture2D>("hair00");
            _circleWidth = _circleTexture.Width;
            _circleHeight = _circleTexture.Height;
        }

        // ----- Public API -----

        /// <summary>
        /// Resets all nodes to the given anchor position.
        /// </summary>
        public void Reset(Vector2 anchor)
        {
            for (int i = 0; i < _nodes.Length; i++)
                _nodes[i] = anchor;
        }

        // ----- IHairSprite.Update -----

        /// <summary>
        /// Updates the hair chain using the full Celeste PlayerHair algorithm.
        /// The anchor position should be in screen space (world position +
        /// hair offset, already scaled).
        /// </summary>
        public void Update(GameTime gameTime, Vector2 anchorPosition, bool faceLeft)
        {
            _faceLeft = faceLeft;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float s = DrawScale;

            // Scale all native-space constants into screen space.
            float maxDist = MaxNodeDistanceNative * s;
            float approachSpeed = StepApproachNative * s;
            Vector2 stepPerSeg = StepPerSegmentNative * s;
            float stepFacing = StepInFacingNative * s;

            // Node 0 is always pinned to the head.
            _nodes[0] = anchorPosition;

            // Hair trails opposite the facing direction.
            float facing = faceLeft ? 1f : -1f;

            // First target: node 0 + initial bias.
            Vector2 target = _nodes[0]
                + new Vector2(facing * stepFacing * 2f, 0f)
                + stepPerSeg;

            Vector2 previous = _nodes[0];

            for (int i = 1; i < _nodes.Length; i++)
            {
                // Speed decreases for tail nodes (stiff near head, loose at tail).
                float speedFactor = 1f - (float)i / _nodes.Length * 0.5f;
                float maxMove = speedFactor * approachSpeed * dt;

                // Move this node toward its target.
                _nodes[i] = Approach(_nodes[i], target, maxMove);

                // Hard distance constraint from previous node.
                Vector2 diff = _nodes[i] - previous;
                float dist = diff.Length();
                if (dist > maxDist)
                {
                    _nodes[i] = previous + SafeNormalize(diff) * maxDist;
                }

                // Derive the next target from this node.
                target = _nodes[i]
                    + new Vector2(facing * stepFacing, 0f)
                    + stepPerSeg;

                previous = _nodes[i];
            }
        }

        // ----- IHairSprite.Draw -----

        public void Draw(SpriteBatch spriteBatch, Color color, float scale = 1f)
        {
            Color drawColor = (color == Color.White) ? HairColor : color;

            // Origin for the 8x8 circle texture: center at (4, 4).
            Vector2 circleOrigin = new Vector2(_circleWidth / 2f, _circleHeight / 2f);

            // Bangs setup (shared by outline and fill passes).
            // Use the actual left-facing bangs frame (2) when facing left
            // instead of flipping frame 0 -- the artist-drawn frames differ
            // in shape/height, so flipping produces misalignment.
            int bangFrame = _faceLeft
                ? 2
                : Math.Clamp(BangsFrame, 0, _bangsFrameCount - 1);
            Rectangle bangSrc = new Rectangle(
                bangFrame * _bangsFrameWidth, 0,
                _bangsFrameWidth, _bangsFrameHeight);
            // No nudge -- bangs center aligns directly to node 0 (head anchor).
            Vector2 bangsPos = _nodes[0];
            SpriteEffects bangEffects = SpriteEffects.None;
            Vector2 bangsOrigin = new Vector2(
                _bangsFrameWidth / 2f,
                _bangsFrameHeight / 2f);

            // ============================================================
            // OUTLINE PASS: draw every sprite at 4 cardinal 1px offsets
            // in Color.Black. This matches the original Celeste
            // PlayerHair.Render which draws an outline before the fill.
            // ============================================================
            float outlineDist = scale; // 1 native pixel in screen space
            Vector2[] outlineOffsets =
            {
                new Vector2( outlineDist, 0f),
                new Vector2(-outlineDist, 0f),
                new Vector2(0f,  outlineDist),
                new Vector2(0f, -outlineDist)
            };

            foreach (Vector2 off in outlineOffsets)
            {
                // Outline: circle nodes (tail to head, skip node 0 -- bangs only there).
                if (_circleTexture != null)
                {
                    for (int i = _nodes.Length - 1; i >= 1; i--)
                    {
                        float taper = 0.25f + (1f - (float)i / _nodes.Length) * 0.75f;
                        Vector2 nodeScale = new Vector2(taper * scale, taper * scale);

                        spriteBatch.Draw(
                            _circleTexture,
                            _nodes[i] + off,
                            null,
                            Color.Black,
                            0f,
                            circleOrigin,
                            nodeScale,
                            SpriteEffects.None,
                            0f);
                    }
                }

                // Outline: bangs sprite.
                if (_bangsTexture != null)
                {
                    spriteBatch.Draw(
                        _bangsTexture,
                        bangsPos + off,
                        bangSrc,
                        Color.Black,
                        0f,
                        bangsOrigin,
                        scale,
                        bangEffects,
                        0f);
                }
            }

            // ============================================================
            // FILL PASS: draw the actual colored hair on top of the outline.
            // ============================================================

            // Fill: circle nodes (tail to head, back-to-front, skip node 0).
            if (_circleTexture != null)
            {
                for (int i = _nodes.Length - 1; i >= 1; i--)
                {
                    float taper = 0.25f + (1f - (float)i / _nodes.Length) * 0.75f;
                    Vector2 nodeScale = new Vector2(taper * scale, taper * scale);

                    spriteBatch.Draw(
                        _circleTexture,
                        _nodes[i],
                        null,
                        drawColor,
                        0f,
                        circleOrigin,
                        nodeScale,
                        SpriteEffects.None,
                        0f);
                }
            }

            // Fill: bangs sprite on top.
            if (_bangsTexture != null)
            {
                spriteBatch.Draw(
                    _bangsTexture,
                    bangsPos,
                    bangSrc,
                    drawColor,
                    0f,
                    bangsOrigin,
                    scale,
                    bangEffects,
                    0f);
            }
        }

        // ----- Helpers -----

        /// <summary>
        /// Moves <paramref name="from"/> toward <paramref name="to"/> by at
        /// most <paramref name="maxMove"/> pixels.
        /// </summary>
        private static Vector2 Approach(Vector2 from, Vector2 to, float maxMove)
        {
            Vector2 diff = to - from;
            float dist = diff.Length();
            if (dist <= maxMove || dist == 0f)
                return to;
            return from + diff / dist * maxMove;
        }

        /// <summary>
        /// Safe normalize: returns unit vector or zero if length is 0.
        /// </summary>
        private static Vector2 SafeNormalize(Vector2 v)
        {
            float len = v.Length();
            if (len == 0f) return Vector2.Zero;
            return v / len;
        }
    }
}
