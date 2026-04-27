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
        private bool _playerWasOnGround;

        public CollisionSystem(List<IBlocks> worldBlocks, Madeline player)
        {
            _worldBlocks = worldBlocks;
            _player = player;
        }



        public void ResolveBlockCollision(Vector2 prevPos, bool prevCrouching, float dt)
        {
            _playerWasOnGround = _player.onGround;

            if (_player.isStarFlying)
            {
                ResolveStarFlyCollision(prevPos, prevCrouching);
                ActivateTouchedCrushBlocks();
                return;
            }

            CarryPlayerByMovingBlock(prevPos, prevCrouching, dt);
            _player.CurrentGroundBlock = null;

            float attemptedDx = _player.position.X - prevPos.X;
            float attemptedDy = _player.position.Y - prevPos.Y;
            bool wasTouchingLeftWall = _player.touchingLeftWall;
            bool wasTouchingRightWall = _player.touchingRightWall;

            _player.hitCeiling = false;
            ResolveHorizontal(prevPos, prevCrouching, attemptedDx);
            ResolveVertical(prevPos, prevCrouching, attemptedDy);
            ResolveRemainingPenetration(prevPos, prevCrouching, attemptedDx, attemptedDy);
            ResolveStandingSupport();
            ActivateTouchedSprings(prevPos, prevCrouching, attemptedDy);
            UpdateWallContacts();
            ResolveLedgeTopOut(prevPos, prevCrouching, attemptedDy, wasTouchingLeftWall, wasTouchingRightWall);
            ActivateTouchedCrushBlocks();
        }

        private void ResolveStarFlyCollision(Vector2 prevPos, bool prevCrouching)
        {
            Vector2 target = _player.position;
            Vector2 move = target - prevPos;
            float maxAxisMove = System.Math.Max(System.Math.Abs(move.X), System.Math.Abs(move.Y));
            int steps = System.Math.Max(1, (int)System.Math.Ceiling(maxAxisMove / PlayerConstants.PlayerStarFlyCollisionStep));
            Vector2 stepMove = move / steps;

            _player.position = prevPos;
            _player.onGround = false;
            _player.hitCeiling = false;

            for (int i = 0; i < steps; i++)
            {
                Vector2 stepStart = _player.position;
                Vector2 expected = stepStart + stepMove;
                Vector2 velocityBefore = new Vector2(_player.velocityX, _player.velocityY);

                _player.position = expected;

                ResolveHorizontal(stepStart, prevCrouching, stepMove.X);
                if (!_player.isStarFlying)
                {
                    return;
                }

                ResolveVertical(stepStart, prevCrouching, stepMove.Y);
                if (!_player.isStarFlying)
                {
                    return;
                }

                ResolveRemainingPenetration(stepStart, prevCrouching, stepMove.X, stepMove.Y);
                if (!_player.isStarFlying)
                {
                    return;
                }

                ResolveStandingSupport();
                UpdateWallContacts();

                Vector2 velocityAfter = new Vector2(_player.velocityX, _player.velocityY);
                bool velocityChanged = velocityAfter != velocityBefore;
                bool positionCorrected = _player.position != expected;
                if (velocityChanged || positionCorrected)
                {
                    break;
                }
            }
        }

        private void CarryPlayerByMovingBlock(Vector2 prevPos, bool prevCrouching, float dt)
        {
            if (_player.CurrentGroundBlock is not MoveBlock moveBlock)
            {
                return;
            }

            if (moveBlock.MovementDelta == Vector2.Zero)
            {
                return;
            }

            Rectangle playerPreviousBounds = GetBoundsAt(prevPos, prevCrouching);
            Rectangle blockPreviousBounds = moveBlock.PreviousBounds;
            if (blockPreviousBounds == Rectangle.Empty)
            {
                return;
            }

            bool feetOnTop = System.Math.Abs(playerPreviousBounds.Bottom - blockPreviousBounds.Top) <= 1;
            bool xOverlap = playerPreviousBounds.Right > blockPreviousBounds.Left
                && playerPreviousBounds.Left < blockPreviousBounds.Right;

            if (feetOnTop && xOverlap)
            {
                _player.position += moveBlock.MovementDelta;
                StoreMoveBlockLift(moveBlock, dt);
            }
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
                if (!CanStandOnBlock(blk)) continue;

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

        private Rectangle GetBoundsAt(Vector2 position, bool crouching)
        {
            return _player.GetBoundsAt(position, crouching);
        }

        public void ResolveHorizontal(Vector2 prevPos, bool prevCrouching, float attemptedDx)
        {
            _player.touchingLeftWall = false;
            _player.touchingRightWall = false;

            if (attemptedDx > 0)
            {
                ResolveHitLeftWall(prevPos, prevCrouching, attemptedDx);
            }
            else if (attemptedDx < 0)
            {
                ResolveHitRightWall(prevPos, prevCrouching, attemptedDx);
            }
        }



        public void ResolveVertical(Vector2 prevPos, bool prevCrouching, float attemptedDy)
        {
            _player.onGround = false;

            
            ResolveStandOnTop(prevPos, prevCrouching, attemptedDy);

            
            ResolveHitCeiling(prevPos, prevCrouching, attemptedDy);
        }



        
        private void ResolveStandOnTop(Vector2 prevPos, bool prevCrouching, float attemptedDy)
        {
            IBlocks bestBlock = null;

            if (attemptedDy <= 0f) return;

            Rectangle prevBounds = GetBoundsAt(prevPos, prevCrouching);
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
                if (!CanStandOnBlock(blk)) continue;

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
                LandOnBlock(bestBlock, bestTop);
            }
        }

        
        private void ResolveHitCeiling(Vector2 prevPos, bool prevCrouching, float attemptedDy)
        {
            if (attemptedDy >= 0f) return;

            Rectangle prevBounds = GetBoundsAt(prevPos, prevCrouching);
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
                if (_player.HandleStarFlyVerticalCollision())
                {
                    _player.hitCeiling = true;
                    return;
                }

                _player.velocityY = 0;
                _player.hitCeiling = true;
            }
        }



        private void ResolveHitLeftWall(Vector2 prevPos, bool prevCrouching, float dx)
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
                _player.touchingRightWall = true;
                if (_player.HandleStarFlyHorizontalCollision())
                {
                    return;
                }

                _player.velocityX = 0f;

                if (!_player.isDashing)
                {
                    _player.Maddy.OnDashRefill();
                    _player.canDash = true;
                }
            }
        }

        private void ResolveHitRightWall(Vector2 prevPos, bool prevCrouching, float dx)
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
                _player.touchingLeftWall = true;
                if (_player.HandleStarFlyHorizontalCollision())
                {
                    return;
                }

                _player.velocityX = 0f;

                if (!_player.isDashing)
                {
                    _player.Maddy.OnDashRefill();
                    _player.canDash = true;
                }
            }
        }

        private void ResolveRemainingPenetration(Vector2 prevPos, bool prevCrouching, float attemptedDx, float attemptedDy)
        {
            Rectangle p = _player.Bounds;
            Rectangle prevBounds = GetBoundsAt(prevPos, prevCrouching);

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

                if (ShouldIgnoreTopCollision(blk, r, p, prevBounds, attemptedDy))
                {
                    continue;
                }

                if (System.Math.Abs(attemptedDy) >= System.Math.Abs(attemptedDx) && minVertical <= minHorizontal)
                {
                    if (cameFromBelow || attemptedDy < 0f || _player.hitCeiling)
                    {
                        _player.position.Y = r.Bottom + p.Height;
                        if (_player.HandleStarFlyVerticalCollision())
                        {
                            _player.hitCeiling = true;
                            p = _player.Bounds;
                            continue;
                        }

                        _player.velocityY = 0f;
                        _player.hitCeiling = true;
                    }
                    else if (cameFromAbove || attemptedDy > 0f || _player.onGround)
                    {
                        if (!CanStandOnBlock(blk))
                        {
                            continue;
                        }

                        LandOnBlock(blk, r.Top);
                    }
                    else if (overlapBottom < overlapTop)
                    {
                        _player.position.Y = r.Bottom + p.Height;
                        if (_player.HandleStarFlyVerticalCollision())
                        {
                            _player.hitCeiling = true;
                            p = _player.Bounds;
                            continue;
                        }

                        _player.velocityY = 0f;
                        _player.hitCeiling = true;
                    }
                    else
                    {
                        if (!CanStandOnBlock(blk))
                        {
                            continue;
                        }

                        LandOnBlock(blk, r.Top);
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

                    if (!_player.HandleStarFlyHorizontalCollision())
                    {
                        _player.velocityX = 0f;
                    }
                }

                p = _player.Bounds;
            }
        }

        private void ResolveStandingSupport()
        {
            if (_player.velocityY < 0f)
            {
                return;
            }

            Rectangle playerBounds = _player.Bounds;
            IBlocks supportBlock = null;
            int bestTop = int.MaxValue;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                var blk = _worldBlocks[i];
                if (blk == null || !CanStandOnBlock(blk))
                {
                    continue;
                }

                Rectangle blockBounds = blk.Bounds;
                if (blockBounds == Rectangle.Empty)
                {
                    continue;
                }

                bool xOverlap = playerBounds.Right > blockBounds.Left && playerBounds.Left < blockBounds.Right;
                bool feetNearTop = System.Math.Abs(playerBounds.Bottom - blockBounds.Top) <= 1;
                if (!xOverlap || !feetNearTop)
                {
                    continue;
                }

                if (blockBounds.Top < bestTop)
                {
                    bestTop = blockBounds.Top;
                    supportBlock = blk;
                }
            }

            if (supportBlock != null)
            {
                LandOnBlock(supportBlock, bestTop);
            }
        }

        private void ResolveLedgeTopOut(Vector2 prevPos, bool prevCrouching, float attemptedDy, bool wasTouchingLeftWall, bool wasTouchingRightWall)
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
                Rectangle targetBounds = GetBoundsAt(targetPosition, crouching: false);

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

        private void ActivateTouchedCrushBlocks()
        {
            Rectangle playerBounds = _player.Bounds;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                if (_worldBlocks[i] is not CrushBlock crushBlock)
                {
                    continue;
                }

                Rectangle blockBounds = crushBlock.Bounds;
                if (blockBounds == Rectangle.Empty)
                {
                    continue;
                }

                if (IsTouchingBlock(playerBounds, blockBounds))
                {
                    crushBlock.Activate();
                }
            }
        }

        private static bool IsTouchingBlock(Rectangle playerBounds, Rectangle blockBounds)
        {
            if (playerBounds.Intersects(blockBounds))
            {
                return true;
            }

            bool xOverlap = playerBounds.Right > blockBounds.Left && playerBounds.Left < blockBounds.Right;
            bool yOverlap = playerBounds.Bottom > blockBounds.Top && playerBounds.Top < blockBounds.Bottom;

            bool touchingTop = xOverlap && playerBounds.Bottom == blockBounds.Top;
            bool touchingBottom = xOverlap && playerBounds.Top == blockBounds.Bottom;
            bool touchingLeft = yOverlap && playerBounds.Right == blockBounds.Left;
            bool touchingRight = yOverlap && playerBounds.Left == blockBounds.Right;

            return touchingTop || touchingBottom || touchingLeft || touchingRight;
        }

        private void ActivateTouchedSprings(Vector2 prevPos, bool prevCrouching, float attemptedDy)
        {
            if (attemptedDy <= 0f)
            {
                return;
            }

            Rectangle prevBounds = GetBoundsAt(prevPos, prevCrouching);
            Rectangle currentBounds = _player.Bounds;
            Rectangle swept = Rectangle.Union(prevBounds, currentBounds);

            Spring bestSpring = null;
            int bestTop = int.MaxValue;

            float prevFootY = prevPos.Y;
            float currFootY = _player.position.Y;

            for (int i = 0; i < _worldBlocks.Count; i++)
            {
                if (_worldBlocks[i] is not Spring spring)
                {
                    continue;
                }

                Rectangle trigger = spring.TriggerBounds;
                if (trigger == Rectangle.Empty)
                {
                    continue;
                }

                bool xOverlap = swept.Right > trigger.Left && swept.Left < trigger.Right;
                if (!xOverlap)
                {
                    continue;
                }

                if (prevFootY <= trigger.Top && currFootY >= trigger.Top && trigger.Top < bestTop)
                {
                    bestSpring = spring;
                    bestTop = trigger.Top;
                }
            }

            if (bestSpring == null)
            {
                return;
            }

            _player.position.Y = bestTop;
            bestSpring.Activate();
            _player.LaunchFromSpring();
        }

        private void LandOnBlock(IBlocks block, int top)
        {
            _player.position.Y = top;
            if (_player.HandleStarFlyVerticalCollision())
            {
                _player.onGround = true;
                _player.CurrentGroundBlock = block;
                return;
            }

            _player.velocityY = 0f;
            _player.onGround = true;
            if (!_playerWasOnGround)
            {
                _player.SpawnLandDust(_player.position);
            }

            if (!_player.isDashing)
            {
                _player.Maddy.OnDashRefill();
                _player.canDash = true;
            }

            _player.CurrentGroundBlock = block;
        }

        private void StoreMoveBlockLift(MoveBlock moveBlock, float dt)
        {
            if (moveBlock == null || dt <= 0f || moveBlock.MovementDelta == Vector2.Zero)
            {
                return;
            }

            _player.StoreLiftSpeed(moveBlock.MovementDelta / dt);
        }

        private static bool CanStandOnBlock(IBlocks block)
        {
            return block is not CrushBlock crushBlock || crushBlock.CanStandOn;
        }

        private static bool ShouldIgnoreTopCollision(IBlocks block, Rectangle blockBounds, Rectangle playerBounds, Rectangle previousBounds, float attemptedDy)
        {
            if (block is not CrushBlock crushBlock || crushBlock.CanStandOn)
            {
                return false;
            }

            if (attemptedDy < 0f)
            {
                return false;
            }

            bool xOverlap = playerBounds.Right > blockBounds.Left && playerBounds.Left < blockBounds.Right;
            bool cameFromAbove = previousBounds.Bottom <= blockBounds.Top && playerBounds.Bottom >= blockBounds.Top;
            return xOverlap && cameFromAbove;
        }
    }
}
