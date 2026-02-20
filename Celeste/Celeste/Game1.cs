using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Animation;
using Celeste.Character;
using Celeste.CollectableItems;
using Celeste.Debug;
using Celeste.Input;

namespace Celeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AnimationCatalog _catalog;
        private Madeline _player;

        private ItemAnimation _normalStawAnim;
        private ItemAnimation _flyStawAnim;
        private ItemAnimation _crystalAnim;

        private KeyboardState _prevKb;
        private Texture2D _pixelTexture;
        private DebugOverlay _debugOverlay;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _catalog = AnimationLoader.LoadAll(Content);

            var startPos = new Vector2(
                Window.ClientBounds.Width  / 2f,
                Window.ClientBounds.Height / 2f);
            _player = new Madeline(Content, _catalog, startPos);

            _normalStawAnim = ItemAnimationFactory.CreateNormalStaw(_catalog);
            _flyStawAnim    = ItemAnimationFactory.CreateFlyStaw(_catalog);
            _crystalAnim    = ItemAnimationFactory.CreateCrystal(_catalog);
            _normalStawAnim.Position = new Vector2(GameConstants.ItemNormalStawX, GameConstants.ItemRowY);
            _normalStawAnim.Scale = GameConstants.DefaultScale;
            _flyStawAnim.Position = new Vector2(GameConstants.ItemFlyStawX, GameConstants.ItemRowY);
            _flyStawAnim.Scale = GameConstants.DefaultScale;
            _crystalAnim.Position = new Vector2(GameConstants.ItemCrystalX, GameConstants.ItemRowY);
            _crystalAnim.Scale = GameConstants.DefaultScale;

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kb = Keyboard.GetState();
            _debugOverlay.HandleInput(kb, _player);

            var cmd = PlayerCommand.FromKeyboard(kb, _prevKb);
            _prevKb = kb;
            _player.SetMovementCommand(cmd);

            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GameConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
            }
            else
            {
                _player.Update(gameTime);
            }

            _normalStawAnim.Update(gameTime);
            _flyStawAnim.Update(gameTime);
            _crystalAnim.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // CullNone needed: BodySprite flips via negative X scale.
            _spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            _player.Draw(_spriteBatch);

            _normalStawAnim.Draw(_spriteBatch);
            _flyStawAnim.Draw(_spriteBatch);
            _crystalAnim.Draw(_spriteBatch);

            if (_debugOverlay.ShowDebug)
                _debugOverlay.Draw(_spriteBatch, _player, _pixelTexture, Window);
            else
                Window.Title = "Celeste";

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
