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

        // ===== Composite player sprite (body + procedural hair) =====
        private MaddySprite _maddy;

        // ===== Legacy player animation pack (kept for comparison, press H to toggle) =====
        private PlayerAnimations _playerAnims;
        private bool _useComposite = true;

        // ===== Player movement data =====
        // Position represents the character's feet (center-bottom origin),
        // matching the original Celeste's Justify=(0.5, 1.0) convention.
        private Vector2 _playerPos = new Vector2(232, 264);
        private float _moveSpeed = 150f;
        private bool _faceLeft = false;

        // ===== Toggle debounce =====
        private bool _hWasDown = false;
        private bool _gWasDown = false;
        private bool _fWasDown = false;
        private bool _useIdleA = false;

        // ===== Debug overlay (press G to toggle) =====
        private bool _showDebug = false;
        private Texture2D _pixelTexture;
        private int _debugLastFrame = -1;
        private string _debugLastAnim = "";

        // ===== Debug frame-step & delta-nudge =====
        private bool _showCrosshair = true;
        private bool _cWasDown = false;
        private bool _pWasDown = false;
        private bool _rightWasDown = false;
        private bool _leftWasDown = false;
        private bool _wNudgeWasDown = false;
        private bool _sNudgeWasDown = false;
        private bool _aNudgeWasDown = false;
        private bool _dNudgeWasDown = false;
        private bool _tabWasDown = false;
        private bool _shiftTabWasDown = false;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Build the new composite sprite (body + procedural hair).
            _maddy = MaddySprite.Build(Content, GraphicsDevice);

            // Keep the legacy animation pack so the team can compare.
            _playerAnims = PlayerAnimations.Build(Content);

            // TODO: Re-add items once ItemAnimations is rebuilt on the new clip system.

            // 1x1 white pixel for debug drawing.
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

            // ===== H: toggle between composite and legacy rendering =====
            bool hDown = kb.IsKeyDown(Keys.H);
            if (hDown && !_hWasDown)
                _useComposite = !_useComposite;
            _hWasDown = hDown;

            // ===== G: toggle debug overlay (shows hair anchor crosshair) =====
            bool gDown = kb.IsKeyDown(Keys.G);
            if (gDown && !_gWasDown)
                _showDebug = !_showDebug;
            _gWasDown = gDown;

            // ===== F: toggle idle vs idleA for testing fidget animation =====
            bool fDown = kb.IsKeyDown(Keys.F);
            if (fDown && !_fWasDown)
            {
                _useIdleA = !_useIdleA;
                Console.WriteLine(_useIdleA ? "Idle -> IdleA" : "IdleA -> Idle");
            }
            _fWasDown = fDown;

            // ===== Debug controls (only active when G debug overlay is on) =====
            if (_showDebug && _useComposite)
            {
                // C: toggle crosshair visibility
                bool cDown = kb.IsKeyDown(Keys.C);
                if (cDown && !_cWasDown)
                    _showCrosshair = !_showCrosshair;
                _cWasDown = cDown;

                // P: pause / resume animation
                bool pDown = kb.IsKeyDown(Keys.P);
                if (pDown && !_pWasDown)
                {
                    _maddy.DebugPauseToggle();
                    Console.WriteLine(_maddy.DebugPaused ? ">>> PAUSED <<<" : ">>> RESUMED <<<");
                }
                _pWasDown = pDown;

                if (_maddy.DebugPaused)
                {
                    // Right arrow: step forward one frame
                    bool rightDown = kb.IsKeyDown(Keys.Right);
                    if (rightDown && !_rightWasDown)
                    {
                        _maddy.DebugStepFrame(+1);
                        Console.WriteLine($"  stepped -> frame {_maddy.DebugFrame}");
                    }
                    _rightWasDown = rightDown;

                    // Left arrow: step backward one frame
                    bool leftDown = kb.IsKeyDown(Keys.Left);
                    if (leftDown && !_leftWasDown)
                    {
                        _maddy.DebugStepFrame(-1);
                        Console.WriteLine($"  stepped -> frame {_maddy.DebugFrame}");
                    }
                    _leftWasDown = leftDown;

                    // Tab: cycle to next animation (Shift+Tab = previous)
                    bool tabDown = kb.IsKeyDown(Keys.Tab);
                    bool shiftHeld = kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift);
                    if (tabDown && !_tabWasDown)
                    {
                        _maddy.DebugCycleAnimation(shiftHeld ? -1 : +1);
                        Console.WriteLine($">>> switched to: {_maddy.DebugAnimName} ({_maddy.DebugFrame + 1} frames) <<<");
                    }
                    _tabWasDown = tabDown;

                    // W/S: nudge delta Y up / down (-1/+1 native pixel)
                    bool wNudge = kb.IsKeyDown(Keys.W);
                    if (wNudge && !_wNudgeWasDown)
                    {
                        _maddy.DebugNudge += new Vector2(0, -1);
                        PrintNudge();
                    }
                    _wNudgeWasDown = wNudge;

                    bool sNudge = kb.IsKeyDown(Keys.S);
                    if (sNudge && !_sNudgeWasDown)
                    {
                        _maddy.DebugNudge += new Vector2(0, 1);
                        PrintNudge();
                    }
                    _sNudgeWasDown = sNudge;

                    // A/D: nudge delta X left / right (-1/+1 native pixel)
                    bool aNudge = kb.IsKeyDown(Keys.A);
                    if (aNudge && !_aNudgeWasDown)
                    {
                        _maddy.DebugNudge += new Vector2(-1, 0);
                        PrintNudge();
                    }
                    _aNudgeWasDown = aNudge;

                    bool dNudge = kb.IsKeyDown(Keys.D);
                    if (dNudge && !_dNudgeWasDown)
                    {
                        _maddy.DebugNudge += new Vector2(1, 0);
                        PrintNudge();
                    }
                    _dNudgeWasDown = dNudge;
                }
            }

            bool isMoving = false;
            bool debugPaused = _showDebug && _useComposite && _maddy.DebugPaused;

            // ===== A: run left (disabled when debug-paused; WASD does nudge instead) =====
            if (!debugPaused && kb.IsKeyDown(Keys.A))
            {
                _playerPos.X -= _moveSpeed * dt;
                _faceLeft = true;
                _maddy.Run();
                _playerAnims.Run();
                isMoving = true;
            }

            // ===== D: run right =====
            if (!debugPaused && kb.IsKeyDown(Keys.D))
            {
                _playerPos.X += _moveSpeed * dt;
                _faceLeft = false;
                _maddy.Run();
                _playerAnims.Run();
                isMoving = true;
            }

            // ===== T: test climb animation =====
            if (!debugPaused && kb.IsKeyDown(Keys.T))
            {
                _faceLeft = false;
                _maddy.ClimbUp();
                _playerAnims.ClimbUp();
                isMoving = true;
            }

            // ===== Idle when no movement (skip when debug-paused to freeze state) =====
            if (!isMoving && !debugPaused)
            {
                if (_useIdleA)
                    _maddy.IdleA();
                else
                    _maddy.Idle();
                _playerAnims.Idle();
            }

            // ===== Update both systems =====
            _maddy.SetPosition(_playerPos, scale: 2f, faceLeft: _faceLeft);
            _maddy.Update(gameTime);

            _playerAnims.Update(gameTime);

            base.Update(gameTime);
        }

        private void PrintNudge()
        {
            Vector2 stored = _maddy.DebugDelta;
            Vector2 nudge = _maddy.DebugNudge;
            Vector2 effective = stored + nudge;
            Console.WriteLine(
                $"  [f{_maddy.DebugFrame}] stored=({stored.X},{stored.Y}) + nudge=({nudge.X},{nudge.Y}) => effective=({effective.X},{effective.Y})");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // CullNone is required because BodySprite flips via negative X scale
            // (matching Celeste's Scale.X *= Facing). Negative scale reverses
            // triangle winding, which the default CullCounterClockwise would cull.
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            // ===== Draw player (press H to toggle composite vs legacy) =====
            if (_useComposite)
            {
                // MaddySprite uses center-bottom origin; _playerPos = feet.
                _maddy.Draw(_spriteBatch, _playerPos, Color.White, scale: 2f, faceLeft: _faceLeft);
            }
            else
            {
                // Legacy system uses top-left origin; convert feet back to top-left.
                Vector2 legacyPos = _playerPos - new Vector2(16 * 2f, 32 * 2f);
                _playerAnims.Draw(_spriteBatch, legacyPos, Color.White, scale: 2f, faceLeft: _faceLeft);
            }

            // ===== G: debug overlay -- hair anchor crosshair + info =====
            if (_showDebug && _useComposite)
            {
                Vector2 anchor = _maddy.LastHairAnchor;

                if (_showCrosshair)
                {
                    // Draw a small crosshair: 5px horizontal + 5px vertical, lime green.
                    _spriteBatch.Draw(_pixelTexture,
                        new Vector2(anchor.X - 4f, anchor.Y),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(9f, 1f),
                        SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_pixelTexture,
                        new Vector2(anchor.X, anchor.Y - 4f),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(1f, 9f),
                        SpriteEffects.None, 0f);

                    // Also mark the feet position for reference (yellow dot).
                    _spriteBatch.Draw(_pixelTexture,
                        _playerPos - Vector2.One,
                        null, Color.Yellow, 0f, Vector2.Zero, 3f,
                        SpriteEffects.None, 0f);
                }

                // Show debug numbers in the window title bar + console.
                string dir = _maddy.DebugFaceLeft ? "L" : "R";
                Vector2 d = _maddy.DebugDelta;      // stored delta from HairOffsetData
                Vector2 n = _maddy.DebugNudge;       // live nudge (IJKL)
                Vector2 eff = d + n;                  // effective delta
                int curFrame = _maddy.DebugFrame;
                string curAnim = _maddy.DebugAnimName;
                string pauseTag = _maddy.DebugPaused ? " PAUSED" : "";

                string info = string.Format(
                    "[{0} f{1}] stored=({2},{3}) nudge=({4},{5}) eff=({6},{7}) anchor=({8:F0},{9:F0}) {10}{11}",
                    curAnim, curFrame,
                    d.X, d.Y,
                    n.X, n.Y,
                    eff.X, eff.Y,
                    anchor.X, anchor.Y, dir, pauseTag);

                Window.Title = info;

                // Print to console only when frame or animation changes (avoids spam).
                if (curFrame != _debugLastFrame || curAnim != _debugLastAnim)
                {
                    Console.WriteLine(info);
                    _debugLastFrame = curFrame;
                    _debugLastAnim = curAnim;
                }
            }
            else if (_showDebug)
            {
                Window.Title = "Debug: switch to composite mode (H)";
            }
            else
            {
                Window.Title = "Celeste";
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
