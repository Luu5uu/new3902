using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.GamePlay
{
    public interface IUpdateable
    {
        void Update(GameTime gameTime);
    }

    public interface IDrawable
    {
        void Draw(SpriteBatch spriteBatch);
    }
}
