using Celeste.Animation;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Character;
// Test Item Animation
using Celeste.CollectableItems;

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

        // Test Item Animation
        private ItemAnimation _normalStawAnim = null!;
        private ItemAnimation _flyStawAnim = null!;
        private ItemAnimation _crystalAnim = null!;

        private Vector2 _normalPos;
        private Vector2 _flyPos;
        private Vector2 _crystalPos;

        public Vector2 startPosition;
        

        

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

            startPosition = new Vector2(Window.ClientBounds.Width / 2f, Window.ClientBounds.Height / 2f);

            // Load dictionary of Textures
            _anims = AnimationLoader.LoadAll(Content);
            m = new Madeline(_anims,startPosition);

            // Create item animations
            _normalStawAnim = ItemAnimationFactory.CreateNormalStaw(_anims);
            _flyStawAnim    = ItemAnimationFactory.CreateFlyStaw(_anims);
            _crystalAnim    = ItemAnimationFactory.CreateCrystal(_anims);

            // Place them on screen (simple layout)
            _normalPos  = new Vector2(120f, 120f);
            _flyPos     = new Vector2(220f, 120f);
            _crystalPos = new Vector2(340f, 120f);  
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            m.update(gameTime);
            _normalStawAnim.Update(gameTime);
            _flyStawAnim.Update(gameTime);
            _crystalAnim.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            // ===== Start drawing =====

            m.draw(_spriteBatch);
            
            // Draw three items
            _normalStawAnim.Draw(_spriteBatch, _normalPos, scale: 2f);
            _flyStawAnim.Draw(_spriteBatch, _flyPos, scale: 2f);
            _crystalAnim.Draw(_spriteBatch, _crystalPos, scale: 2f);

            // ===== End drawing =====
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
