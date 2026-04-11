using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Scenes
{
    public abstract class Scene
    {
        protected Game1 Game;
        
        public bool BlocksUpdate { get; protected set; } = false;
        public bool BlocksDraw { get; protected set; } = false;

        public Scene(Game1 game)
        {
            Game = game;
        }

        public abstract void LoadContent();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spriteBatch);
    }
}