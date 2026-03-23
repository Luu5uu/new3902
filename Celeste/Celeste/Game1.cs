using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Celeste.Animation;
using Celeste.Character;
using Celeste.Items;
using Celeste.Blocks;
using Celeste.Blocks.Rooms;
using Celeste.DevTools;
using Celeste.Input;

using Celeste.DeathAnimation.Particles;
using System.Collections.Generic;
using Celeste.Utils;
using Celeste.Collision;
using System.Linq;
using System.Globalization;

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

        // new additions
        private MapBuilder _worldMap;
        private RoomOne _roomOne;
        private RoomTwo _roomTwo;
        private RoomThree _roomThree;
        private RoomFour _roomFour;
        private RoomFive _roomFive;
        private int _currentRoom = 0;

        private List<IBlocks> _blockList;

        /// <summary>When true, the currently displayed block (if animated) is updated each frame.</summary>
        private bool _blockAnimateEnabled;

        /// <summary>When true, the current block/obstacle is drawn. Toggle with V.</summary>
        private bool _blocksVisible = true;

        // Test collision and platform
        List<IBlocks> platform;
        CollisionSystem _collisionSystem;
        List<IHazard> hazards;
        HazardCollisioncs HazardCollisioncs;

        //worldBound

        Rectangle worldBound;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            // TODO: update  sprites/blocks scale later
            //_graphics.PreferredBackBufferWidth = 1280;
            //_graphics.PreferredBackBufferHeight = 720;

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

            worldBound = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
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
            if (factory.CreateBlock("upSpike", pos) is IBlocks b6) _blockList.Add(b6);
            if (factory.CreateBlock("top_a00", pos) is IBlocks b7) _blockList.Add(b7);
            if (factory.CreateBlock("top_a01", pos) is IBlocks b8) _blockList.Add(b8);
            if (factory.CreateBlock("top_a02", pos) is IBlocks b9) _blockList.Add(b9);
            if (factory.CreateBlock("top_a03", pos) is IBlocks b10) _blockList.Add(b10);
            _blockList.Add(new Spring(new Vector2(0, 0), _catalog));
            _blockList.Add(new MoveBlock(new Vector2(0, 0), 80f, 60f, 0f, _catalog));
            _blockList.Add(new CrushBlock(new Vector2(0, 0), _catalog));
            _totalBlocks = _blockList.Count;

            hazards = new List<IHazard>();
            hazards.Add(factory.CreateHazerd("upSpike", new Vector2(500, 400)));
            hazards.Add(factory.CreateHazerd("upSpike", new Vector2(520, 400)));

            HazardCollisioncs = new HazardCollisioncs(hazards.Cast<ICollideable>().ToList(), _player);

            platform = new List<IBlocks>();

            platform.Add(factory.CreateSnowBlock(new Vector2(300, 400)));
            platform.Add(factory.CreateSnowBlock(new Vector2(400, 400)));
            platform.Add(factory.CreateSnowBlock(new Vector2(600, 400)));
            platform.Add(factory.CreateSnowBlock(new Vector2(300, 60)));
            platform.Add(factory.CreateSnowBlock(new Vector2(200, 230)));
            platform.Add(factory.CreateSnowBlock(new Vector2(700, 300)));

            _collisionSystem = new CollisionSystem(platform, _player);


            _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
            _controllerLoader = new ControllerLoader(this);

            _worldMap = new MapBuilder(factory, 50, 30);
            _roomOne = new RoomOne(_worldMap, factory);
            _roomTwo = new RoomTwo(_worldMap, factory);
            _roomThree = new RoomThree(_worldMap, factory);
            _roomFour = new RoomFour(_worldMap, factory);
            _roomFive = new RoomFive(_worldMap, factory);

            BuildMap();
        }

        // for debugging and figuring out map values
        private void BuildMap()
        {

            if (_worldMap == null)
            {
                return;
            }
            _worldMap.ClearBlocks();

            switch (_currentRoom)
            {
                case 1:
                    _roomOne.PlaceRoomOneBlocks();
                    break;
                case 2:
                    _roomTwo.PlaceRoomTwoBlocks();
                    break;
                case 3:
                    _roomThree.PlaceRoomThreeBlocks();
                    break;
                case 4:
                    _roomFour.PlaceRoomFourBlocks();
                    break;
                case 5:
                    _roomFive.PlaceRoomFiveBlocks();
                    break;
                case 0:
                default:
                    break;
            }


        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kb = Keyboard.GetState();
            _debugOverlay.HandleInput(kb, _player);

            // FOR IN -CLASS CODE REVIEW
            int previousRoom = _currentRoom;
            if (kb.IsKeyDown(Keys.D0) && _prevKb.IsKeyUp(Keys.D0) || kb.IsKeyDown(Keys.NumPad0) && _prevKb.IsKeyUp(Keys.NumPad0)) { _currentRoom = 0; }
            else if (kb.IsKeyDown(Keys.D1) && _prevKb.IsKeyUp(Keys.D1) || kb.IsKeyDown(Keys.NumPad1) && _prevKb.IsKeyUp(Keys.NumPad1)) { _currentRoom = 1; }
            else if (kb.IsKeyDown(Keys.D2) && _prevKb.IsKeyUp(Keys.D2) || kb.IsKeyDown(Keys.NumPad2) && _prevKb.IsKeyUp(Keys.NumPad2)) { _currentRoom = 2; }
            else if (kb.IsKeyDown(Keys.D3) && _prevKb.IsKeyUp(Keys.D3) || kb.IsKeyDown(Keys.NumPad3) && _prevKb.IsKeyUp(Keys.NumPad3)) { _currentRoom = 3; }
            else if (kb.IsKeyDown(Keys.D4) && _prevKb.IsKeyUp(Keys.D4) || kb.IsKeyDown(Keys.NumPad4) && _prevKb.IsKeyUp(Keys.NumPad4)) { _currentRoom = 4; }
            else if (kb.IsKeyDown(Keys.D5) && _prevKb.IsKeyUp(Keys.D5) || kb.IsKeyDown(Keys.NumPad5) && _prevKb.IsKeyUp(Keys.NumPad5)) { _currentRoom = 5; }
            if (_currentRoom != previousRoom)
            {
                // rebuild room when changed
                BuildMap();
            }

            var cmd = PlayerCommand.FromKeyboard(kb, _prevKb);
            _prevKb = kb;
            _player.SetMovementCommand(cmd);

            _controllerLoader.Update();
            Vector2 prevPos = _player.position;


            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GlobalConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
            }
            else
            {
                _player.Update(gameTime);
                HazardCollisioncs.ResolveHazardCollision();
                _collisionSystem.ResolveBlockCollision(prevPos);
                _player.UpdateSprite(gameTime);
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

            _worldMap.Draw(_spriteBatch);

            _player.Draw(_spriteBatch);
            DrawUtils.DrawRectangleOutline(_spriteBatch, _pixelTexture, _player.Bounds, Color.Red);

            foreach (var b in platform)
            {
                b.Draw(_spriteBatch);
            }


            foreach (var h in hazards)
            {
                h.Draw(_spriteBatch);
            }

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
            /*if (_blocksVisible && _totalBlocks > 0)
            {
                var block = _blockList[_activeBlockIndex];
                block.Position = new Vector2(BlockConstants.BlockDisplayX, BlockConstants.BlockDisplayY);
                block.Draw(_spriteBatch);
                DrawUtils.DrawRectangleOutline( _spriteBatch, _pixelTexture, block.Bounds, Color.Lime);
            }*/

            if (_debugOverlay.ShowDebug)
                _debugOverlay.Draw(_spriteBatch, _player, _pixelTexture, Window);
            else

                // to get resolution
                Window.Title = $"Celeste - {Window.ClientBounds.Width}x{Window.ClientBounds.Height}";

            _spriteBatch.End();
            base.Draw(gameTime);
        }

        public void Reset() => _player.Reset();

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

        /// <summary>Toggles whether the currently displayed block (if animated) is updated each frame. Bound to B.</summary>
        public void ToggleBlockAnimation() => _blockAnimateEnabled = !_blockAnimateEnabled;

        /// <summary>Toggles whether the current block/obstacle is drawn at all. Bound to V.</summary>
        public void ToggleBlockDisplay() => _blocksVisible = !_blocksVisible;
    }
}