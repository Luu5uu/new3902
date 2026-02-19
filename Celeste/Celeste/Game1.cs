using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Animation;
using Celeste.Character;
using Celeste.CollectableItems;

namespace Celeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AnimationCatalog _catalog;
        private Madeline _player;

        // Item animations
        private ItemAnimation _normalStawAnim;
        private ItemAnimation _flyStawAnim;
        private ItemAnimation _crystalAnim;
        private Vector2 _normalPos  = new Vector2(120f, 120f);
        private Vector2 _flyPos     = new Vector2(220f, 120f);
        private Vector2 _crystalPos = new Vector2(340f, 120f);

        // Key debounce
        private bool _gWasDown;

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

            var startPos = new Vector2(
                Window.ClientBounds.Width  / 2f,
                Window.ClientBounds.Height / 2f);
            _player = new Madeline(Content, _catalog, startPos);

            _normalStawAnim = ItemAnimationFactory.CreateNormalStaw(_catalog);
            _flyStawAnim    = ItemAnimationFactory.CreateFlyStaw(_catalog);
            _crystalAnim    = ItemAnimationFactory.CreateCrystal(_catalog);

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kb = Keyboard.GetState();

            // G: debug overlay (closing resets nudge and unpauses)
            bool gDown = kb.IsKeyDown(Keys.G);
            if (gDown && !_gWasDown)
            {
                _showDebug = !_showDebug;
                if (!_showDebug)
                {
                    _player.Maddy.DebugNudge = Vector2.Zero;
                    if (_player.Maddy.DebugPaused) _player.Maddy.DebugPauseToggle();
                }
            }
            _gWasDown = gDown;

            // --- Debug controls (only when G overlay is on) ---
            if (_showDebug)
            {
                bool cDown = kb.IsKeyDown(Keys.C);
                if (cDown && !_cWasDown) _showCrosshair = !_showCrosshair;
                _cWasDown = cDown;

                bool pDown = kb.IsKeyDown(Keys.P);
                if (pDown && !_pWasDown)
                {
                    _player.Maddy.DebugPauseToggle();
                    Console.WriteLine(_player.Maddy.DebugPaused ? ">>> PAUSED <<<" : ">>> RESUMED <<<");
                }
                _pWasDown = pDown;

                if (_player.Maddy.DebugPaused)
                {
                    bool rDown = kb.IsKeyDown(Keys.Right);
                    if (rDown && !_rightWasDown) { _player.Maddy.DebugStepFrame(+1); Console.WriteLine($"  stepped -> frame {_player.Maddy.DebugFrame}"); }
                    _rightWasDown = rDown;

                    bool lDown = kb.IsKeyDown(Keys.Left);
                    if (lDown && !_leftWasDown) { _player.Maddy.DebugStepFrame(-1); Console.WriteLine($"  stepped -> frame {_player.Maddy.DebugFrame}"); }
                    _leftWasDown = lDown;

                    bool tabDown = kb.IsKeyDown(Keys.Tab);
                    bool shiftHeld = kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift);
                    if (tabDown && !_tabWasDown)
                    {
                        _player.Maddy.DebugCycleAnimation(shiftHeld ? -1 : +1);
                        Console.WriteLine($">>> switched to: {_player.Maddy.DebugAnimName} <<<");
                    }
                    _tabWasDown = tabDown;

                    bool wN = kb.IsKeyDown(Keys.W);
                    if (wN && !_wNudgeWasDown) { _player.Maddy.DebugNudge += new Vector2(0, -1); PrintNudge(); }
                    _wNudgeWasDown = wN;

                    bool sN = kb.IsKeyDown(Keys.S);
                    if (sN && !_sNudgeWasDown) { _player.Maddy.DebugNudge += new Vector2(0, 1); PrintNudge(); }
                    _sNudgeWasDown = sN;

                    bool aN = kb.IsKeyDown(Keys.A);
                    if (aN && !_aNudgeWasDown) { _player.Maddy.DebugNudge += new Vector2(-1, 0); PrintNudge(); }
                    _aNudgeWasDown = aN;

                    bool dN = kb.IsKeyDown(Keys.D);
                    if (dN && !_dNudgeWasDown) { _player.Maddy.DebugNudge += new Vector2(1, 0); PrintNudge(); }
                    _dNudgeWasDown = dN;
                }
            }

            // Player physics + state machine (skipped while debug-paused)
            if (!(_showDebug && _player.Maddy.DebugPaused))
                _player.update(gameTime);

            _normalStawAnim.Update(gameTime);
            _flyStawAnim.Update(gameTime);
            _crystalAnim.Update(gameTime);

            base.Update(gameTime);
        }

        private void PrintNudge()
        {
            var s = _player.Maddy.DebugDelta;
            var n = _player.Maddy.DebugNudge;
            var e = s + n;
            Console.WriteLine($"  [f{_player.Maddy.DebugFrame}] stored=({s.X},{s.Y}) + nudge=({n.X},{n.Y}) => effective=({e.X},{e.Y})");
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // CullNone needed: BodySprite flips via negative X scale.
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            _player.draw(_spriteBatch);

            _normalStawAnim.Draw(_spriteBatch, _normalPos, scale: 2f);
            _flyStawAnim.Draw(_spriteBatch, _flyPos, scale: 2f);
            _crystalAnim.Draw(_spriteBatch, _crystalPos, scale: 2f);

            // --- Debug overlay ---
            if (_showDebug)
            {
                Vector2 anchor = _player.Maddy.LastHairAnchor;

                if (_showCrosshair)
                {
                    _spriteBatch.Draw(_pixelTexture, new Vector2(anchor.X - 4f, anchor.Y),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(9f, 1f), SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_pixelTexture, new Vector2(anchor.X, anchor.Y - 4f),
                        null, Color.Lime, 0f, Vector2.Zero, new Vector2(1f, 9f), SpriteEffects.None, 0f);
                    _spriteBatch.Draw(_pixelTexture, _player.position - Vector2.One,
                        null, Color.Yellow, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
                }

                string dir   = _player.Maddy.DebugFaceLeft ? "L" : "R";
                var d        = _player.Maddy.DebugDelta;
                var n        = _player.Maddy.DebugNudge;
                var eff      = d + n;
                int frame    = _player.Maddy.DebugFrame;
                string anim  = _player.Maddy.DebugAnimName;
                string pause = _player.Maddy.DebugPaused ? " PAUSED" : "";

                string info = $"[{anim} f{frame}] stored=({d.X},{d.Y}) nudge=({n.X},{n.Y}) eff=({eff.X},{eff.Y}) anchor=({anchor.X:F0},{anchor.Y:F0}) {dir}{pause}";
                Window.Title = info;

                if (frame != _debugLastFrame || anim != _debugLastAnim)
                {
                    Console.WriteLine(info);
                    _debugLastFrame = frame;
                    _debugLastAnim  = anim;
                }
            }
            else
                Window.Title = "Celeste";

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
