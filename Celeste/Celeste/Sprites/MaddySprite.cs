using System.Collections.Generic;
using Celeste.Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Sprites
{
    // Composite sprite: hairless body + procedural hair + sweat overlay.
    public sealed partial class MaddySprite : IMaddySprite
    {
        private enum SweatState
        {
            Idle,
            Still,
            Climb,
            ClimbLoop,
            Danger,
            Jump
        }

        private readonly BodySprite<PlayerState> _body;
        private readonly HairRenderer _hair;
        private readonly AnimationController<SweatState> _sweatController;

        private static readonly Color NormalHairColor = new Color(0xAC, 0x32, 0x32);
        private static readonly Color UsedHairColor = new Color(0x44, 0xB7, 0xFF);

        private bool _dashUsed = false;
        private float _hairFlashTimer = 0f;
        private float _hairUsedDisplayTimer = 0f;
        private bool _faceLeft;
        private bool _sweatVisible;
        private string _currentAnimName = "idled";
        private readonly List<(PlayerState state, string name)> _allAnims = new();

        private const float MinUsedDisplayTime = 0.35f;
        private const float BaseHeadY = 12f;
        private const float DuckHeadY = 7f;
        private const float TiredHeadY = 10f;

        public IBodySprite Body => _body;
        public IHairSprite Hair => _hair;
        public bool IsBodyAnimationFinished => _body.Controller.IsFinished;
        public PlayerState CurrentBodyState => _body.Controller.CurrentState;

        public Texture2D BodyAtlasTexture => _body.Controller.Get(_body.Controller.CurrentState).Texture;

        public (Rectangle Src, Vector2 Origin) BodyCurrentFrame
        {
            get
            {
                var anim = _body.Controller.Get(_body.Controller.CurrentState);
                return (anim.CurrentSourceRect, anim.Origin);
            }
        }

        private MaddySprite(
            BodySprite<PlayerState> body,
            HairRenderer hair,
            AnimationController<SweatState> sweatController)
        {
            _body = body;
            _hair = hair;
            _sweatController = sweatController;
        }

        public static MaddySprite Build(ContentManager content, GraphicsDevice graphicsDevice = null)
        {
            var catalog = AnimationLoader.LoadAll(content);
            return Build(content, catalog, graphicsDevice);
        }

        public static MaddySprite Build(ContentManager content, AnimationCatalog catalog, GraphicsDevice graphicsDevice = null)
        {
            var bodyController = new AnimationController<PlayerState>();
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerIdle, PlayerState.Idle, setAsDefault: true);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerIdleFidgetA, PlayerState.IdleFidgetA, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerIdleFidgetB, PlayerState.IdleFidgetB, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerIdleFidgetC, PlayerState.IdleFidgetC, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerRun, PlayerState.Run, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerJumpFast, PlayerState.JumpFast, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerFallSlow, PlayerState.FallSlow, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerDash, PlayerState.Dash, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerClimbUp, PlayerState.ClimbUp, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerDangling, PlayerState.Dangling, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerWallSlide, PlayerState.WallSlide, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerTired, PlayerState.Tired, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerTiredStill, PlayerState.TiredStill, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerClimbPull, PlayerState.ClimbPull, setAsDefault: false);
            RegisterFromClip(bodyController, catalog, AnimationKeys.PlayerDuck, PlayerState.Duck, setAsDefault: false);

            var sweatController = new AnimationController<SweatState>();
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatIdle, SweatState.Idle, setAsDefault: true);
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatStill, SweatState.Still, setAsDefault: false);
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatClimb, SweatState.Climb, setAsDefault: false);
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatClimbLoop, SweatState.ClimbLoop, setAsDefault: false);
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatDanger, SweatState.Danger, setAsDefault: false);
            RegisterFromClip(sweatController, catalog, AnimationKeys.PlayerSweatJump, SweatState.Jump, setAsDefault: false);

            var body = new BodySprite<PlayerState>(bodyController);
            var hair = new HairRenderer();
            hair.LoadContent(content);

            var maddy = new MaddySprite(body, hair, sweatController);
            maddy.SetHairContent(content);
            maddy.LoadHairConfigAndInit(catalog);

            maddy._allAnims.Add((PlayerState.Idle, "idled"));
            maddy._allAnims.Add((PlayerState.IdleFidgetA, "idlea"));
            maddy._allAnims.Add((PlayerState.IdleFidgetB, "idleb"));
            maddy._allAnims.Add((PlayerState.IdleFidgetC, "idlec"));
            maddy._allAnims.Add((PlayerState.Run, "run"));
            maddy._allAnims.Add((PlayerState.JumpFast, "jumpfast"));
            maddy._allAnims.Add((PlayerState.FallSlow, "fallslow"));
            maddy._allAnims.Add((PlayerState.Dash, "dash"));
            maddy._allAnims.Add((PlayerState.ClimbUp, "climbup"));
            maddy._allAnims.Add((PlayerState.Dangling, "dangling"));
            maddy._allAnims.Add((PlayerState.WallSlide, "wallslide"));
            maddy._allAnims.Add((PlayerState.Tired, "tired"));
            maddy._allAnims.Add((PlayerState.TiredStill, "tiredstill"));
            maddy._allAnims.Add((PlayerState.ClimbPull, "climbpull"));
            maddy._allAnims.Add((PlayerState.Duck, "duck"));
            return maddy;
        }

        private static void RegisterFromClip<TState>(
            AnimationController<TState> controller,
            AnimationCatalog catalog,
            string clipKey,
            TState state,
            bool setAsDefault) where TState : notnull
        {
            var clip = catalog.Clips[clipKey];
            var anim = AutoAnimation.FromClip(clip);
            anim.Origin = new Vector2(16, 32);
            controller.Register(state, anim, setAsDefault: setAsDefault);
        }

        private void SetAnimation(PlayerState state, string animName, bool restart = false)
        {
            _body.Controller.SetState(state, restart);
            _currentAnimName = animName;
        }

        private void SetSweat(SweatState state, bool restart = false)
        {
            _sweatVisible = true;
            _sweatController.SetState(state, restart);
        }

        public void ClearSweat()
        {
            _sweatVisible = false;
            _sweatController.SetState(SweatState.Idle, restart: true);
        }

        public void SweatIdle(bool restart = false) => SetSweat(SweatState.Idle, restart);
        public void SweatStill(bool restart = false) => SetSweat(SweatState.Still, restart);
        public void SweatClimb(bool restart = true) => SetSweat(SweatState.Climb, restart);
        public void SweatDanger(bool restart = false) => SetSweat(SweatState.Danger, restart);
        public void SweatJump(bool restart = true) => SetSweat(SweatState.Jump, restart);

        public void SetClimbSweat(bool climbingUp, bool tired, bool onGround)
        {
            if (tired)
            {
                SweatDanger();
            }
            else if (climbingUp)
            {
                if (_sweatController.CurrentState != SweatState.Climb && _sweatController.CurrentState != SweatState.ClimbLoop)
                    SweatClimb();
            }
            else if (onGround)
            {
                SweatIdle();
            }
            else
            {
                SweatStill();
            }
        }

        public void Idle(bool restart = false) { ClearSweat(); SetAnimation(PlayerState.Idle, "idled", restart); }
        public void IdleA(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.IdleFidgetA, "idlea", restart); }
        public void IdleB(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.IdleFidgetB, "idleb", restart); }
        public void IdleC(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.IdleFidgetC, "idlec", restart); }
        public void Run(bool restart = false) { ClearSweat(); SetAnimation(PlayerState.Run, "run", restart); }
        public void JumpFast(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.JumpFast, "jumpfast", restart); }
        public void FallSlow(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.FallSlow, "fallslow", restart); }
        public void Dash(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.Dash, "dash", restart); }
        public void ClimbUp(bool restart = false) => SetAnimation(PlayerState.ClimbUp, "climbup", restart);
        public void Dangling(bool restart = false) => SetAnimation(PlayerState.Dangling, "dangling", restart);
        public void WallSlide(bool restart = false) => SetAnimation(PlayerState.WallSlide, "wallslide", restart);
        public void Tired(bool restart = false) => SetAnimation(PlayerState.Tired, "tired", restart);
        public void TiredStill(bool restart = false) => SetAnimation(PlayerState.TiredStill, "tiredstill", restart);
        public void ClimbPull(bool restart = true) { ClearSweat(); SetAnimation(PlayerState.ClimbPull, "climbpull", restart); }
        public void Duck(bool restart = false) { ClearSweat(); SetAnimation(PlayerState.Duck, "duck", restart); }

        private Vector2 _lastPosition;
        private float _lastScale = 1f;

        public Vector2 LastHairAnchor { get; private set; }
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
            if (DebugPaused)
                anim.Pause();
            else
                anim.Play();
        }

        public void DebugStepFrame(int direction)
        {
            if (!DebugPaused)
                return;

            var anim = _body.Controller.Get(_body.Controller.CurrentState);
            anim.SetFrame(anim.CurrentFrame + direction);
        }

        public void DebugCycleAnimation(int direction)
        {
            if (!DebugPaused || _allAnims.Count == 0)
                return;

            int idx = _allAnims.FindIndex(a => a.name == _currentAnimName);
            if (idx < 0)
                idx = 0;
            idx = ((idx + direction) % _allAnims.Count + _allAnims.Count) % _allAnims.Count;

            var (state, name) = _allAnims[idx];
            _body.Controller.SetState(state, restart: true);
            _currentAnimName = name;

            var anim = _body.Controller.Get(state);
            anim.Pause();
            anim.SetFrame(0);
            DebugNudge = Vector2.Zero;
        }

        public void OnDashUsed()
        {
            _dashUsed = true;
            _hairFlashTimer = 0f;
            _hairUsedDisplayTimer = MinUsedDisplayTime;
            _hair.HairColor = UsedHairColor;
        }

        public void OnDashRefill()
        {
            if (!_dashUsed || _hairUsedDisplayTimer > 0f)
                return;

            _dashUsed = false;
            _hairFlashTimer = 0.12f;
        }

        public void SetPosition(Vector2 position, float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _body.Update(gameTime);

            if (_sweatVisible)
            {
                _sweatController.Update(gameTime);
                if (_sweatController.CurrentState == SweatState.Climb && _sweatController.IsFinished)
                    _sweatController.SetState(SweatState.ClimbLoop, restart: true);
                else if (_sweatController.CurrentState == SweatState.Jump && _sweatController.IsFinished)
                    _sweatVisible = false;
            }

            if (_hairUsedDisplayTimer > 0f)
                _hairUsedDisplayTimer -= dt;

            if (_hairFlashTimer > 0f)
            {
                _hair.HairColor = Color.White;
                _hairFlashTimer -= dt;
                if (_hairFlashTimer <= 0f)
                    _hair.HairColor = _dashUsed ? UsedHairColor : NormalHairColor;
            }

            Vector2 hairAnchor = GetCurrentHairAnchor();
            LastHairAnchor = hairAnchor;
            _hair.BangsFrame = BangsFrameData.GetFrame(_currentAnimName, _body.CurrentFrame);
            _hair.Update(gameTime, hairAnchor, _faceLeft);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float scale = 1f, bool faceLeft = false)
        {
            _lastPosition = position;
            _lastScale = scale;
            _faceLeft = faceLeft;

            _hair.Draw(spriteBatch, Color.White, scale);
            _body.Draw(spriteBatch, position, color, scale, faceLeft);

            if (_sweatVisible)
            {
                float scaleX = faceLeft ? -scale : scale;
                _sweatController.Draw(spriteBatch, position, Color.White, new Vector2(scaleX, scale));
            }
        }

        private float GetBaseHeadY()
        {
            return _currentAnimName switch
            {
                "duck" => DuckHeadY,
                "tired" or "tiredstill" => TiredHeadY,
                _ => BaseHeadY,
            };
        }

        private Vector2 GetCurrentHairAnchor()
        {
            _hair.DrawScale = _lastScale;
            Vector2 hairDelta = HairOffsetData.GetOffset(_currentAnimName, _body.CurrentFrame);
            DebugDelta = hairDelta;
            hairDelta += DebugNudge;

            float facing = _faceLeft ? -1f : 1f;
            hairDelta.X *= facing;

            return _lastPosition
                + new Vector2(0f, -GetBaseHeadY() * _lastScale)
                + hairDelta * _lastScale;
        }
    }
}
