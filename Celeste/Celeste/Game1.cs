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
using System.IO;

namespace Celeste
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private AnimationCatalog _catalog;
        private Madeline _player;

        private KeyboardState _prevKb;
        private Texture2D _pixelTexture;
        private DebugOverlay _debugOverlay;

        // NEW: particle dot texture
        private Texture2D _deathDotTex;

        private ControllerLoader _controllerLoader;
        private int _activeBlockIndex = 0;  // T = previous, Y = next; only this block is drawn
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
        private readonly List<CollectibleItem> _collectibles = new();

        /// <summary>When true, the currently displayed block (if animated) is updated each frame.</summary>
        private bool _blockAnimateEnabled;


        // Collasion systems
        CollisionSystem _collisionSystem;
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
            _deathDotTex = LoadDeathDotTexture();

            if (!_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathSide, out var deathSideClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathUp, out var deathUpClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathDown, out var deathDownClip))
                throw new ContentLoadException(
                    $"Directional death clips not found in AnimationCatalog. " +
                    $"Available keys: {string.Join(", ", _catalog.Clips.Keys)}");

            _player.ConfigureDeathAnimation(deathSideClip, deathUpClip, deathDownClip, _deathDotTex);

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

         


            _debugOverlay = new DebugOverlay();
            _prevKb = Keyboard.GetState();
            _controllerLoader = new ControllerLoader(this, _player);

            _worldMap = new MapBuilder(factory, 50, 30);
            _roomOne = new RoomOne(_worldMap, factory);
            _roomTwo = new RoomTwo(_worldMap, factory);
            _roomThree = new RoomThree(_worldMap, factory);
            _roomFour = new RoomFour(_worldMap, factory);
            _roomFive = new RoomFive(_worldMap, factory);

            RebuildCurrentRoom(resetPlayer: false);
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
                    _roomThree.PlaceRoomThreeBlocks();
                    break;
            }

            Vector2 checkpointSpawn = GetRespawnPointForRoom(_currentRoom);
            _player.position = checkpointSpawn;
            _player.RespawnPoint = checkpointSpawn;
        }

        private Texture2D LoadDeathDotTexture()
        {
            string dotPath = Path.Combine(Content.RootDirectory, "DeathParticleDot.png");
            try
            {
                using Stream stream = TitleContainer.OpenStream(dotPath);
                Texture2D loaded = Texture2D.FromStream(GraphicsDevice, stream);
                Texture2D trimmed = TrimTransparentBounds(loaded);
                if (!ReferenceEquals(trimmed, loaded))
                {
                    loaded.Dispose();
                }
                return trimmed;
            }
            catch
            {
                return ProceduralParticleTexture.CreateHardDot(GraphicsDevice, size: 3);
            }
        }

        private Texture2D TrimTransparentBounds(Texture2D texture)
        {
            Color[] pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);

            int minX = texture.Width;
            int minY = texture.Height;
            int maxX = -1;
            int maxY = -1;

            for (int y = 0; y < texture.Height; y++)
            {
                for (int x = 0; x < texture.Width; x++)
                {
                    Color pixel = pixels[(y * texture.Width) + x];
                    if (pixel.A == 0)
                    {
                        continue;
                    }

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }
            }

            if (maxX < minX || maxY < minY)
            {
                return texture;
            }

            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            if (width == texture.Width && height == texture.Height)
            {
                return texture;
            }

            Color[] trimmedPixels = new Color[width * height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    trimmedPixels[(y * width) + x] = pixels[((minY + y) * texture.Width) + (minX + x)];
                }
            }

            Texture2D trimmed = new Texture2D(GraphicsDevice, width, height);
            trimmed.SetData(trimmedPixels);
            return trimmed;
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit(); // dont think we need this since we have a QuitCommand already handled in keyboard controller.

            var kb = Keyboard.GetState();
            _debugOverlay.HandleInput(kb, _player);
            HandleRoomHotkeys(kb);

            _controllerLoader.Update();
            Vector2 prevPos = _player.position;
            bool prevCrouching = _player.isCrouching;


            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GlobalConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
            }
            else
            {
                _player.Update(gameTime);

                if (_player.ConsumeLevelResetRequest())
                {
                    RebuildCurrentRoom(resetPlayer: true);
                }
                else
                {
                    HazardCollisioncs.ResolveHazardCollision();
                    _collisionSystem.ResolveBlockCollision(prevPos, prevCrouching);
                    UpdateCollectibles(gameTime);
                    _player.UpdateSprite(gameTime);
                }
            }

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

            if(_player.Bounds.Bottom> worldBound.Bottom)
            {
                _player.Reset();
                
            }

            _prevKb = kb;

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
            DrawCollectibles(_spriteBatch);
            _player.Draw(_spriteBatch);
            if (_debugOverlay.ShowDebug)
                DrawUtils.DrawRectangleOutline(_spriteBatch, _pixelTexture, _player.Bounds, Color.Red);

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

        public void CycleGameScene(int direction)
        {
            const int firstRoom = 1;
            const int lastRoom = 5;

            int nextRoom = _currentRoom;
            if (nextRoom < firstRoom || nextRoom > lastRoom)
            {
                nextRoom = 3;
            }

            nextRoom += direction;
            if (nextRoom < firstRoom)
            {
                nextRoom = lastRoom;
            }
            else if (nextRoom > lastRoom)
            {
                nextRoom = firstRoom;
            }

            if (nextRoom != _currentRoom)
            {
                _currentRoom = nextRoom;
                RebuildCurrentRoom(resetPlayer: false);
            }
        }
        public void Reset() => RebuildCurrentRoom(resetPlayer: true);

        public void CycleActiveBlock(int direction)
        {
            _activeBlockIndex += direction;

            if (_activeBlockIndex < 0) _activeBlockIndex = _totalBlocks - 1;
            if (_activeBlockIndex >= _totalBlocks) _activeBlockIndex = 0;
        }

        /// <summary>Toggles whether the currently displayed block (if animated) is updated each frame. Bound to B.</summary>
        public void ToggleBlockAnimation() => _blockAnimateEnabled = !_blockAnimateEnabled;

        /// <summary>Toggles whether the current block/obstacle is drawn at all. Bound to V.</summary>
        //public void ToggleBlockDisplay() => _blocksVisible = !_blocksVisible;

        private void HandleRoomHotkeys(KeyboardState kb)
        {
            int requestedRoom = _currentRoom;

            if ((kb.IsKeyDown(Keys.D0) && _prevKb.IsKeyUp(Keys.D0)) || (kb.IsKeyDown(Keys.NumPad0) && _prevKb.IsKeyUp(Keys.NumPad0))) { requestedRoom = 0; }
            else if ((kb.IsKeyDown(Keys.D1) && _prevKb.IsKeyUp(Keys.D1)) || (kb.IsKeyDown(Keys.NumPad1) && _prevKb.IsKeyUp(Keys.NumPad1))) { requestedRoom = 1; }
            else if ((kb.IsKeyDown(Keys.D2) && _prevKb.IsKeyUp(Keys.D2)) || (kb.IsKeyDown(Keys.NumPad2) && _prevKb.IsKeyUp(Keys.NumPad2))) { requestedRoom = 2; }
            else if ((kb.IsKeyDown(Keys.D3) && _prevKb.IsKeyUp(Keys.D3)) || (kb.IsKeyDown(Keys.NumPad3) && _prevKb.IsKeyUp(Keys.NumPad3))) { requestedRoom = 3; }
            else if ((kb.IsKeyDown(Keys.D4) && _prevKb.IsKeyUp(Keys.D4)) || (kb.IsKeyDown(Keys.NumPad4) && _prevKb.IsKeyUp(Keys.NumPad4))) { requestedRoom = 4; }
            else if ((kb.IsKeyDown(Keys.D5) && _prevKb.IsKeyUp(Keys.D5)) || (kb.IsKeyDown(Keys.NumPad5) && _prevKb.IsKeyUp(Keys.NumPad5))) { requestedRoom = 5; }

            if (requestedRoom != _currentRoom)
            {
                _currentRoom = requestedRoom;
                RebuildCurrentRoom(resetPlayer: false);
            }
        }

        private void RebuildCurrentRoom(bool resetPlayer)
        {
            BuildMap();
            RebuildCollisionSystems();
            RebuildCollectibles();

            if (resetPlayer)
            {
                _player.Reset();
            }
        }

        private void RebuildCollisionSystems()
        {
            _collisionSystem = new CollisionSystem(_worldMap._blocks, _player);
            HazardCollisioncs = new HazardCollisioncs(_worldMap._hazards.Cast<ICollideable>().ToList(), _player);
            _player.SetWorldBlocks(_worldMap._blocks);
        }

        private void RebuildCollectibles()
        {
            _collectibles.Clear();

            Vector2 spawn = _player.RespawnPoint;
            if (_currentRoom == 0)
            {
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateNormalStaw(_catalog), spawn + new Vector2(-80f, -40f)));
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateFlyStaw(_catalog), spawn + new Vector2(0f, -60f)));
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateCrystal(_catalog), spawn + new Vector2(90f, -20f)));
                return;
            }

            _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateNormalStaw(_catalog), spawn + new Vector2(72f, -40f)));

            if (_currentRoom == 2 || _currentRoom == 5)
            {
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateCrystal(_catalog), spawn + new Vector2(120f, -56f)));
            }
        }

        private static CollectibleItem CreateCollectible(ItemAnimation animation, Vector2 position)
        {
            animation.Position = position;
            animation.Scale = GlobalConstants.DefaultScale;
            return new CollectibleItem(animation);
        }

        private void UpdateCollectibles(GameTime gameTime)
        {
            foreach (var collectible in _collectibles)
            {
                collectible.Update(gameTime);

                if (collectible.TryCollect(_player.Bounds))
                {
                    _player.canDash = true;
                    _player.Maddy.OnDashRefill();
                }
            }
        }

        private void DrawCollectibles(SpriteBatch spriteBatch)
        {
            foreach (var collectible in _collectibles)
            {
                collectible.Draw(spriteBatch);
            }
        }

        private Vector2 GetRespawnPointForRoom(int room)
        {
            return room switch
            {
                1 => new Vector2(250f, 200f),
                2 => new Vector2(150f, 300f),
                3 => new Vector2(200f, 150f),
                4 => new Vector2(250f, 150f),
                5 => new Vector2(175f, 150f),
                _ => new Vector2(200f, 150f),
            };
        }
    }

    
}
