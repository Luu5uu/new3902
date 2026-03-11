using Celeste.Collision;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public class Hazard:IHazard,ICollideable
    {
        public Vector2 Position { get; set; }
        public string Type { get; }

        public Texture2D Texture { get; set; }
        public float Scale { get; set; } = 2.0f;

        public ICollider Collider { get; private set; }
        public Hazard(string type, Vector2 position, Texture2D texture = null, float scale = 2.5f)
        {
            Type = type;
            Position = position;
            Texture = texture;
            Collider = new AabbCollider(() => Bounds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Texture != null)
            {
                spriteBatch.Draw(
                    Texture, Position, null, Color.White, 0f, Vector2.Zero, Scale, SpriteEffects.None, 0f);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                if (Texture == null)
                    return Rectangle.Empty;

                int w = (int)(Texture.Width * Scale);
                int h = (int)(Texture.Height * Scale);

                return new Rectangle(
                    (int)Position.X,
                    (int)Position.Y,
                    w,
                    h
                );
            }
        }
    }
}

