using Celeste.Animation;

using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Character;


namespace Celeste
{
    public class Game1 : Game
    {
        //initial repo test
        //private Texture2D madeLine;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private AnimationCatalog _anims = null!;
        Madeline m;
        

        

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load dictionary of Textures
            _anims = AnimationLoader.LoadAll(Content);
            m = new Madeline(_anims,Vector2.Zero);

           

            
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            m.update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // ===== Start drawing =====

            m.draw(_spriteBatch);

            // ===== End drawing =====
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
