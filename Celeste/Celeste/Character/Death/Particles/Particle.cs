using Microsoft.Xna.Framework;

namespace Celeste.DeathAnimation.Particles
{
    /// <summary>
    /// Lightweight particle data container.
    /// </summary>
    public struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;

        public float Age;
        public float Lifetime;

        public float StartSize;
        public float EndSize;

        public float StartAlpha;
        public float EndAlpha;

        public float Rotation;
        public float AngularVelocity;

        public Color Tint;

        public bool Alive => Age < Lifetime;

        public float T => Lifetime <= 0f
            ? 1f
            : MathHelper.Clamp(Age / Lifetime, 0f, 1f);
    }
}
