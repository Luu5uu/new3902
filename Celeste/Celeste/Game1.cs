using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Scenes;

namespace Celeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch SpriteBatch { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
            // In the future, this will push MainMenuScene instead
            SceneManager.PushScene(new MainMenuScene(this)); 
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            // The SceneManager takes care of everything now
            SceneManager.Update(gameTime); 
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clear the screen once per frame
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Tell the active scenes to draw themselves
            SceneManager.Draw(SpriteBatch);
            
            base.Draw(gameTime);
        }
        public void Reset()
        {   
            var gameplay = SceneManager.GetScene<GameplayScene>();
            gameplay?.Reset();
        }

        public void CycleGameScene(int direction)
        {
            var gameplay = SceneManager.GetScene<GameplayScene>();
            gameplay?.CycleGameScene(direction);
        }
    }

    
}