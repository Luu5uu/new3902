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
            _player.CurrentGroundBlock = null;

            float attemptedDx = _player.position.X - prevPos.X;
            float attemptedDy = _player.position.Y - prevPos.Y;
            bool wasTouchingLeftWall = _player.touchingLeftWall;
            bool wasTouchingRightWall = _player.touchingRightWall;

            _player.hitCeiling = false;
            ResolveHorizontal(prevPos, attemptedDx);
            ResolveVertical(prevPos, attemptedDy);
            ResolveRemainingPenetration(prevPos, attemptedDx, attemptedDy);
            UpdateWallContacts();
            ResolveLedgeTopOut(prevPos, attemptedDy, wasTouchingLeftWall, wasTouchingRightWall);
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

        public void ResolveHorizontal(Vector2 prevPos, float attemptedDx)
        {
            _player.touchingLeftWall = false;
            _player.touchingRightWall = false;

            if (attemptedDx > 0)
            {
                ResolveHitLeftWall(prevPos, attemptedDx);
            }
            else if (attemptedDx < 0)
            {
                ResolveHitRightWall(prevPos, attemptedDx);
            }
        }



        public void ResolveVertical(Vector2 prevPos, float attemptedDy)
        {
            _player.onGround = false;

            
            ResolveStandOnTop(prevPos, attemptedDy);

            
            ResolveHitCeiling(prevPos, attemptedDy);
        }



        
        private void ResolveStandOnTop(Vector2 prevPos, float attemptedDy)
        {
            IBlocks bestBlock = null;

            if (attemptedDy <= 0f) return;

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
                        
                        bestBlock = blk;
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

                _player.CurrentGroundBlock = bestBlock;
            }
        }

        
        private void ResolveHitCeiling(Vector2 prevPos, float attemptedDy)
        {
            if (attemptedDy >= 0f) return;

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
                _player.touchingRightWall = true;
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
                _player.touchingLeftWall = true;
            }
        }

        private void ResolveRemainingPenetration(Vector2 prevPos, float attemptedDx, float attemptedDy)
        {
            Rectangle p = _player.Bounds;
            Rectangle prevBounds = GetBoundsAt(prevPos);

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
                bool cameFromAbove = prevBounds.Bottom <= r.Top;
                bool cameFromBelow = prevBounds.Top >= r.Bottom;
                bool cameFromLeft = prevBounds.Right <= r.Left;
                bool cameFromRight = prevBounds.Left >= r.Right;

                if (System.Math.Abs(attemptedDy) >= System.Math.Abs(attemptedDx) && minVertical <= minHorizontal)
                {
                    if (cameFromBelow || attemptedDy < 0f || _player.hitCeiling)
                    {
                        _player.position.Y = r.Bottom + p.Height;
                        _player.velocityY = 0f;
                        _player.hitCeiling = true;
                    }
                    else if (cameFromAbove || attemptedDy > 0f || _player.onGround)
                    {
                        _player.position.Y = r.Top;
                        _player.velocityY = 0f;
                        _player.onGround = true;
                    }
                    else if (overlapBottom < overlapTop)
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
                    if (cameFromLeft || attemptedDx > 0f)
                    {
                        _player.position.X = r.Left - p.Width / 2f;
                        _player.touchingRightWall = true;
                    }
                    else if (cameFromRight || attemptedDx < 0f)
                    {
                        _player.position.X = r.Right + p.Width / 2f;
                        _player.touchingLeftWall = true;
                    }
                    else if (overlapLeft <= overlapRight)
                    {
                        _player.position.X = r.Left - p.Width / 2f;
                        _player.touchingRightWall = true;
                    }
                    else
                    {
                        _player.position.X = r.Right + p.Width / 2f;
                        _player.touchingLeftWall = true;
                    }

                    _player.velocityX = 0f;
                }

                p = _player.Bounds;
            }
        }

        private void ResolveLedgeTopOut(Vector2 prevPos, float attemptedDy, bool wasTouchingLeftWall, bool wasTouchingRightWall)
        {
            if (!_player.isClimbing || _player.moveY >= 0f || attemptedDy >= 0f)
            {
                return;
            }

            Rectangle p = _player.Bounds;
            bool climbingWallOnLeft = (_player.touchingLeftWall || wasTouchingLeftWall)
                && !(_player.touchingRightWall || wasTouchingRightWall);
            bool climbingWallOnRight = (_player.touchingRightWall || wasTouchingRightWall)
                && !(_player.touchingLeftWall || wasTouchingLeftWall);
            if (!climbingWallOnLeft && !climbingWallOnRight)
            {
                return;
            }

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                bool nearTop = p.Top <= r.Top + PlayerConstants.PlayerLedgeTopOutHeightWindow
                    && p.Bottom >= r.Top;
                if (!nearTop)
                {
                    continue;
                }

                bool matchesWallSide =
                    (climbingWallOnRight && p.Right >= r.Left - 6 && p.Right <= r.Left + 6)
                    || (climbingWallOnLeft && p.Left <= r.Right + 6 && p.Left >= r.Right - 6);
                if (!matchesWallSide)
                {
                    continue;
                }

                float targetX = climbingWallOnRight
                    ? r.Left + p.Width / 2f + PlayerConstants.PlayerLedgeTopOutInset
                    : r.Right - p.Width / 2f - PlayerConstants.PlayerLedgeTopOutInset;
                Vector2 targetPosition = new Vector2(targetX, r.Top);
                Rectangle targetBounds = GetBoundsAt(targetPosition);

                if (!IsStandingSpotClear(targetBounds, blk))
                {
                    continue;
                }

                _player.QueueLedgeTopOut(targetPosition);
                break;
            }
        }

        private bool IsStandingSpotClear(Rectangle targetBounds, IBlocks supportBlock)
        {
            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null || ReferenceEquals(blk, supportBlock)) continue;

                Rectangle r = blk.Bounds;
                if (r == Rectangle.Empty) continue;

                if (targetBounds.Intersects(r))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
