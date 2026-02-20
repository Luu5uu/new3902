using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation.Particles
{
    public static class ProceduralParticleTexture
    {
        public static Texture2D CreateHardDot(GraphicsDevice gd, int size = 5)
        {
            var tex = new Texture2D(gd, size, size, false, SurfaceFormat.Color);
            var data = new Color[size * size];

            for (int i = 0; i < data.Length; i++) data[i] = Color.Transparent;

            int c = size / 2;

            void Set(int x, int y)
            {
                if (x < 0 || x >= size || y < 0 || y >= size) return;
                data[y * size + x] = Color.White;
            }

            // pixel-ish dot
            Set(c, c);
            Set(c - 1, c); Set(c + 1, c);
            Set(c, c - 1); Set(c, c + 1);

            // slightly fuller
            Set(c - 1, c - 1); Set(c + 1, c - 1);
            Set(c - 1, c + 1); Set(c + 1, c + 1);

            tex.SetData(data);
            return tex;
        }
    }
}