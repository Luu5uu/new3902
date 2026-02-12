using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation{
    /// <summary>
    /// High-level animation states for the player character.
    /// </summary>
    public enum PlayerState{
        Standard,
        Idle,
        IdleFidgetA,
        IdleFidgetB,
        IdleFidgetC,
        Run,
        JumpFast,
        FallSlow,
        Dash,
        ClimbUp,
        Dangling
    }
    /// <summary>
    /// High-level animation facade for the player character.
    ///
    /// Responsibilities:
    /// - Load and configure all player animations
    /// - Register animations with AnimationController
    /// - Expose semantic methods (Idle, Run, etc.) for gameplay code
    ///
    /// This is the ONLY animation class that gameplay code should use.
    /// <author> Albert Liu </author>
    public sealed class PlayerAnimations{
        private readonly AnimationController<PlayerState> _controller = new();

        private PlayerAnimations() { }

        /// <summary>
        /// Builds and initializes the full player animation set.
        /// Should be called once during LoadContent.
        /// </summary>
        public static PlayerAnimations Build(ContentManager content){
            var anims = new PlayerAnimations();
            // ---- Standard ----
            var standard = new AutoAnimation();
            standard.Detect(content.Load<Texture2D>("standard"), 32, 32, 1f, true);

            // ---- Idle ----
            var idle = new AutoAnimation();
            idle.Detect(content.Load<Texture2D>("idelA"), 32, 32, 8f, true);

            // ---- Run ----
            var run = new AutoAnimation();
            run.Detect(content.Load<Texture2D>("run"), 32, 32, 12f, true);

            // ---- Jump (fast) ----
            var jumpFast = new AutoAnimation();
            jumpFast.Detect(content.Load<Texture2D>("jumpfast"), 32, 32, fps: 4f, loop: false);

            // ---- Fall (slow) ----
            var fallSlow = new AutoAnimation();
            fallSlow.Detect(content.Load<Texture2D>("fallSlow"), 32, 32, fps: 4f, loop: true);

            // ---- Dash ----
            var dash = new AutoAnimation();
            dash.Detect(content.Load<Texture2D>("dash"), 32, 32, fps: 8f, loop: false);

            // ---- Climb Up ----
            var climbUp = new AutoAnimation();
            climbUp.Detect(content.Load<Texture2D>("climbup"), 32, 32, fps: 12f, loop: true);

            // ---- Dangling (hang on wall) ----
            var dangling = new AutoAnimation();
            dangling.Detect(content.Load<Texture2D>("dangling"), 32, 32, fps: 8f, loop: true);

            anims._controller.Register(PlayerState.Standard,standard, setAsDefault: true);
            anims._controller.Register(PlayerState.Idle, idle);
            anims._controller.Register(PlayerState.Run, run);
            anims._controller.Register(PlayerState.JumpFast, jumpFast);
            anims._controller.Register(PlayerState.FallSlow, fallSlow);
            anims._controller.Register(PlayerState.Dash, dash);
            anims._controller.Register(PlayerState.ClimbUp, climbUp);
            anims._controller.Register(PlayerState.Dangling, dangling);
            return anims;
        }
        /// Switches to the Standard animation.
        public void Standard(bool restart = false)      => _controller.SetState(PlayerState.Standard, restart);

        /// Switches to the idle animation.
        public void Idle(bool restart = false)      => _controller.SetState(PlayerState.Idle, restart);

        /// Switches to the run animation.
        public void Run(bool restart = false)       => _controller.SetState(PlayerState.Run, restart);

        /// Switches to the JumpFast animation.
        public void JumpFast(bool restart = true)   => _controller.SetState(PlayerState.JumpFast, restart);

        /// Switches to the FallSlow animation.
        public void FallSlow(bool restart = true)  => _controller.SetState(PlayerState.FallSlow, restart);

        /// Switches to the Dash animation.
        public void Dash(bool restart = true)       => _controller.SetState(PlayerState.Dash, restart);

        /// Switches to the ClimbUp animation.
        public void ClimbUp(bool restart = false)   => _controller.SetState(PlayerState.ClimbUp, restart);

        /// Switches to the Dangling animation.
        public void Dangling(bool restart = false)  => _controller.SetState(PlayerState.Dangling, restart);

        /// <summary>
        /// Updates the currently active animation.
        /// Must be called once per frame.
        /// </summary>
        public void Update(GameTime gameTime)
            => _controller.Update(gameTime);

        /// <summary>
        /// Draws the current animation, automatically handling facing direction.
        /// </summary>
        /// <param name="faceLeft">
        /// If true, the animation is drawn flipped horizontally.
        /// </param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f, bool faceLeft = false){
            var effects = faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _controller.Draw(spriteBatch, position, color, scale, effects);
        }

    }
}
