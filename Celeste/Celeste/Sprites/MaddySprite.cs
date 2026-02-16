using System.Collections.Generic;
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Composite sprite: hairless body + procedural hair, following Celeste's approach.
    public sealed class MaddySprite : IMaddySprite
    {
        private readonly BodySprite<PlayerState> _body;
        private readonly HairRenderer _hair;

        private bool _faceLeft;
        private string _currentAnimName = "idle";
        private readonly List<(PlayerState state, string name)> _allAnims = new();

        public IBodySprite Body => _body;
        public IHairSprite Hair => _hair;

        private MaddySprite(BodySprite<PlayerState> body, HairRenderer hair)
        {
            _body = body;
            _hair = hair;
        }

        // Factory: builds from catalog. Call once during LoadContent.
        // Overload without catalog loads the catalog internally (one place to load).
        public static MaddySprite Build(ContentManager content, GraphicsDevice graphicsDevice = null)
        {
            var catalog = AnimationLoader.LoadAll(content);
            return Build(content, catalog, graphicsDevice);
        }

        public static MaddySprite Build(ContentManager content, AnimationCatalog catalog, GraphicsDevice graphicsDevice = null)
        {
            var controller = new AnimationController<PlayerState>();
            var origin = new Vector2(16, 32); // center-bottom (feet), matches Celeste Justify=(0.5,1.0).

            RegisterFromClip(controller, catalog, AnimationKeys.PlayerIdle,         PlayerState.Idle,         setAsDefault: true);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerIdleFidgetA,  PlayerState.IdleFidgetA,  setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerRun,          PlayerState.Run,          setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerJumpFast,     PlayerState.JumpFast,     setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerFallSlow,       PlayerState.FallSlow,     setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerDash,          PlayerState.Dash,         setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerClimbUp,        PlayerState.ClimbUp,      setAsDefault: false);
            RegisterFromClip(controller, catalog, AnimationKeys.PlayerDangling,     PlayerState.Dangling,     setAsDefault: false);

            var body = new BodySprite<PlayerState>(controller);
            var hair = new HairRenderer();
            hair.LoadContent(content);

            var maddy = new MaddySprite(body, hair);

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

        private static void RegisterFromClip(AnimationController<PlayerState> controller, AnimationCatalog catalog,
            string clipKey, PlayerState state, bool setAsDefault)
        {
            var clip = catalog.Clips[clipKey];
            var anim = AutoAnimation.FromClip(clip);
            anim.Origin = new Vector2(16, 32);
            controller.Register(state, anim, setAsDefault: setAsDefault);
        }

        // --- Animation switching ---

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

        // --- Draw parameters for hair anchor ---

        private Vector2 _lastPosition;
        private float _lastScale = 1f;

        public Vector2 LastHairAnchor { get; private set; }

        // --- Debug (press G in Game1) ---

        public string DebugAnimName => _currentAnimName;
        public int DebugFrame => _body.CurrentFrame;
        public Vector2 DebugDelta { get; private set; }
        public bool DebugFaceLeft => _faceLeft;
        public Vector2 DebugNudge { get; set; } = Vector2.Zero;
        public bool DebugPaused { get; private set; }

        public void DebugPauseToggle()
        {
            DebugPaused = !DebugPaused;
            var anim = _body.Controller.Get(_body.Controller.CurrentState);
            if (DebugPaused) anim.Pause(); else anim.Play();
        }

        public void DebugStepFrame(int direction)
        {
            if (!DebugPaused) return;
            var anim = _body.Controller.Get(_body.Controller.CurrentState);
            anim.SetFrame(anim.CurrentFrame + direction);
        }

        public void DebugCycleAnimation(int direction)
        {
            if (!DebugPaused || _allAnims.Count == 0) return;

            int idx = _allAnims.FindIndex(a => a.name == _currentAnimName);
            if (idx < 0) idx = 0;
            idx = ((idx + direction) % _allAnims.Count + _allAnims.Count) % _allAnims.Count;

            var (state, name) = _allAnims[idx];
            _body.Controller.SetState(state, restart: true);
            _currentAnimName = name;

            var anim = _body.Controller.Get(state);
            anim.Pause();
            anim.SetFrame(0);
            DebugNudge = Vector2.Zero;
        }

        public void SetPosition(Vector2 position, float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;
        }

        // --- Update: body animation + hair anchor calculation ---

        // Native pixels from feet to crown. Matches Celeste's PlayerHair offset.
        private const float BaseHeadY = 12f;

        public void Update(GameTime gameTime)
        {
            _body.Update(gameTime);
            _hair.DrawScale = _lastScale;

            // Hair anchor = feetPos + head offset + per-frame delta.
            // For left-facing, negate X (Celeste: offset.X * Facing).
            Vector2 hairDelta = HairOffsetData.GetOffset(_currentAnimName, _body.CurrentFrame);
            DebugDelta = hairDelta;
            hairDelta += DebugNudge;

            float facing = _faceLeft ? -1f : 1f;
            hairDelta.X *= facing;

            Vector2 hairAnchor = _lastPosition
                + new Vector2(0f, -BaseHeadY * _lastScale)
                + hairDelta * _lastScale;

            LastHairAnchor = hairAnchor;
            _hair.BangsFrame = BangsFrameData.GetFrame(_currentAnimName, _body.CurrentFrame);
            _hair.Update(gameTime, hairAnchor, _faceLeft);
        }

        // --- Draw: hair behind body ---

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color,
                         float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;

            _hair.Draw(spriteBatch, Color.White, scale);
            _body.Draw(spriteBatch, position, color, scale, faceLeft);
        }
    }
}
