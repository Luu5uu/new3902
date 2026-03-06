using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

public interface ICollider
{
    Rectangle Bounds { get; }
}
