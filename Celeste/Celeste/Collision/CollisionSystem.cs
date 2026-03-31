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
            _player.hitCeiling = false;
            ResolveHorizontal(prevPos);
            ResolveVertical(prevPos);
            ResolveRemainingPenetration(prevPos);
            UpdateWallContacts();

            

        }
        private void UpdateWallContacts()
        {
            _player.touchingLeftWall = false;
            _player.touchingRightWall = false;

            Rectangle p = _player.Bounds;

            Rectangle leftProbe = new Rectangle(
                p.Left - 1,
                p.Top + 1,
                1,
                p.Height - 2
            );

            Rectangle rightProbe = new Rectangle(
                p.Right,
                p.Top + 1,
                1,
                p.Height - 2
            );

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                if (leftProbe.Intersects(r))
                    _player.touchingLeftWall = true;

                if (rightProbe.Intersects(r))
                    _player.touchingRightWall = true;

                if (_player.touchingLeftWall && _player.touchingRightWall)
                    break;
            }
        }

        private Rectangle GetBoundsAt(Vector2 position)
        {
            Rectangle current = _player.Bounds;
            int left = (int)(position.X - current.Width / 2f);
            int top = (int)(position.Y - current.Height);
            return new Rectangle(left, top, current.Width, current.Height);
        }

        public void ResolveHorizontal(Vector2 prevPos)
        {
            _player.touchingLeftWall = false;
            _player.touchingRightWall = false;
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
            if (_player.position.Y <= prevPos.Y) return;

            Rectangle prevBounds = GetBoundsAt(prevPos);
            Rectangle p = _player.Bounds;
            Rectangle swept = Rectangle.Union(prevBounds, p);

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

                
                bool xOverlap = swept.Right > r.Left && swept.Left < r.Right;
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
            if (_player.position.Y >= prevPos.Y) return;

            Rectangle prevBounds = GetBoundsAt(prevPos);
            Rectangle p = _player.Bounds;
            Rectangle swept = Rectangle.Union(prevBounds, p);

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

                
                bool xOverlap = swept.Right > r.Left && swept.Left < r.Right;
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
                _player.hitCeiling = true;
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
                _player.velocityX = 0f;
                _player.moveX = 0;
                _player.touchingLeftWall = true;
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
                _player.velocityX = 0f;
                _player.moveX = 0;
                _player.touchingRightWall = true;
            }
        }

        private void ResolveRemainingPenetration(Vector2 prevPos)
        {
            Rectangle p = _player.Bounds;
            float dx = _player.position.X - prevPos.X;
            float dy = _player.position.Y - prevPos.Y;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty || !p.Intersects(r)) continue;

                int overlapLeft = p.Right - r.Left;
                int overlapRight = r.Right - p.Left;
                int overlapTop = p.Bottom - r.Top;
                int overlapBottom = r.Bottom - p.Top;

                int minHorizontal = System.Math.Min(overlapLeft, overlapRight);
                int minVertical = System.Math.Min(overlapTop, overlapBottom);

                if (System.Math.Abs(dy) >= System.Math.Abs(dx) && minVertical <= minHorizontal)
                {
                    if (overlapBottom < overlapTop)
                    {
                        _player.position.Y = r.Bottom + p.Height;
                        _player.velocityY = 0f;
                        _player.hitCeiling = true;
                    }
                    else
                    {
                        _player.position.Y = r.Top;
                        _player.velocityY = 0f;
                        _player.onGround = true;
                    }
                }
                else
                {
                    if (dx > 0f)
                    {
                        _player.position.X = r.Left - p.Width / 2f;
                        _player.touchingLeftWall = true;
                    }
                    else
                    {
                        _player.position.X = r.Right + p.Width / 2f;
                        _player.touchingRightWall = true;
                    }

                    _player.velocityX = 0f;
                }

                p = _player.Bounds;
            }
        }
    }
}
