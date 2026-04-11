using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celeste.Animation;
using Celeste.AudioSystem;
using Celeste.Blocks;
using Celeste.Blocks.Rooms;
using Celeste.Character;
using Celeste.CollectText;
using Celeste.Collision;
using Celeste.DeathAnimation.Particles;
using Celeste.DevTools;
using Celeste.Input;
using Celeste.Items;
using Celeste.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Celeste.Scenes
{
    public class GameplayScene : Scene
    {
        private AnimationCatalog _catalog;
        private Madeline _player;
        private Texture2D _pixelTexture;
        private DebugOverlay _debugOverlay;
        private Texture2D _deathDotTex;
        private ControllerLoader _controllerLoader;
        private SpriteFont _uiFont;
        private float _gameTimer = 0f;
        private bool _timerRunning = true;
        private bool _showUI = true;
        private KeyboardState _previousKeyboardState;

        private MapBuilder _worldMap;
        private RoomOne _roomOne;
        private RoomTwo _roomTwo;
        private RoomThree _roomThree;
        private RoomFour _roomFour;
        private RoomFive _roomFive;
        private int _currentRoom;

        private readonly List<CollectibleItem> _collectibles = new();
        private CollisionSystem _collisionSystem;
        private HazardCollisioncs _hazardCollisionSystem;
        private Rectangle _worldBound;

        public GameplayScene(Game1 game) : base(game)
        {
            _previousKeyboardState = Keyboard.GetState();
        }

        public override void LoadContent()
        {
            _catalog = AnimationLoader.LoadAll(Game.Content);
            CollectTextPrompt.Initialize(Game.Content);
            SoundManager.Load(Game.Content);

            _uiFont = Game.Content.Load<SpriteFont>("MenuFont");


            var startPos = new Vector2(
                Game.Window.ClientBounds.Width / 2f,
                Game.Window.ClientBounds.Height / 2f);

            _worldBound = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
            _player = new Madeline(Game.Content, _catalog, startPos);

            _deathDotTex = LoadDeathDotTexture();

            if (!_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathSide, out var deathSideClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathUp, out var deathUpClip)
                || !_catalog.Clips.TryGetValue(AnimationKeys.PlayerDeathDown, out var deathDownClip))
            {
                throw new ContentLoadException(
                    $"Directional death clips not found in AnimationCatalog. Available keys: {string.Join(", ", _catalog.Clips.Keys)}");
            }

            _player.ConfigureDeathAnimation(deathSideClip, deathUpClip, deathDownClip, _deathDotTex);

            _pixelTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });

            var factory = BlockFactory.GetInstance;
            factory.LoadTextures(Game.Content);

            _debugOverlay = new DebugOverlay();
            _controllerLoader = new ControllerLoader(Game, _player);
            InputMapper.ConfigureGameplay(
                _controllerLoader.GetKeyboard(),
                _controllerLoader.GetMouse(),
                _controllerLoader.GetGamepad(),
                Game,
                this);

            _worldMap = new MapBuilder(factory, _catalog, 50, 30);
            _roomOne = new RoomOne(_worldMap, factory);
            _roomTwo = new RoomTwo(_worldMap, factory);
            _roomThree = new RoomThree(_worldMap, factory);
            _roomFour = new RoomFour(_worldMap, factory);
            _roomFive = new RoomFive(_worldMap, factory);

            CollectibleItem.ResetStrawberryCount();
            RebuildCurrentRoom(resetPlayer: false);
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();
            _debugOverlay.HandleInput(keyboard, _player);
            _controllerLoader.Update();

            Vector2 previousPosition = _player.position;
            bool wasCrouching = _player.isCrouching;

            if (keyboard.IsKeyDown(Keys.T) && _previousKeyboardState.IsKeyUp(Keys.T))
            {
                _showUI = !_showUI;
            }
            _previousKeyboardState = keyboard;

            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GlobalConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
                return;
            }

            _player.Update(gameTime);
            _worldMap.Update(gameTime);

            if (_player.ConsumeLevelResetRequest())
            {
                RebuildCurrentRoom(resetPlayer: true);
            }
            else
            {
                _hazardCollisionSystem.ResolveHazardCollision();
                _collisionSystem.ResolveBlockCollision(previousPosition, wasCrouching);
                _player.UpdateFootstep((float)gameTime.ElapsedGameTime.TotalSeconds);
                UpdateCollectibles(gameTime);
                _player.UpdateSprite(gameTime);
            }

            if (_timerRunning)
            {
                _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_player.Bounds.Bottom > _worldBound.Bottom)
            {
                _player.Reset();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            _worldMap.Draw(spriteBatch);
            DrawCollectibles(spriteBatch);
            _player.Draw(spriteBatch);

            if (_debugOverlay.ShowDebug)
            {
                DrawUtils.DrawRectangleOutline(spriteBatch, _pixelTexture, _player.Bounds, Color.Red);
                _debugOverlay.Draw(spriteBatch, _player, _pixelTexture, Game.Window);
            }
            else
            {
                Game.Window.Title =
                    $"Celeste - {Game.Window.ClientBounds.Width}x{Game.Window.ClientBounds.Height} | Room: {_currentRoom} | BGM: {Game1.GetBgmStatusText()}";
            }

            if (_showUI)
            {
                string strawberryText = $"Strawberries: {CollectibleItem.StrawberryCount}";
                spriteBatch.DrawString(_uiFont, strawberryText, new Vector2(10, 10), Color.White);

                string timerText = $"Time: {FormatTime(_gameTimer)}";
                spriteBatch.DrawString(_uiFont, timerText, new Vector2(10, 40), Color.White);
            }

            spriteBatch.End();

        }


        private string FormatTime(float time)
        {
            int minutes = (int)(time / 60);
            int seconds = (int)(time % 60);
            int milliseconds = (int)((time * 100) % 100);
            return $"{minutes:D2}:{seconds:D2}.{milliseconds:D2}";
        }

        public void Reset()
        {
            RebuildCurrentRoom(resetPlayer: true);
        }

        public void JumpToRoom(int roomNumber)
        {
            if (roomNumber < 0 || roomNumber > 5)
            {
                return;
            }

            if (roomNumber != _currentRoom)
            {
                _currentRoom = roomNumber;
                RebuildCurrentRoom(resetPlayer: false);
            }
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

        private void RebuildCollisionSystems()
        {
            _collisionSystem = new CollisionSystem(_worldMap._blocks, _player);
            _hazardCollisionSystem = new HazardCollisioncs(_worldMap._hazards.Cast<ICollideable>().ToList(), _player);
            _player.SetWorldBlocks(_worldMap._blocks);
        }

        private void RebuildCollectibles()
        {
            _collectibles.Clear();

            Vector2 spawn = _player.RespawnPoint;
            if (_currentRoom == 0)
            {
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateNormalStaw(_catalog), spawn + new Vector2(-80f, -40f), CollectibleItem.ItemType.Strawberry));
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateFlyStaw(_catalog), spawn + new Vector2(0f, -60f), CollectibleItem.ItemType.Strawberry));
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateCrystal(_catalog), spawn + new Vector2(90f, -20f), CollectibleItem.ItemType.Crystal));
                return;
            }

            _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateNormalStaw(_catalog), spawn + new Vector2(72f, -40f), CollectibleItem.ItemType.Strawberry));

            if (_currentRoom == 2 || _currentRoom == 5)
            {
                _collectibles.Add(CreateCollectible(ItemAnimationFactory.CreateCrystal(_catalog), spawn + new Vector2(120f, -56f), CollectibleItem.ItemType.Crystal));
            }
        }

        private static CollectibleItem CreateCollectible(ItemAnimation animation, Vector2 position, CollectibleItem.ItemType itemType = CollectibleItem.ItemType.Strawberry)
        {
            animation.Position = position;
            animation.Scale = GlobalConstants.DefaultScale;
            return new CollectibleItem(animation, itemType);
        }

        private void UpdateCollectibles(GameTime gameTime)
        {
            foreach (var collectible in _collectibles)
            {
                collectible.Update(gameTime);

                if (collectible.TryCollect(_player.Bounds))
                {
                    SoundManager.Play("collect");
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

        private Texture2D LoadDeathDotTexture()
        {
            string dotPath = Path.Combine(Game.Content.RootDirectory, "DeathParticleDot.png");

            try
            {
                using Stream stream = TitleContainer.OpenStream(dotPath);
                Texture2D loaded = Texture2D.FromStream(Game.GraphicsDevice, stream);
                Texture2D trimmed = TrimTransparentBounds(loaded);

                if (!ReferenceEquals(trimmed, loaded))
                {
                    loaded.Dispose();
                }

                return trimmed;
            }
            catch
            {
                return ProceduralParticleTexture.CreateHardDot(Game.GraphicsDevice, size: 3);
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

            Texture2D trimmed = new Texture2D(Game.GraphicsDevice, width, height);
            trimmed.SetData(trimmedPixels);
            return trimmed;
        }
    }
}
