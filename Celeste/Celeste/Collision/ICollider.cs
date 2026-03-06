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

public class AabbCollider : ICollider
{
    private readonly Func<Rectangle> _getBounds;
    public AabbCollider(Func<Rectangle> getBounds) => _getBounds = getBounds;
    public Rectangle Bounds => _getBounds();
}