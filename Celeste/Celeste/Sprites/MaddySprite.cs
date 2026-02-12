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

            // Basic 9-frame idle (breathing loop). Frozen for hair alignment.
            var idle = new AutoAnimation();
            idle.Detect(content.Load<Texture2D>("idle"), 32, 32, 0.001f, false);

            var run = new AutoAnimation();
            run.Detect(content.Load<Texture2D>("runFast"), 32, 32, 12f, true);

            var jumpFast = new AutoAnimation();
            jumpFast.Detect(content.Load<Texture2D>("jumpFast"), 32, 32, 4f, false);

            var fallSlow = new AutoAnimation();
            fallSlow.Detect(content.Load<Texture2D>("fallSlow"), 32, 32, 4f, true);

            var dash = new AutoAnimation();
            dash.Detect(content.Load<Texture2D>("dash"), 32, 32, 8f, false);

            var climbUp = new AutoAnimation();
            climbUp.Detect(content.Load<Texture2D>("climbup"), 32, 32, 12f, true);

            var dangling = new AutoAnimation();
            dangling.Detect(content.Load<Texture2D>("dangling"), 32, 32, 8f, true);

            controller.Register(PlayerState.Idle, idle, setAsDefault: true);
            controller.Register(PlayerState.Run, run);
            controller.Register(PlayerState.JumpFast, jumpFast);
            controller.Register(PlayerState.FallSlow, fallSlow);
            controller.Register(PlayerState.Dash, dash);
            controller.Register(PlayerState.ClimbUp, climbUp);
            controller.Register(PlayerState.Dangling, dangling);

            var body = new BodySprite<PlayerState>(controller);
            var hair = new HairRenderer();
            hair.LoadContent(content);

            return new MaddySprite(body, hair);
        }

        // ----- Semantic animation switching -----

        private void SetAnimation(PlayerState state, string animName, bool restart = false)
        {
            _body.Controller.SetState(state, restart);
            _currentAnimName = animName;
        }

        public void Idle(bool restart = false) => SetAnimation(PlayerState.Idle, "idle", restart);
        public void Run(bool restart = false) => SetAnimation(PlayerState.Run, "run", restart);
        public void JumpFast(bool restart = true) => SetAnimation(PlayerState.JumpFast, "jumpfast", restart);
        public void FallSlow(bool restart = true) => SetAnimation(PlayerState.FallSlow, "fallslow", restart);
        public void Dash(bool restart = true) => SetAnimation(PlayerState.Dash, "dash", restart);
        public void ClimbUp(bool restart = false) => SetAnimation(PlayerState.ClimbUp, "climbup", restart);
        public void Dangling(bool restart = false) => SetAnimation(PlayerState.Dangling, "dangling", restart);

        // ----- Stored draw parameters for hair anchor calculation -----
        private Vector2 _lastPosition;
        private float _lastScale = 1f;

        public void SetPosition(Vector2 position, float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;
        }

        // ----- IMaddySprite.Update -----

        public void Update(GameTime gameTime)
        {
            _body.Update(gameTime);

            _hair.DrawScale = _lastScale;

            // Hair anchor: position within the 32x32 frame where the
            // top-center of Madeline's head is.
            Vector2 localOffset = HairOffsetData.GetOffset(_currentAnimName, _body.CurrentFrame);

            if (_faceLeft)
            {
                // When the sprite is flipped horizontally, pixel column c
                // appears at column (FrameWidth - 1 - c). Mirror the X offset.
                localOffset.X = (_body.FrameWidth - 1) - localOffset.X;
            }

            Vector2 hairAnchor = _lastPosition + localOffset * _lastScale;

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
