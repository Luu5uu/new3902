using Microsoft.Xna.Framework;
using Celeste.Animation;
using Celeste.Character;
using Celeste.Items;
using Celeste.Blocks;
using Celeste.DevTools;
using Celeste.Input;

using Celeste.DeathAnimation.Particles;
using System.Collections.Generic;
using Celeste.Utils;

namespace Celeste.Collision
{
    public class CollisionSystem
    {
        private readonly List<IBlocks> _worldBlocks;
        private readonly Madeline _player;

        public CollisionSystem(List<IBlocks> worldBlocks, Madeline player)
        {
            _worldBlocks = worldBlocks;
            _player = player;
        }



        public void ResolveBlockCollision(Vector2 prevPos)
        {
            ResolveHorizontal(prevPos);
            ResolveVertical(prevPos);

            

        }

       


        public void ResolveHorizontal(Vector2 prevPos)
        {
            float dx = _player.position.X - prevPos.X;

            if (dx > 0)
            {
                ResolveHitLeftWall(prevPos, dx);
            }
            else if (dx < 0)
            {
                ResolveHitRightWall(prevPos, dx);
            }
        }



        public void ResolveVertical(Vector2 prevPos)
        {
            _player.onGround = false;

            
            ResolveStandOnTop(prevPos);

            
            ResolveHitCeiling(prevPos);
        }



        
        private void ResolveStandOnTop(Vector2 prevPos)
        {
            
            if (_player.velocityY < 0) return;

            Rectangle p = _player.Bounds;

            int bestTop = int.MaxValue;
            bool found = false;

            float prevFootY = prevPos.Y;          
            float currFootY = _player.position.Y; 

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                
                bool xOverlap = p.Right > r.Left && p.Left < r.Right;
                if (!xOverlap) continue;

                
                if (prevFootY <= r.Top && currFootY >= r.Top)
                {
                    if (r.Top < bestTop)
                    {
                        bestTop = r.Top;
                        found = true;
                    }
                }
            }

            if (found)
            {
                _player.position.Y = bestTop;
                _player.velocityY = 0;
                _player.onGround = true;

                if (!_player.isDashing)
                {
                    _player.Maddy.OnDashRefill();
                    _player.canDash = true;
                }
            }
        }

        
        private void ResolveHitCeiling(Vector2 prevPos)
        {
            
            if (_player.velocityY >= 0) return;

            Rectangle p = _player.Bounds;

            int bestBottom = int.MinValue;
            bool found = false;

            
            float prevHeadY = prevPos.Y - p.Height;
            float currHeadY = _player.position.Y - p.Height;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                
                bool xOverlap = p.Right > r.Left && p.Left < r.Right;
                if (!xOverlap) continue;

                
                if (prevHeadY >= r.Bottom && currHeadY < r.Bottom)
                {
                    if (r.Bottom > bestBottom)
                    {
                        bestBottom = r.Bottom;
                        found = true;
                    }
                }
            }

            if (found)
            {
                _player.position.Y = bestBottom + p.Height;
                _player.velocityY = 0;
            }
        }



        private void ResolveHitLeftWall(Vector2 prevPos, float dx)
        {
            if (dx <= 0) return;

            Rectangle p = _player.Bounds;

            int bestLeft = int.MaxValue;
            bool found = false;

            float prevRightX = prevPos.X + p.Width / 2f;
            float currRightX = _player.position.X + p.Width / 2f;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                bool yOverlap = p.Bottom > r.Top && p.Top < r.Bottom;
                if (!yOverlap) continue;

                if (prevRightX <= r.Left && currRightX >= r.Left)
                {
                    if (r.Left < bestLeft)
                    {
                        bestLeft = r.Left;
                        found = true;
                    }
                }
            }

            if (found)
            {
                _player.position.X = bestLeft - p.Width / 2f;

                _player.moveX = 0;
            }
        }

        private void ResolveHitRightWall(Vector2 prevPos, float dx)
        {
            if (dx >= 0) return;

            Rectangle p = _player.Bounds;

            int bestRight = int.MinValue;
            bool found = false;

            float prevLeftX = prevPos.X - p.Width / 2f;
            float currLeftX = _player.position.X - p.Width / 2f;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                bool yOverlap = p.Bottom > r.Top && p.Top < r.Bottom;
                if (!yOverlap) continue;

                if (prevLeftX >= r.Right && currLeftX <= r.Right)
                {
                    if (r.Right > bestRight)
                    {
                        bestRight = r.Right;
                        found = true;
                    }
                }
            }

            if (found)
            {
                _player.position.X = bestRight + p.Width / 2f;

                _player.moveX = 0;
            }
        }
    }
}
