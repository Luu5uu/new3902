using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Celeste.Animation;
using Celeste.Character;
using Celeste.CollectableItems;
using Celeste.DevTools;
using Celeste.Input;

using Celeste.DeathAnimation.Particles;
using Celeste.Blocks;
using System.Collections.Generic;
using System.Linq;

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

        private List<IBlocks> _blocks;
        private int _currentBlock = 0;
        private KeyboardState _keyboardBlockInput;

        // NEW: particle dot texture
        private Texture2D _deathDotTex;

        private ControllerLoader _controllerLoader;
        private int _activeItemIndex = 0;
        private int _activeBlockIndex = 0;
        private int _totalItems = 3;
        private int _totalBlocks; // Set based on total block types

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() => base.Initialize();

        protected override void LoadContent()
        {
            _blocks = new List<IBlocks>();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _catalog = AnimationLoader.LoadAll(Content);

            BlockFactory.Instance.LoadAllTextures(Content);
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

            //debugging  for sizes of blocks
            _blocks.Add(new AllBlocks(_blocks.Take(_blocks.Count).ToList(), _catalog));

            _totalBlocks = _blocks.Count;

            var startPos = new Vector2(
                Window.ClientBounds.Width / 2f,
                Window.ClientBounds.Height / 2f);

            _player = new Madeline(Content, _catalog, startPos);

            // ===== Inject DeathAnimation resources (no constructor change) =====
            _deathDotTex = ProceduralParticleTexture.CreateHardDot(GraphicsDevice, size: 5);

            // Use the SAME key as AnimationLoader registered.
            const string DeathClipKey = AnimationKeys.PlayerDeath; // "Player/Death"

            if (!_catalog.Clips.TryGetValue(DeathClipKey, out var deathClip))
                throw new ContentLoadException(
                    $"Death clip key '{DeathClipKey}' not found in AnimationCatalog. " +
                    $"Available keys: {string.Join(", ", _catalog.Clips.Keys)}");

            _player.ConfigureDeathAnimation(deathClip, _deathDotTex);

            // ===== Items =====
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

            _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
            _keyboardBlockInput = Keyboard.GetState();
            _controllerLoader = new ControllerLoader(this);
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



            _controllerLoader.Update();

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

            // BLOCKS
            if (_blocks != null && _blocks.Count > 0 && _currentBlock >= 0 && _currentBlock < _blocks.Count)
            {
                var curBlock = _blocks[_currentBlock];
                if (curBlock is AllBlocks gallery)
                {
                    gallery.Draw(_spriteBatch, GraphicsDevice);

                }
                else
                {
                    curBlock.Draw(_spriteBatch);
                }
            }

            _player.Draw(_spriteBatch);

            switch (_activeItemIndex)
            {
                case 0:
                    _normalStawAnim.Draw(_spriteBatch);
                    break;
                case 1:
                    _flyStawAnim.Draw(_spriteBatch);
                    break;
                case 2:
                    _crystalAnim.Draw(_spriteBatch);
                    break;
            }

            if (_debugOverlay.ShowDebug)
                _debugOverlay.Draw(_spriteBatch, _player, _pixelTexture, Window);
            else
                Window.Title = "Celeste";

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void CycleActiveItem(int direction)
        {
            _activeItemIndex += direction;

            if (_activeItemIndex < 0) _activeItemIndex = _totalItems - 1;
            if (_activeItemIndex >= _totalItems) _activeItemIndex = 0;
        }

        public void CycleActiveBlock(int direction)
        {
            _activeBlockIndex += direction;

            if (_activeBlockIndex < 0) _activeBlockIndex = _totalBlocks - 1;
            if (_activeBlockIndex >= _totalBlocks) _activeBlockIndex = 0;
        }
    }
}