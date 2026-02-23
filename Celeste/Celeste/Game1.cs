using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Celeste.Animation;
using Celeste.Character;
using Celeste.Items;
using Celeste.Blocks;
using Celeste.DevTools;
using Celeste.Input;

using Celeste.DeathAnimation.Particles;
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
        private DebugOverlay _debugOverlay;

        // NEW: particle dot texture
        private Texture2D _deathDotTex;

        private ControllerLoader _controllerLoader;
        private int _activeItemIndex = 0;
        private int _activeBlockIndex = 0;  // T = previous, Y = next; only this block is drawn
        private int _totalItems = 3;
        private int _totalBlocks;            // Set from _blockList.Count after load

        private List<IBlocks> _blockList;

        /// <summary>When true, the currently displayed block (if animated) is updated each frame.</summary>
        private bool _blockAnimateEnabled;

        /// <summary>When true, the current block/obstacle is drawn. Toggle with V.</summary>
        private bool _blocksVisible = true;

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
            _normalStawAnim.Position = new Vector2(ItemConstants.ItemNormalStawX, ItemConstants.ItemRowY);
            _normalStawAnim.Scale = GlobalConstants.DefaultScale;
            _flyStawAnim.Position = new Vector2(ItemConstants.ItemFlyStawX, ItemConstants.ItemRowY);
            _flyStawAnim.Scale = GlobalConstants.DefaultScale;
            _crystalAnim.Position = new Vector2(ItemConstants.ItemCrystalX, ItemConstants.ItemRowY);
            _crystalAnim.Scale = GlobalConstants.DefaultScale;

            _pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            // ---- Blocks (from allBlocks, adapted to new structure) ----
            var factory = BlockFactory.GetInstance;
            factory.LoadTextures(Content);
            _blockList = new List<IBlocks>();
            var pos = new Vector2(0, 0);
            if (factory.CreateSnowBlock(pos) is IBlocks b0) _blockList.Add(b0);
            if (factory.CreateCementBlock(pos) is IBlocks b1) _blockList.Add(b1);
            if (factory.CreateDirtBlock(pos) is IBlocks b2) _blockList.Add(b2);
            if (factory.CreateGirderBlock(pos) is IBlocks b3) _blockList.Add(b3);
            if (factory.CreateBlock("4", pos) is IBlocks b4) _blockList.Add(b4);
            if (factory.CreateBlock("7", pos) is IBlocks b5) _blockList.Add(b5);
            if (factory.CreateBlock("spikeUp", pos) is IBlocks b6) _blockList.Add(b6);
            if (factory.CreateBlock("top_a00", pos) is IBlocks b7) _blockList.Add(b7);
            if (factory.CreateBlock("top_a01", pos) is IBlocks b8) _blockList.Add(b8);
            if (factory.CreateBlock("top_a02", pos) is IBlocks b9) _blockList.Add(b9);
            if (factory.CreateBlock("top_a03", pos) is IBlocks b10) _blockList.Add(b10);
            _blockList.Add(new Spring(new Vector2(0, 0), _catalog));
            _blockList.Add(new MoveBlock(new Vector2(0, 0), 80f, 60f, 0f, _catalog));
            _blockList.Add(new CrushBlock(new Vector2(0, 0), _catalog));
            _totalBlocks = _blockList.Count;

            _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
            _controllerLoader = new ControllerLoader(this);
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

            _controllerLoader.Update();

            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GlobalConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
            }
            else
            {
                _player.Update(gameTime);
            }

            _normalStawAnim.Update(gameTime);
            _flyStawAnim.Update(gameTime);
            _crystalAnim.Update(gameTime);

            // When animation toggle is on, update the current block if it's an animated type (spring, move, crush).
            if (_blockAnimateEnabled && _totalBlocks > 0)
            {
                var block = _blockList[_activeBlockIndex];
                if (block is Spring spring)
                    spring.Update(gameTime);
                else if (block is MoveBlock moveBlock)
                    moveBlock.Update(gameTime);
                else if (block is CrushBlock crushBlock)
                    crushBlock.Update(gameTime);
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

            // Draw current block/obstacle only when block display is on (T = previous, Y = next). Stationary, no interaction.
            if (_blocksVisible && _totalBlocks > 0)
            {
                var block = _blockList[_activeBlockIndex];
                block.Position = new Vector2(BlockConstants.BlockDisplayX, BlockConstants.BlockDisplayY);
                block.Draw(_spriteBatch);
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

            if (_activeItemIndex < 0) _activeItemIndex = _totalItems -1;
            if (_activeItemIndex >= _totalItems) _activeItemIndex = 0;
        }

        public void CycleActiveBlock(int direction)
        {
            _activeBlockIndex += direction;

            if (_activeBlockIndex < 0) _activeBlockIndex = _totalBlocks -1;
            if (_activeBlockIndex >= _totalBlocks) _activeBlockIndex = 0;
        }

        /// <summary>Toggles whether the currently displayed block (if animated) is updated each frame. Bound to B.</summary>
        public void ToggleBlockAnimation() => _blockAnimateEnabled = !_blockAnimateEnabled;

        /// <summary>Toggles whether the current block/obstacle is drawn at all. Bound to V.</summary>
        public void ToggleBlockDisplay() => _blocksVisible = !_blocksVisible;
    }
}