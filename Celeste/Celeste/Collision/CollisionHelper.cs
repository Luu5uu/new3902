using Microsoft.Xna.Framework;
using System;

namespace Celeste.Collision
{
    public static class CollisionHelper
    {
        public static bool Overlaps(ICollideable a, ICollideable b)
        {
            if (a == null || b == null) return false;
            if (a.Collider == null || b.Collider == null) return false;

            return Overlaps(a.Collider, b.Collider);
        }

        public static bool Overlaps(ICollider a, ICollider b)
        {
            if (a == null || b == null) return false;

            Rectangle ra = a.Bounds;
            Rectangle rb = b.Bounds;

            if (ra == Rectangle.Empty || rb == Rectangle.Empty)
                return false;

            return ra.Intersects(rb);
        }
        public static bool Overlaps(Rectangle a, Rectangle b)
        {
            return a.Intersects(b);
        }

        public static Vector2 GetPushOut(ICollideable a, ICollideable b)
        {
            if (a == null || b == null) return Vector2.Zero;
            if (a.Collider == null || b.Collider == null) return Vector2.Zero;

            return GetPushOut(a.Collider, b.Collider);
        }

        public static Vector2 GetPushOut(ICollider a, ICollider b)
        {
            if (a == null || b == null) return Vector2.Zero;

            Rectangle ra = a.Bounds;
            Rectangle rb = b.Bounds;

            if (ra == Rectangle.Empty || rb == Rectangle.Empty)
                return Vector2.Zero;

            if (!ra.Intersects(rb))
                return Vector2.Zero;

            int overlapLeft = ra.Right - rb.Left;
            int overlapRight = rb.Right - ra.Left;
            int overlapTop = ra.Bottom - rb.Top;
            int overlapBottom = rb.Bottom - ra.Top;

            int pushX = Math.Abs(overlapLeft) < Math.Abs(overlapRight)
                ? -overlapLeft
                : overlapRight;

            int pushY = Math.Abs(overlapTop) < Math.Abs(overlapBottom)
                ? -overlapTop
                : overlapBottom;

            if (Math.Abs(pushX) < Math.Abs(pushY))
                return new Vector2(pushX, 0);

            return new Vector2(0, pushY);
        }

        public static float GetHorizontalPushOut(Rectangle a, Rectangle b)
        {
            if (!a.Intersects(b))
                return 0f;

            int pushLeft = b.Left - a.Right;   
            int pushRight = b.Right - a.Left;  

            return Math.Abs(pushLeft) < Math.Abs(pushRight) ? pushLeft : pushRight;
        }

        public static float GetVerticalPushOut(Rectangle a, Rectangle b)
        {
            if (!a.Intersects(b))
                return 0f;

            int pushUp = b.Top - a.Bottom;     
            int pushDown = b.Bottom - a.Top;    

            return Math.Abs(pushUp) < Math.Abs(pushDown) ? pushUp : pushDown;
        }
    }
}