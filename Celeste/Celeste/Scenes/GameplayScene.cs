using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Celeste.Animation;
using Celeste.Character;
using Celeste.Items;
using Celeste.Blocks;
using Celeste.Blocks.Rooms;
using Celeste.DevTools;
using Celeste.Input;
using Celeste.DeathAnimation.Particles;
using Celeste.Utils;
using Celeste.Collision;
using Celeste.Scenes;

namespace Celeste.Scenes
{
    public class GameplayScene : Scene
    {
        // --- Logic and Content moved from Game1 ---
        private AnimationCatalog _catalog;
        private Madeline _player;
        private KeyboardState _prevKb;
        private Texture2D _pixelTexture;
        private DebugOverlay _debugOverlay;
        private Texture2D _deathDotTex;
        private ControllerLoader _controllerLoader;

        private MapBuilder _worldMap;
        private RoomOne _roomOne;
        private RoomTwo _roomTwo;
        private RoomThree _roomThree;
        private RoomFour _roomFour;
        private RoomFive _roomFive;
        private int _currentRoom = 0;

        private List<IBlocks> _blockList;
        private readonly List<CollectibleItem> _collectibles = new();
        private int _totalBlocks;

        private CollisionSystem _collisionSystem;
        private HazardCollisioncs HazardCollisioncs;
        private Rectangle worldBound;

        public GameplayScene(Game1 game) : base(game) { }

        public override void LoadContent()
        {
            // Use Game.Content and Game.GraphicsDevice from the base class
            _catalog = AnimationLoader.LoadAll(Game.Content);

            var startPos = new Vector2(
                Game.Window.ClientBounds.Width / 2f,
                Game.Window.ClientBounds.Height / 2f);

            worldBound = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            _player = new Madeline(Game.Content, _catalog, startPos);

            // Inject DeathAnimation resources
            _deathDotTex = LoadDeathDotTexture();

            if (!_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathSide, out var deathSideClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathUp, out var deathUpClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathDown, out var deathDownClip))
                throw new ContentLoadException(
                    $"Directional death clips not found in AnimationCatalog. " +
                    $"Available keys: {string.Join(", ", _catalog.Clips.Keys)}");

            _player.ConfigureDeathAnimation(deathSideClip, deathUpClip, deathDownClip, _deathDotTex);

            _pixelTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            var factory = BlockFactory.GetInstance;
            factory.LoadTextures(Game.Content);
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
            _controllerLoader = new ControllerLoader(Game, _player);
            InputMapper.ConfigureGameplay(_controllerLoader.GetKeyboard(), Game, this);

            _worldMap = new MapBuilder(factory, 50, 30);
            _roomOne = new RoomOne(_worldMap, factory);
            _roomTwo = new RoomTwo(_worldMap, factory);
            _roomThree = new RoomThree(_worldMap, factory);
            _roomFour = new RoomFour(_worldMap, factory);
            _roomFive = new RoomFive(_worldMap, factory);

            RebuildCurrentRoom(resetPlayer: false);
        }

        public override void Update(GameTime gameTime)
        {
            var kb = Keyboard.GetState();
            _controllerLoader.Update();

            // --- REGULAR GAMEPLAY UPDATE ---
            _debugOverlay.HandleInput(kb, _player);

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

            if (_player.Bounds.Bottom > worldBound.Bottom)
            {
                _player.Reset();
            }

            _prevKb = kb;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Note: Game1.cs handles GraphicsDevice.Clear
            // We handle the actual drawing block
            // CullNone needed: BodySprite flips via negative X scale.
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            _worldMap.Draw(spriteBatch);
            DrawCollectibles(spriteBatch);
            _player.Draw(spriteBatch);

            if (_debugOverlay.ShowDebug)
                DrawUtils.DrawRectangleOutline(spriteBatch, _pixelTexture, _player.Bounds, Color.Red);

            // Draw current block/obstacle only when block display is on (T = previous, Y = next). Stationary, no interaction.
            /*if (_blocksVisible && _totalBlocks > 0)
            {
                var block = _blockList[_activeBlockIndex];
                block.Position = new Vector2(BlockConstants.BlockDisplayX, BlockConstants.BlockDisplayY);
                block.Draw(_spriteBatch);
                DrawUtils.DrawRectangleOutline( _spriteBatch, _pixelTexture, block.Bounds, Color.Lime);
            }*/

            if (_debugOverlay.ShowDebug)
                _debugOverlay.Draw(spriteBatch, _player, _pixelTexture, Game.Window);
            else

                // to get resolution
                Game.Window.Title = $"Celeste - {Game.Window.ClientBounds.Width}x{Game.Window.ClientBounds.Height}";

            spriteBatch.End();
        }

        // Helper Methods

        public void JumpToRoom(int roomNumber)
        {
            _currentRoom = roomNumber;
            RebuildCurrentRoom(resetPlayer: false);
        }
        private void RebuildCurrentRoom(bool resetPlayer)
        {
            BuildMap();
            RebuildCollisionSystems();
            RebuildCollectibles();
            if (resetPlayer) _player.Reset();
        }

        private void BuildMap()
        {
            if (_worldMap == null) return;
            _worldMap.ClearBlocks();

            switch (_currentRoom)
            {
                case 1: _roomOne.PlaceRoomOneBlocks(); break;
                case 2: _roomTwo.PlaceRoomTwoBlocks(); break;
                case 3: _roomThree.PlaceRoomThreeBlocks(); break;
                case 4: _roomFour.PlaceRoomFourBlocks(); break;
                case 5: _roomFive.PlaceRoomFiveBlocks(); break;
                default: _roomThree.PlaceRoomThreeBlocks(); break;
            }

            Vector2 checkpointSpawn = GetRespawnPointForRoom(_currentRoom);
            _player.position = checkpointSpawn;
            _player.RespawnPoint = checkpointSpawn;
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
            foreach (var collectible in _collectibles) collectible.Draw(spriteBatch);
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

        private void HandleRoomHotkeys(KeyboardState kb)
        {
            int requestedRoom = _currentRoom;
            if (kb.IsKeyDown(Keys.D1) && _prevKb.IsKeyUp(Keys.D1)) requestedRoom = 1;
            else if (kb.IsKeyDown(Keys.D2) && _prevKb.IsKeyUp(Keys.D2)) requestedRoom = 2;
            else if (kb.IsKeyDown(Keys.D3) && _prevKb.IsKeyUp(Keys.D3)) requestedRoom = 3;

            if (requestedRoom != _currentRoom)
            {
                _currentRoom = requestedRoom;
                RebuildCurrentRoom(resetPlayer: false);
            }
        }

        private Texture2D LoadDeathDotTexture()
        {
            // Procedural fallback if file missing
            return ProceduralParticleTexture.CreateHardDot(Game.GraphicsDevice, size: 3);
        }
         // This fixes the Reset() error
public void Reset() => RebuildCurrentRoom(resetPlayer: true);

// This fixes the CycleGameScene error
public void CycleGameScene(int direction)
{
    const int firstRoom = 1;
    const int lastRoom = 5;

    int nextRoom = _currentRoom;
    if (nextRoom < firstRoom || nextRoom > lastRoom) nextRoom = 3;

    nextRoom += direction;
    if (nextRoom < firstRoom) nextRoom = lastRoom;
    else if (nextRoom > lastRoom) nextRoom = firstRoom;

    if (nextRoom != _currentRoom)
    {
        _currentRoom = nextRoom;
        RebuildCurrentRoom(resetPlayer: false);
    }
}
         
    }
}