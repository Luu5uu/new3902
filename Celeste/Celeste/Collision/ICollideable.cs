using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Celeste.Collision
{
    public interface ICollideable
    {
        ICollider Collider { get; }
    }
}
