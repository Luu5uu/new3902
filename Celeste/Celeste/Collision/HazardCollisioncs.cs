using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Celeste.Blocks;
using Celeste.Character;
using Celeste.DeathAnimation;
using Celeste.MadelineStates;
using Microsoft.Xna.Framework;


namespace Celeste.Collision
{
    public class HazardCollisioncs
    {
        private readonly List<ICollideable> _worldHazerd;
        private readonly Madeline _player;

        public HazardCollisioncs(List<ICollideable> worldHazard, Madeline player)
        {
            _worldHazerd = worldHazard;
            _player = player;
        }

        public void ResolveHazardCollision()
        {
            
            Rectangle playerRect = _player.Bounds;

            for (int i = 0; i < _worldHazerd.Count; i++)
            {
                var h = _worldHazerd[i];
                if (h == null || h.Collider == null) continue;

                Rectangle hazardRect = h.Collider.Bounds;
                if (hazardRect == Rectangle.Empty) continue;

                if (playerRect.Intersects(hazardRect))
                {
                    if (CollisionHelper.Overlaps(playerRect, hazardRect))
                    {
                        // Avoid re-entering DeathState if already playing
                        if (_player._deathEffect == null)
                        {
                            _player.changeState(_player.deathState);
                        }
                    }
                    return; 
                }
            }
        }
    }
}
