using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Animation;
using Celeste.Sprites;

namespace Celeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AnimationCatalog _catalog;
        private MaddySprite _maddy;
        private PlayerAnimations _playerAnims;  // legacy (press H to compare)
        private bool _useComposite = true;

        // Position = feet (center-bottom origin, matches Celeste's Justify=(0.5,1.0)).
        private Vector2 _playerPos = new Vector2(232, 264);
        private float _moveSpeed = 150f;
        private bool _faceLeft = false;

        // Key debounce
        private bool _hWasDown, _gWasDown, _fWasDown;
        private bool _useIdleA = false;

        // Debug overlay (G)
        private bool _showDebug = false;
        private Texture2D _pixelTexture;
        private int _debugLastFrame = -1;
        private string _debugLastAnim = "";

        // Debug frame-step & nudge
        private bool _showCrosshair = true;
        private bool _cWasDown, _pWasDown, _rightWasDown, _leftWasDown;
        private bool _wNudgeWasDown, _sNudgeWasDown, _aNudgeWasDown, _dNudgeWasDown;
        private bool _tabWasDown;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _catalog = AnimationLoader.LoadAll(Content);
            _maddy = MaddySprite.Build(Content, _catalog, GraphicsDevice);
            _playerAnims = PlayerAnimations.Build(_catalog);
            // TODO: Re-add items once ItemAnimations is rebuilt.

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kb = Keyboard.GetState();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // H: composite vs legacy toggle
            bool hDown = kb.IsKeyDown(Keys.H);
            if (hDown && !_hWasDown) _useComposite = !_useComposite;
            _hWasDown = hDown;

            // G: debug overlay
            bool gDown = kb.IsKeyDown(Keys.G);
            if (gDown && !_gWasDown) _showDebug = !_showDebug;
            _gWasDown = gDown;

            // F: idle vs idleA
            bool fDown = kb.IsKeyDown(Keys.F);
            if (fDown && !_fWasDown)
            {
                _useIdleA = !_useIdleA;
                Console.WriteLine(_useIdleA ? "Idle -> IdleA" : "IdleA -> Idle");
            }
            _fWasDown = fDown;

            // --- Debug controls (only when G overlay is on) ---
            if (_showDebug && _useComposite)
            {
                bool cDown = kb.IsKeyDown(Keys.C);
                if (cDown && !_cWasDown) _showCrosshair = !_showCrosshair;
                _cWasDown = cDown;

                bool pDown = kb.IsKeyDown(Keys.P);
                if (pDown && !_pWasDown)
                {
                    _maddy.DebugPauseToggle();
                    Console.WriteLine(_maddy.DebugPaused ? ">>> PAUSED <<<" : ">>> RESUMED <<<");
                }
                _pWasDown = pDown;

                if (_maddy.DebugPaused)
                {
                    // Arrow keys: frame step
                    bool rDown = kb.IsKeyDown(Keys.Right);
                    if (rDown && !_rightWasDown) { _maddy.DebugStepFrame(+1); Console.WriteLine($"  stepped -> frame {_maddy.DebugFrame}"); }
                    _rightWasDown = rDown;

                    bool lDown = kb.IsKeyDown(Keys.Left);
                    if (lDown && !_leftWasDown) { _maddy.DebugStepFrame(-1); Console.WriteLine($"  stepped -> frame {_maddy.DebugFrame}"); }
                    _leftWasDown = lDown;

                    // Tab: cycle animation
                    bool tabDown = kb.IsKeyDown(Keys.Tab);
                    bool shiftHeld = kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift);
                    if (tabDown && !_tabWasDown)
                    {
                        _maddy.DebugCycleAnimation(shiftHeld ? -1 : +1);
                        Console.WriteLine($">>> switched to: {_maddy.DebugAnimName} <<<");
                    }
                    _tabWasDown = tabDown;

                    // WASD: nudge delta
                    bool wN = kb.IsKeyDown(Keys.W);
                    if (wN && !_wNudgeWasDown) { _maddy.DebugNudge += new Vector2(0, -1); PrintNudge(); }
                    _wNudgeWasDown = wN;

                    bool sN = kb.IsKeyDown(Keys.S);
                    if (sN && !_sNudgeWasDown) { _maddy.DebugNudge += new Vector2(0, 1); PrintNudge(); }
                    _sNudgeWasDown = sN;

                    bool aN = kb.IsKeyDown(Keys.A);
                    if (aN && !_aNudgeWasDown) { _maddy.DebugNudge += new Vector2(-1, 0); PrintNudge(); }
                    _aNudgeWasDown = aN;

                    bool dN = kb.IsKeyDown(Keys.D);
                    if (dN && !_dNudgeWasDown) { _maddy.DebugNudge += new Vector2(1, 0); PrintNudge(); }
                    _dNudgeWasDown = dN;
                }
            }

            bool isMoving = false;
            bool debugPaused = _showDebug && _useComposite && _maddy.DebugPaused;

            if (!debugPaused && kb.IsKeyDown(Keys.A))
            {
                _playerPos.X -= _moveSpeed * dt;
                _faceLeft = true;
                _maddy.Run(); _playerAnims.Run();
                isMoving = true;
            }
            if (!debugPaused && kb.IsKeyDown(Keys.D))
            {
                _playerPos.X += _moveSpeed * dt;
                _faceLeft = false;
                _maddy.Run(); _playerAnims.Run();
                isMoving = true;
            }
            if (!debugPaused && kb.IsKeyDown(Keys.T))
            {
                _faceLeft = false;
                _maddy.ClimbUp(); _playerAnims.ClimbUp();
                isMoving = true;
            }

            if (!isMoving && !debugPaused)
            {
                if (_useIdleA) _maddy.IdleA(); else _maddy.Idle();
                _playerAnims.Idle();
            }

            _maddy.SetPosition(_playerPos, scale: 2f, faceLeft: _faceLeft);
            _maddy.Update(gameTime);
            _playerAnims.Update(gameTime);

            base.Update(gameTime);
        }

        private void PrintNudge()
        {
            Vector2 s = _maddy.DebugDelta, n = _maddy.DebugNudge, e = s + n;
            Console.WriteLine($"  [f{_maddy.DebugFrame}] stored=({s.X},{s.Y}) + nudge=({n.X},{n.Y}) => effective=({e.X},{e.Y})");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // CullNone needed: BodySprite flips via negative X scale.
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            if (_useComposite || !_playerAnims.IsUsable)
                _maddy.Draw(_spriteBatch, _playerPos, Color.White, scale: 2f, faceLeft: _faceLeft);
            else
            {
                Vector2 legacyPos = _playerPos - new Vector2(16 * 2f, 32 * 2f);
                _playerAnims.Draw(_spriteBatch, legacyPos, Color.White, scale: 2f, faceLeft: _faceLeft);
            }

            // --- Debug overlay ---
            if (_showDebug && _useComposite)
            {
                Vector2 anchor = _maddy.LastHairAnchor;

                if (_showCrosshair)
                {
                    _spriteBatch.Draw(_pixelTexture, new Vector2(anchor.X - 4f, anchor.Y),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(9f, 1f), SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_pixelTexture, new Vector2(anchor.X, anchor.Y - 4f),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(1f, 9f), SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_pixelTexture, _playerPos - Vector2.One,
                        null, Color.Yellow, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
                }

                string dir = _maddy.DebugFaceLeft ? "L" : "R";
                Vector2 d = _maddy.DebugDelta, n = _maddy.DebugNudge, eff = d + n;
                int frame = _maddy.DebugFrame;
                string anim = _maddy.DebugAnimName;
                string pause = _maddy.DebugPaused ? " PAUSED" : "";

                string info = $"[{anim} f{frame}] stored=({d.X},{d.Y}) nudge=({n.X},{n.Y}) eff=({eff.X},{eff.Y}) anchor=({anchor.X:F0},{anchor.Y:F0}) {dir}{pause}";
                Window.Title = info;

                if (frame != _debugLastFrame || anim != _debugLastAnim)
                {
                    Console.WriteLine(info);
                    _debugLastFrame = frame;
                    _debugLastAnim = anim;
                }
            }
            else if (_showDebug)
                Window.Title = "Debug: switch to composite mode (H)";
            else
                Window.Title = "Celeste";

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
