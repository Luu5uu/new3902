using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.DeathAnimation.Particles
{
    public static class ProceduralParticleTexture
    {
        public static Texture2D CreateSoftDot(GraphicsDevice gd, int size = 16)
        {
            var tex = new Texture2D(gd, size, size, false, SurfaceFormat.Color);
            var data = new Color[size * size];
            float center = (size - 1) / 2f;
            float radius  = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dist = (float)Math.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                    float t = Math.Max(0f, 1f - dist / radius);
                    byte a = (byte)(t * t * 255f);
                    data[y * size + x] = new Color((byte)255, (byte)255, (byte)255, a);
                }
            }

            tex.SetData(data);
            return tex;
        }

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
