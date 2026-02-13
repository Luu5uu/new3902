using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Animation
{
    /// <summary>
    /// High-level animation states for the player character.
    /// </summary>
    public enum PlayerState
    {
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
    /// Legacy player animations (H toggle): full strips with manually-drawn hair.
    /// Builds from the same catalog as the new system; uses *_static_hair clips when present.
    /// </summary>
    public sealed class PlayerAnimations
    {
        private readonly AnimationController<PlayerState> _controller = new();
        private static readonly Vector2 Origin = new(16, 32);

        private PlayerAnimations() { }

        /// <summary>
        /// Builds the legacy set from the catalog. Only registers states for which a *_static_hair clip exists.
        /// Call after AnimationLoader.LoadAll(Content); pass the same catalog used by MaddySprite.
        /// </summary>
        public static PlayerAnimations Build(AnimationCatalog catalog)
        {
            var anims = new PlayerAnimations();
            bool anyRegistered = false;

            void RegisterIfPresent(string key, PlayerState state)
            {
                if (!catalog.Clips.TryGetValue(key, out var clip)) return;
                var auto = AutoAnimation.FromClip(clip);
                auto.Origin = Origin;
                anims._controller.Register(state, auto, setAsDefault: !anyRegistered);
                anyRegistered = true;
            }

            RegisterIfPresent(AnimationKeys.PlayerStandardStaticHair,  PlayerState.Standard);
            RegisterIfPresent(AnimationKeys.PlayerIdleStaticHair,       PlayerState.Idle);
            RegisterIfPresent(AnimationKeys.PlayerRunStaticHair,        PlayerState.Run);
            RegisterIfPresent(AnimationKeys.PlayerJumpFastStaticHair,   PlayerState.JumpFast);
            RegisterIfPresent(AnimationKeys.PlayerFallSlowStaticHair,   PlayerState.FallSlow);
            RegisterIfPresent(AnimationKeys.PlayerDashStaticHair,       PlayerState.Dash);
            RegisterIfPresent(AnimationKeys.PlayerClimbUpStaticHair,    PlayerState.ClimbUp);
            RegisterIfPresent(AnimationKeys.PlayerDanglingStaticHair,   PlayerState.Dangling);

            return anims;
        }

        public void Standard(bool restart = false)   => _controller.SetState(PlayerState.Standard, restart);
        public void Idle(bool restart = false)       => _controller.SetState(PlayerState.Idle, restart);
        public void Run(bool restart = false)        => _controller.SetState(PlayerState.Run, restart);
        public void JumpFast(bool restart = true)    => _controller.SetState(PlayerState.JumpFast, restart);
        public void FallSlow(bool restart = true)    => _controller.SetState(PlayerState.FallSlow, restart);
        public void Dash(bool restart = true)        => _controller.SetState(PlayerState.Dash, restart);
        public void ClimbUp(bool restart = false)    => _controller.SetState(PlayerState.ClimbUp, restart);
        public void Dangling(bool restart = false)  => _controller.SetState(PlayerState.Dangling, restart);

        public void Update(GameTime gameTime) => _controller.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f, bool faceLeft = false)
        {
            if (!_controller.HasAnyState) return;
            var effects = faceLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _controller.Draw(spriteBatch, position, color, scale, effects);
        }

        public bool IsUsable => _controller.HasAnyState;
    }
}
