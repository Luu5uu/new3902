using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Character;

namespace Celeste.DevTools
{
    // Owns all debug overlay state and input; Game1 only creates and calls HandleInput/Draw.
    public sealed class DebugOverlay
    {
        private bool _showDebug;
        private bool _showCrosshair = true;
        private int _debugLastFrame = -1;
        private string _debugLastAnim = "";

        private bool _gWasDown, _cWasDown, _pWasDown;
        private bool _rightWasDown, _leftWasDown, _tabWasDown, _backspaceWasDown;
        private bool _wNudgeWasDown, _sNudgeWasDown, _aNudgeWasDown, _dNudgeWasDown;

        public bool ShowDebug => _showDebug;

        public void HandleInput(KeyboardState kb, Madeline player)
        {
            bool gDown = kb.IsKeyDown(Keys.G);
            if (gDown && !_gWasDown)
            {
                _showDebug = !_showDebug;
                if (!_showDebug)
                {
                    player.Maddy.DebugNudge = Vector2.Zero;
                    if (player.Maddy.DebugPaused) player.Maddy.DebugPauseToggle();
                }
            }
            _gWasDown = gDown;

            if (!_showDebug) return;

            bool cDown = kb.IsKeyDown(Keys.C);
            if (cDown && !_cWasDown) _showCrosshair = !_showCrosshair;
            _cWasDown = cDown;

            bool pDown = kb.IsKeyDown(Keys.P);
            if (pDown && !_pWasDown)
            {
                player.Maddy.DebugPauseToggle();
                Console.WriteLine(player.Maddy.DebugPaused ? ">>> PAUSED <<<" : ">>> RESUMED <<<");
            }
            _pWasDown = pDown;

            if (!player.Maddy.DebugPaused) return;

            bool rDown = kb.IsKeyDown(Keys.Right);
            if (rDown && !_rightWasDown) { player.Maddy.DebugStepFrame(+1); Console.WriteLine($"  stepped -> frame {player.Maddy.DebugFrame}"); }
            _rightWasDown = rDown;

            bool lDown = kb.IsKeyDown(Keys.Left);
            if (lDown && !_leftWasDown) { player.Maddy.DebugStepFrame(-1); Console.WriteLine($"  stepped -> frame {player.Maddy.DebugFrame}"); }
            _leftWasDown = lDown;

            bool tabDown = kb.IsKeyDown(Keys.Tab);
            bool shiftHeld = kb.IsKeyDown(Keys.LeftShift) || kb.IsKeyDown(Keys.RightShift);
            if (tabDown && !_tabWasDown)
            {
                player.Maddy.DebugCycleAnimation(shiftHeld ? -1 : +1);
                Console.WriteLine($">>> switched to: {player.Maddy.DebugAnimName} <<<");
            }
            _tabWasDown = tabDown;

            bool backspaceDown = kb.IsKeyDown(Keys.Back);
            if (backspaceDown && !_backspaceWasDown)
            {
                player.Maddy.DebugCycleAnimation(-1);
                Console.WriteLine($">>> switched to: {player.Maddy.DebugAnimName} <<<");
            }
            _backspaceWasDown = backspaceDown;

            bool wN = kb.IsKeyDown(Keys.W);
            if (wN && !_wNudgeWasDown) { player.Maddy.DebugNudge += new Vector2(0, -1); PrintNudge(player); }
            _wNudgeWasDown = wN;

            bool sN = kb.IsKeyDown(Keys.S);
            if (sN && !_sNudgeWasDown) { player.Maddy.DebugNudge += new Vector2(0, 1); PrintNudge(player); }
            _sNudgeWasDown = sN;

            bool aN = kb.IsKeyDown(Keys.A);
            if (aN && !_aNudgeWasDown) { player.Maddy.DebugNudge += new Vector2(-1, 0); PrintNudge(player); }
            _aNudgeWasDown = aN;

            bool dN = kb.IsKeyDown(Keys.D);
            if (dN && !_dNudgeWasDown) { player.Maddy.DebugNudge += new Vector2(1, 0); PrintNudge(player); }
            _dNudgeWasDown = dN;
        }

        private static void PrintNudge(Madeline player)
        {
            var s = player.Maddy.DebugDelta;
            var n = player.Maddy.DebugNudge;
            var e = s + n;
            Console.WriteLine($"  [f{player.Maddy.DebugFrame}] stored=({s.X},{s.Y}) + nudge=({n.X},{n.Y}) => effective=({e.X},{e.Y})");
        }

        public void Draw(SpriteBatch spriteBatch, Madeline player, Texture2D pixelTexture, GameWindow window)
        {
            Vector2 anchor = player.Maddy.LastHairAnchor;

            if (_showCrosshair)
            {
                spriteBatch.Draw(pixelTexture, new Vector2(anchor.X - 4f, anchor.Y),
                    null, Color.Lime, 0f, Vector2.Zero, new Vector2(9f, 1f), SpriteEffects.None, 0f);
                spriteBatch.Draw(pixelTexture, new Vector2(anchor.X, anchor.Y - 4f),
                    null, Color.Lime, 0f, Vector2.Zero, new Vector2(1f, 9f), SpriteEffects.None, 0f);
                spriteBatch.Draw(pixelTexture, player.position - Vector2.One,
                    null, Color.Yellow, 0f, Vector2.Zero, 3f, SpriteEffects.None, 0f);
            }

            string dir   = player.Maddy.DebugFaceLeft ? "L" : "R";
            var d        = player.Maddy.DebugDelta;
            var n        = player.Maddy.DebugNudge;
            var eff      = d + n;
            int frame    = player.Maddy.DebugFrame;
            string anim  = player.Maddy.DebugAnimName;
            string pause = player.Maddy.DebugPaused ? " PAUSED" : "";

            string info = $"[{anim} f{frame}] stored=({d.X},{d.Y}) nudge=({n.X},{n.Y}) eff=({eff.X},{eff.Y}) anchor=({anchor.X:F0},{anchor.Y:F0}) {dir}{pause}";
            window.Title = info;

            if (frame != _debugLastFrame || anim != _debugLastAnim)
            {
                Console.WriteLine(info);
                _debugLastFrame = frame;
                _debugLastAnim  = anim;
            }
        }
    }
}
