using System.Collections.Generic;
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    /// <summary>
    /// Composite sprite that combines a hairless body animation with
    /// a procedural hair renderer, following the original Celeste approach.
    ///
    /// Implements IMaddySprite so gameplay code has a single object to
    /// update and draw for the full character.
    /// </summary>
    public sealed class MaddySprite : IMaddySprite
    {
        // ----- Sub-sprites -----
        private readonly BodySprite<PlayerState> _body;
        private readonly HairRenderer _hair;

        // ----- Facing / animation tracking -----
        private bool _faceLeft;
        private string _currentAnimName = "idle";

        // ----- Debug: ordered list of all animations for Tab cycling -----
        private readonly List<(PlayerState state, string name)> _allAnims = new();

        // ----- IMaddySprite properties -----
        public IBodySprite Body => _body;
        public IHairSprite Hair => _hair;

        // ----- Construction (use Build factory method) -----
        private MaddySprite(BodySprite<PlayerState> body, HairRenderer hair)
        {
            _body = body;
            _hair = hair;
        }

        /// <summary>
        /// Factory method: builds the full composite sprite.
        /// Call once during LoadContent.
        /// </summary>
        public static MaddySprite Build(ContentManager content, GraphicsDevice graphicsDevice = null)
        {
            var controller = new AnimationController<PlayerState>();

            // All textures are HAIRLESS body sprites from Spriters Resource.
            // Hair is added in real-time by HairRenderer.

            // Center-bottom origin (feet) for all 32x32 sprites.
            // This matches the original Celeste's Justify=(0.5, 1.0) approach:
            // the position parameter represents the character's feet, and
            // SpriteEffects.FlipHorizontally mirrors around the center axis.
            var origin = new Vector2(16, 32);

            // Basic standing pose (single frame, frozen).
            var idle = new AutoAnimation();
            idle.Detect(content.Load<Texture2D>("idle"), 32, 32, 0.001f, false);
            idle.Origin = origin;

            // IdleA fidget: 12-frame head-turn animation (~6 fps, loops).
            var idleA = new AutoAnimation();
            idleA.Detect(content.Load<Texture2D>("idleA"), 32, 32, 6f, true);
            idleA.Origin = origin;

            var run = new AutoAnimation();
            run.Detect(content.Load<Texture2D>("runFast"), 32, 32, 12f, true);
            run.Origin = origin;

            var jumpFast = new AutoAnimation();
            jumpFast.Detect(content.Load<Texture2D>("jumpFast"), 32, 32, 4f, false);
            jumpFast.Origin = origin;

            var fallSlow = new AutoAnimation();
            fallSlow.Detect(content.Load<Texture2D>("fallSlow"), 32, 32, 4f, true);
            fallSlow.Origin = origin;

            var dash = new AutoAnimation();
            dash.Detect(content.Load<Texture2D>("dash"), 32, 32, 8f, false);
            dash.Origin = origin;

            var climbUp = new AutoAnimation();
            climbUp.Detect(content.Load<Texture2D>("climbup"), 32, 32, 12f, true);
            climbUp.Origin = origin;

            var dangling = new AutoAnimation();
            dangling.Detect(content.Load<Texture2D>("dangling"), 32, 32, 8f, true);
            dangling.Origin = origin;

            controller.Register(PlayerState.Idle, idle, setAsDefault: true);
            controller.Register(PlayerState.IdleFidgetA, idleA);
            controller.Register(PlayerState.Run, run);
            controller.Register(PlayerState.JumpFast, jumpFast);
            controller.Register(PlayerState.FallSlow, fallSlow);
            controller.Register(PlayerState.Dash, dash);
            controller.Register(PlayerState.ClimbUp, climbUp);
            controller.Register(PlayerState.Dangling, dangling);

            var body = new BodySprite<PlayerState>(controller);
            var hair = new HairRenderer();
            hair.LoadContent(content);

            var maddy = new MaddySprite(body, hair);

            // Register all animations for debug Tab-cycling (order shown in debug).
            maddy._allAnims.Add((PlayerState.Idle, "idle"));
            maddy._allAnims.Add((PlayerState.IdleFidgetA, "idlea"));
            maddy._allAnims.Add((PlayerState.Run, "run"));
            maddy._allAnims.Add((PlayerState.JumpFast, "jumpfast"));
            maddy._allAnims.Add((PlayerState.FallSlow, "fallslow"));
            maddy._allAnims.Add((PlayerState.Dash, "dash"));
            maddy._allAnims.Add((PlayerState.ClimbUp, "climbup"));
            maddy._allAnims.Add((PlayerState.Dangling, "dangling"));

            return maddy;
        }

        // ----- Semantic animation switching -----

        private void SetAnimation(PlayerState state, string animName, bool restart = false)
        {
            _body.Controller.SetState(state, restart);
            _currentAnimName = animName;
        }

        public void Idle(bool restart = false) => SetAnimation(PlayerState.Idle, "idle", restart);
        public void IdleA(bool restart = false) => SetAnimation(PlayerState.IdleFidgetA, "idlea", restart);
        public void Run(bool restart = false) => SetAnimation(PlayerState.Run, "run", restart);
        public void JumpFast(bool restart = true) => SetAnimation(PlayerState.JumpFast, "jumpfast", restart);
        public void FallSlow(bool restart = true) => SetAnimation(PlayerState.FallSlow, "fallslow", restart);
        public void Dash(bool restart = true) => SetAnimation(PlayerState.Dash, "dash", restart);
        public void ClimbUp(bool restart = false) => SetAnimation(PlayerState.ClimbUp, "climbup", restart);
        public void Dangling(bool restart = false) => SetAnimation(PlayerState.Dangling, "dangling", restart);

        // ----- Stored draw parameters for hair anchor calculation -----
        private Vector2 _lastPosition;
        private float _lastScale = 1f;

        /// <summary>
        /// The hair anchor position computed during the last Update().
        /// Exposed for debug overlays (press G in Game1 to visualize).
        /// </summary>
        public Vector2 LastHairAnchor { get; private set; }

        // ----- Debug info exposed for the overlay (press G) -----
        public string DebugAnimName => _currentAnimName;
        public int DebugFrame => _body.CurrentFrame;
        public Vector2 DebugDelta { get; private set; }
        public bool DebugFaceLeft => _faceLeft;

        /// <summary>
        /// Debug nudge applied on top of the stored HairOffsetData delta.
        /// Adjusted live with I/J/K/L while paused. Reset when animation changes.
        /// </summary>
        public Vector2 DebugNudge { get; set; } = Vector2.Zero;

        /// <summary>True when animation playback is paused for debug inspection.</summary>
        public bool DebugPaused { get; private set; }

        /// <summary>Toggle pause/resume on the current body animation.</summary>
        public void DebugPauseToggle()
        {
            DebugPaused = !DebugPaused;
            var anim = _body.Controller.Get(_body.Controller.CurrentState);
            if (DebugPaused)
                anim.Pause();
            else
                anim.Play();
        }

        /// <summary>Step the current animation by +1 or -1 frame while paused.</summary>
        public void DebugStepFrame(int direction)
        {
            if (!DebugPaused) return;
            var anim = _body.Controller.Get(_body.Controller.CurrentState);
            anim.SetFrame(anim.CurrentFrame + direction);
        }

        /// <summary>
        /// Cycle to the next (or previous) animation while paused.
        /// Resets nudge and sets frame to 0.
        /// </summary>
        public void DebugCycleAnimation(int direction)
        {
            if (!DebugPaused || _allAnims.Count == 0) return;

            // Find current animation in the list.
            int idx = _allAnims.FindIndex(a => a.name == _currentAnimName);
            if (idx < 0) idx = 0;

            idx = ((idx + direction) % _allAnims.Count + _allAnims.Count) % _allAnims.Count;

            var (state, name) = _allAnims[idx];
            _body.Controller.SetState(state, restart: true);
            _currentAnimName = name;

            // Pause the new animation immediately and reset frame to 0.
            var anim = _body.Controller.Get(state);
            anim.Pause();
            anim.SetFrame(0);

            // Reset nudge since offsets differ per animation.
            DebugNudge = Vector2.Zero;
        }

        public void SetPosition(Vector2 position, float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;
        }

        // ----- IMaddySprite.Update -----

        /// <summary>
        /// Distance in native pixels from the center-bottom origin (feet)
        /// up to the base head/crown position. Matches the Celeste approach
        /// where PlayerHair uses a fixed vertical shift from RenderPosition.
        /// </summary>
        private const float BaseHeadY = 12f;

        public void Update(GameTime gameTime)
        {
            _body.Update(gameTime);

            _hair.DrawScale = _lastScale;

            // Hair anchor using the Celeste formula:
            //   anchor = feetPos + (0, -BaseHeadY * scale) + delta * (facing, 1) * scale
            //
            // _lastPosition is the feet (center-bottom) position in screen space.
            // The delta is a small per-frame offset from HairOffsetData.
            // For left-facing, negate the X delta (Celeste: offset.X * Facing).

            Vector2 hairDelta = HairOffsetData.GetOffset(_currentAnimName, _body.CurrentFrame);
            DebugDelta = hairDelta; // store raw authored delta for debug overlay

            // Apply live debug nudge (adjusted with I/J/K/L while paused).
            hairDelta += DebugNudge;

            float facing = _faceLeft ? -1f : 1f;
            hairDelta.X *= facing;

            Vector2 hairAnchor = _lastPosition
                + new Vector2(0f, -BaseHeadY * _lastScale)
                + hairDelta * _lastScale;

            LastHairAnchor = hairAnchor;

            // Select the correct bangs frame based on the current animation
            // and frame (e.g. during idle head turn, bangs change direction).
            _hair.BangsFrame = BangsFrameData.GetFrame(_currentAnimName, _body.CurrentFrame);

            _hair.Update(gameTime, hairAnchor, _faceLeft);
        }

        // ----- IMaddySprite.Draw -----

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                         float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;

            // Draw hair behind the body.
            _hair.Draw(spriteBatch, Color.White, scale);

            // Draw the body on top.
            _body.Draw(spriteBatch, position, color, scale, faceLeft);
        }
    }
}
