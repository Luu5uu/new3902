using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Procedural hair using the full Celeste PlayerHair algorithm:
    // node 0 pinned to head, nodes 1+ trail with gravity/facing bias,
    // hard 3px distance constraint, bangs overlay at node 0,
    // 1px black outline pass before colored fill pass.
    public sealed class HairRenderer : IHairSprite
    {
        private const int DefaultNodeCount = 5;
        private const float MaxNodeDistNative = 3f;      // PlayerHair.cs clamp
        private const float StepApproachNative = 64f;     // PlayerHair.cs approach speed
        private static readonly Vector2 StepPerSegNative = new(0f, 2f);
        private const float StepFacingNative = 0.5f;

        public float DrawScale { get; set; } = 1f;

        private Texture2D _bangsTexture;
        private int _bangsFrameW = 8, _bangsFrameH = 8, _bangsFrameCount = 3;
        private Texture2D _circleTexture;
        private int _circleW = 8, _circleH = 8;

        public int BangsFrame { get; set; } = 0;

        private readonly Vector2[] _nodes;
        private bool _faceLeft;

        public Color HairColor { get; set; } = new Color(172, 50, 50); // Madeline's red
        public int NodeCount => _nodes.Length;

        public HairRenderer(int nodeCount = DefaultNodeCount)
        {
            _nodes = new Vector2[nodeCount];
        }

        public void LoadContent(ContentManager content)
        {
            _bangsTexture = content.Load<Texture2D>("bangs");
            _bangsFrameW = _bangsTexture.Width / _bangsFrameCount;
            _bangsFrameH = _bangsTexture.Height;
            _circleTexture = content.Load<Texture2D>("hair00");
            _circleW = _circleTexture.Width;
            _circleH = _circleTexture.Height;
        }

        public void Reset(Vector2 anchor)
        {
            for (int i = 0; i < _nodes.Length; i++)
                _nodes[i] = anchor;
        }

        public void Update(GameTime gameTime, Vector2 anchorPosition, bool faceLeft)
        {
            _faceLeft = faceLeft;
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float s = DrawScale;

            float maxDist = MaxNodeDistNative * s;
            float approachSpeed = StepApproachNative * s;
            Vector2 stepPerSeg = StepPerSegNative * s;
            float stepFacing = StepFacingNative * s;

            _nodes[0] = anchorPosition;
            float facing = faceLeft ? 1f : -1f;

            Vector2 target = _nodes[0] + new Vector2(facing * stepFacing * 2f, 0f) + stepPerSeg;
            Vector2 previous = _nodes[0];

            for (int i = 1; i < _nodes.Length; i++)
            {
                float speedFactor = 1f - (float)i / _nodes.Length * 0.5f;
                _nodes[i] = Approach(_nodes[i], target, speedFactor * approachSpeed * dt);

                Vector2 diff = _nodes[i] - previous;
                float dist = diff.Length();
                if (dist > maxDist)
                    _nodes[i] = previous + SafeNorm(diff) * maxDist;

                target = _nodes[i] + new Vector2(facing * stepFacing, 0f) + stepPerSeg;
                previous = _nodes[i];
            }
        }

        public void Draw(SpriteBatch spriteBatch, Color color, float scale = 1f)
        {
            Color drawColor = (color == Color.White) ? HairColor : color;
            Vector2 circleOrigin = new(_circleW / 2f, _circleH / 2f);

            // Pick bangs frame (use left-facing frame directly when facing left).
            int bangFrame = _faceLeft ? 2 : Math.Clamp(BangsFrame, 0, _bangsFrameCount - 1);
            Rectangle bangSrc = new(bangFrame * _bangsFrameW, 0, _bangsFrameW, _bangsFrameH);
            Vector2 bangsOrigin = new(_bangsFrameW / 2f, _bangsFrameH / 2f);

            // --- Outline pass: 1px black border at 4 cardinal offsets ---
            float od = scale;
            Vector2[] offsets = { new(od, 0), new(-od, 0), new(0, od), new(0, -od) };

            foreach (Vector2 off in offsets)
            {
                for (int i = _nodes.Length - 1; i >= 1; i--)
                {
                    float taper = 0.25f + (1f - (float)i / _nodes.Length) * 0.75f;
                    spriteBatch.Draw(_circleTexture, _nodes[i] + off, null, Color.Black,
                        0f, circleOrigin, new Vector2(taper * scale), SpriteEffects.None, 0f);
                }
                spriteBatch.Draw(_bangsTexture, _nodes[0] + off, bangSrc, Color.Black,
                    0f, bangsOrigin, scale, SpriteEffects.None, 0f);
            }

            // --- Fill pass: colored hair on top ---
            for (int i = _nodes.Length - 1; i >= 1; i--)
            {
                float taper = 0.25f + (1f - (float)i / _nodes.Length) * 0.75f;
                spriteBatch.Draw(_circleTexture, _nodes[i], null, drawColor,
                    0f, circleOrigin, new Vector2(taper * scale), SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(_bangsTexture, _nodes[0], bangSrc, drawColor,
                0f, bangsOrigin, scale, SpriteEffects.None, 0f);
        }

        private static Vector2 Approach(Vector2 from, Vector2 to, float maxMove)
        {
            Vector2 diff = to - from;
            float dist = diff.Length();
            return (dist <= maxMove || dist == 0f) ? to : from + diff / dist * maxMove;
        }

        private static Vector2 SafeNorm(Vector2 v)
        {
            float len = v.Length();
            return len == 0f ? Vector2.Zero : v / len;
        }
    }
}
