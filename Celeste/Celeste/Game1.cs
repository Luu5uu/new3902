using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Celeste.Animation;
using Celeste.Character;
using Celeste.CollectableItems;
// using Celeste.Debug;
using Celeste.Input;
using Celeste.Blocks;
using System.Collections.Generic;

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
        // private DebugOverlay _debugOverlay;

        private List<IBlocks> _blocks;
        private int _currentBlock = 0;
        private KeyboardState _keyboardBlockInput;

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

            BlockFactory.Instance.LoadAllTextures(Content);
            _blocks = new List<IBlocks>();
            //non-moving mass blocks
            _blocks.Add(BlockFactory.Instance.CreateSnowBlock(new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateCementBlock(new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateDirtBlock(new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateGirderBlock(new Vector2(200, 200)));

            //non-moving textures
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("4", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("7", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("top_a00", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("top_a01", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("top_a02", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("top_a03", new Vector2(200, 200)));
            _blocks.Add(BlockFactory.Instance.CreateAnyBlock("spikeUp", new Vector2(200, 200)));
            //_blocks.Add(BlockFactory.Instance.CreateAnyBlock("spikeDown", new Vector2(200, 200)));
            //_blocks.Add(BlockFactory.Instance.CreateAnyBlock("spikeLeft", new Vector2(200, 200)));
            //_blocks.Add(BlockFactory.Instance.CreateAnyBlock("spikeRight", new Vector2(200, 200)));

            //moving blocks
            _blocks.Add(new Spring(new Vector2(200, 200), _catalog));
            _blocks.Add(new CrushBlock(new Vector2(200, 200), _catalog));
            _blocks.Add(new MoveBlock(new Vector2(200, 400), 100f, 50f, -20f, _catalog));

            var startPos = new Vector2(
                Window.ClientBounds.Width / 2f,
                Window.ClientBounds.Height / 2f);
            _player = new Madeline(Content, _catalog, startPos);

            _normalStawAnim = ItemAnimationFactory.CreateNormalStaw(_catalog);
            _flyStawAnim = ItemAnimationFactory.CreateFlyStaw(_catalog);
            _crystalAnim = ItemAnimationFactory.CreateCrystal(_catalog);
            _normalStawAnim.Position = new Vector2(GameConstants.ItemNormalStawX, GameConstants.ItemRowY);
            _normalStawAnim.Scale = GameConstants.DefaultScale;
            _flyStawAnim.Position = new Vector2(GameConstants.ItemFlyStawX, GameConstants.ItemRowY);
            _flyStawAnim.Scale = GameConstants.DefaultScale;
            _crystalAnim.Position = new Vector2(GameConstants.ItemCrystalX, GameConstants.ItemRowY);
            _crystalAnim.Scale = GameConstants.DefaultScale;

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
            _keyboardBlockInput = Keyboard.GetState();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kb = Keyboard.GetState();
            // _debugOverlay.HandleInput(kb, _player);

            var cmd = PlayerCommand.FromKeyboard(kb, _prevKb);
            _prevKb = kb;
            _player.SetMovementCommand(cmd);

            // for the block switching
            if (kb.IsKeyDown(Keys.T) && _keyboardBlockInput.IsKeyUp(Keys.T))
            {
                // move backwards (T)
                _currentBlock--;
                if (_currentBlock < 0)
                {
                    _currentBlock = _blocks.Count - 1;
                }
            }
            if (kb.IsKeyDown(Keys.Y) && _keyboardBlockInput.IsKeyUp(Keys.Y))
            {
                // move forwards Y
                _currentBlock++;
                if (_currentBlock >= _blocks.Count)
                {
                    _currentBlock = 0;
                }
            }
            // update state
            _keyboardBlockInput = kb;


            /* if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
             {
                 _player.Maddy.SetPosition(_player.position, scale: GameConstants.DefaultScale, faceLeft: _player.FaceLeft);
                 _player.Maddy.Update(gameTime);
             }
             else
             {
                 _player.Update(gameTime);
             } */

            //for debugging
            _player.Update(gameTime);

            _normalStawAnim.Update(gameTime);
            _flyStawAnim.Update(gameTime);
            _crystalAnim.Update(gameTime);

            //  possibly change to switch cases (?)
            foreach (var block in _blocks)
            {
                if (block is Spring spring)
                {
                    spring.Update(gameTime);
                }
                if (block is CrushBlock crushBlock)
                {
                    crushBlock.Update(gameTime);
                }
                if (block is MoveBlock moveBlock)
                {
                    moveBlock.Update(gameTime);
                }
            }

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

            if (_blocks != null && _blocks.Count > 0 && _currentBlock < _blocks.Count)
            {
                _blocks[_currentBlock].Draw(_spriteBatch);
            }

            /* if (_debugOverlay.ShowDebug)
                 _debugOverlay.Draw(_spriteBatch, _player, _pixelTexture, Window);
             else
                 Window.Title = "Celeste"; */

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
