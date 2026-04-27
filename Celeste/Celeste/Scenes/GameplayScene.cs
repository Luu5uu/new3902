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
using Microsoft.Xna.Framework.Media;
using System;
using BgmAudioPlayer = Celeste.BGMPlayer.BGMPlayer;

namespace Celeste.Scenes
{
    public class GameplayScene : Scene
    {
        private readonly record struct RoomTransitionZone(int SourceRoom, Rectangle TriggerArea, int TargetRoom);
        private const string GameplayTrackOne = "first-steps";
        private const string GameplayTrackTwo = "resurrections";

        private AnimationCatalog _catalog;
        private Madeline _player;
        private Texture2D _pixelTexture;
        private DebugOverlay _debugOverlay;
        private Texture2D _deathDotTex;
        private Texture2D _background;
        private ControllerLoader _controllerLoader;
        private SpriteFont _uiFont;

        private float _gameTimer = 0f;
        private bool _timerRunning = true;
        private bool _showUI = true;
        private KeyboardState _previousKeyboardState;
        private static int _sessionDeathCount = 0;
        private static int _sessionCollectedStrawberryCount = 0;
        private static readonly HashSet<string> _sessionCollectedStrawberryIds = new();

        private MapBuilder _worldMap;
        private RoomOne _roomOne;
        private RoomTwo _roomTwo;
        private RoomThree _roomThree;
        private RoomFour _roomFour;
        private RoomFive _roomFive;
        private RoomCustom _roomCustom;
        private int _currentRoom = 1;

        private readonly List<CollectibleItem> _collectibles = new();
        private CollisionSystem _collisionSystem;
        private HazardCollisioncs _hazardCollisionSystem;
        private Rectangle _worldBound;

        private bool _isRecordingRewind;
        private bool _isRewinding;
        private float _rewindRecordTimer;
        private float _rewindCooldownTimer;

        private const float RewindRecordDuration = 3.0f;
        private const float RewindCooldownDuration = 3.0f;
        private static readonly RoomTransitionZone[] RoomTransitionZones =
        {
            new(1, new Rectangle(620, 0, 120, 36), 2),
            new(2, new Rectangle(680, 0, 80, 36), 3),
            new(3, new Rectangle(660, 0, 80, 36), 4),
            new(4, new Rectangle(400, 0, 80, 36), 5),
        };


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
            _background = Game.Content.Load<Texture2D>("bg");


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
            _roomCustom = new RoomCustom(_worldMap, factory);


            StartGameplayBgm();
            RebuildCurrentRoom(resetPlayer: false);
            _rewindCooldownTimer = RewindCooldownDuration;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState keyboard = Keyboard.GetState();

            _debugOverlay.HandleInput(keyboard, _player);
            _controllerLoader.Update();
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_rewindCooldownTimer > 0f)
            {
                _rewindCooldownTimer -= dt;
                if (_rewindCooldownTimer < 0f)
                {
                    _rewindCooldownTimer = 0f;
                }
            }
            Vector2 previousPosition = _player.position;
            bool wasCrouching = _player.isCrouching;
            bool wasDashing = _player.isDashing;

            if (keyboard.IsKeyDown(Keys.T) && _previousKeyboardState.IsKeyUp(Keys.T))
            {
                _showUI = !_showUI;
            }

            bool rewindPressed = keyboard.IsKeyDown(Keys.V) && _previousKeyboardState.IsKeyUp(Keys.V);

            if (rewindPressed && !_player.IsInDeathSequence && _rewindCooldownTimer <= 0f)
            {
                if (!_isRecordingRewind && !_isRewinding)
                {
                    _isRecordingRewind = true;
                    _rewindRecordTimer = RewindRecordDuration;

                    _player.ClearRewindHistory();
                    _player.SeedInitialRewindSnapshot();
                }
                else if (_isRecordingRewind)
                {
                    _isRecordingRewind = false;

                    if (_player.CanRewind)
                    {
                        _isRewinding = true;
                    }
                    else
                    {
                        _player.ClearRewindHistory();
                        _player.SeedInitialRewindSnapshot();
                    }

                    _rewindCooldownTimer = RewindCooldownDuration;
                }
            }


            if (_debugOverlay.ShowDebug && _player.Maddy.DebugPaused)
            {
                _player.Maddy.SetPosition(_player.position, scale: GlobalConstants.DefaultScale, faceLeft: _player.FaceLeft);
                _player.Maddy.Update(gameTime);
                return;
            }


            if (_isRewinding && !_player.IsInDeathSequence)
            {
                bool rewound = _player.StepRewind();
                _player.UpdateSprite(gameTime);

                if (!rewound)
                {
                    _isRewinding = false;
                    _player.ClearRewindHistory();
                    _player.SeedInitialRewindSnapshot();
                }

                _previousKeyboardState = keyboard;
                return;
            }



            _player.Update(gameTime);
            _worldMap.Update(gameTime);
            UpdateGameplayBgm();

            if (!wasDashing && _player.isDashing)
            {
                TriggerDashScaredCollectibles();
            }

            if (_player.ConsumeLevelResetRequest())
            {
                _sessionDeathCount++;
                RebuildCurrentRoom(resetPlayer: true);
                _rewindCooldownTimer = RewindCooldownDuration;
            }
            else
            {
                _hazardCollisionSystem.ResolveHazardCollision();
                _collisionSystem.ResolveBlockCollision(previousPosition, wasCrouching);

                if (TryHandleRoomTransition(gameTime))
                {
                    _previousKeyboardState = keyboard;
                    return;
                }

                _player.UpdateClimbSound((float)gameTime.ElapsedGameTime.TotalSeconds);
                _player.UpdateFootstep((float)gameTime.ElapsedGameTime.TotalSeconds);
                UpdateCollectibles(gameTime);
                _player.UpdateSprite(gameTime);

                if (_isRecordingRewind)
                {
                    _player.SaveRewindSnapshot();
                }

            }

            if (_isRecordingRewind)
            {
                _rewindRecordTimer -= dt;

                if (_rewindRecordTimer <= 0f)
                {
                    _isRecordingRewind = false;
                    _rewindRecordTimer = 0f;
                    _rewindCooldownTimer = RewindCooldownDuration;

                    _player.ClearRewindHistory();
                    _player.SeedInitialRewindSnapshot();
                }
            }

            if (_timerRunning)
            {
                _gameTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (_player.Bounds.Bottom > _worldBound.Bottom && !_player.IsInDeathSequence)
            {
                _player.Die();
            }

            _previousKeyboardState = keyboard;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                rasterizerState: RasterizerState.CullNone);

            spriteBatch.Draw(_background, Game.GraphicsDevice.Viewport.Bounds, Color.White);
            _worldMap.Draw(spriteBatch);
            DrawCollectibles(spriteBatch);
            if (_isRecordingRewind || _isRewinding)
            {
                _player.DrawRewindTrail(spriteBatch, _pixelTexture);
            }

            if (_isRewinding)
            {
                DrawRewindDarkOverlay(spriteBatch);
            }
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
                string strawberryText = $"Strawberries: {_sessionCollectedStrawberryCount}";
                spriteBatch.DrawString(_uiFont, strawberryText, new Vector2(10, 10), Color.White);

                string deathText = $"Deaths: {_sessionDeathCount}";
                spriteBatch.DrawString(_uiFont, deathText, new Vector2(10, 40), Color.White);

                string timerText = $"Time: {FormatTime(_gameTimer)}";
                spriteBatch.DrawString(_uiFont, timerText, new Vector2(10, 70), Color.White);
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

            _isRecordingRewind = false;
            _isRewinding = false;
            _rewindRecordTimer = 0f;
            _rewindCooldownTimer = RewindCooldownDuration;

            _gameTimer = 0f;
            _timerRunning = true;
        }

        public void JumpToRoom(int roomNumber)
        {
            ChangeRoom(roomNumber, resetPlayer: false);
        }

        public void CycleGameScene(int direction)
        {
            const int firstRoom = 1;
            const int lastRoom = 6;

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
                ChangeRoom(nextRoom, resetPlayer: false);
            }
        }

        private void ChangeRoom(int roomNumber, bool resetPlayer)
        {
            if (roomNumber < 0 || roomNumber > 6 || roomNumber == _currentRoom)
            {
                return;
            }

            _currentRoom = roomNumber;
            RebuildCurrentRoom(resetPlayer);
        }

        private bool TryHandleRoomTransition(GameTime gameTime)
        {
            Rectangle playerBounds = _player.Bounds;

            for (int i = 0; i < RoomTransitionZones.Length; i++)
            {
                RoomTransitionZone zone = RoomTransitionZones[i];
                if (zone.SourceRoom != _currentRoom || !playerBounds.Intersects(zone.TriggerArea))
                {
                    continue;
                }

                SceneManager.PushScene(new ScreenWipeScene(Game, () =>{
                    _isRecordingRewind = false;
                    _isRewinding = false;
                    _rewindRecordTimer = 0f;
                    _rewindCooldownTimer = RewindCooldownDuration;
                    ChangeRoom(zone.TargetRoom, resetPlayer: true);
                    _player.UpdateSprite(gameTime);
                }));
                
                return true;
            }

            return false;
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
            _player.SeedInitialRewindSnapshot();
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
                case 6:
                    _roomCustom.PlaceRoomCustomBlocks();
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
                _collectibles.Add(CreateCollectible(
                    "room0_straw_0",
                    ItemAnimationFactory.CreateNormalStaw(_catalog),
                    spawn + new Vector2(-80f, -40f),
                    CollectibleItem.ItemType.Strawberry));

                _collectibles.Add(CreateCollectible(
                    "room0_straw_1",
                    ItemAnimationFactory.CreateFlyStaw(_catalog),
                    spawn + new Vector2(0f, -60f),
                    CollectibleItem.ItemType.Strawberry,
                    fliesAwayOnDash: true));

                _collectibles.Add(CreateCollectible(
                    "room0_crystal_0",
                    ItemAnimationFactory.CreateCrystal(_catalog),
                    spawn + new Vector2(90f, -20f),
                    CollectibleItem.ItemType.Crystal));

                return;
            }

            if (_currentRoom == 2)
            {
                _collectibles.Add(CreateCollectible(
                    "room2_straw_0",
                    ItemAnimationFactory.CreateNormalStaw(_catalog),
                    new Vector2(207f, 68f),
                    CollectibleItem.ItemType.Strawberry));
            }

            if (_currentRoom == 3)
            {
                _collectibles.Add(CreateCollectible(
                    "room3_straw_0",
                    ItemAnimationFactory.CreateNormalStaw(_catalog),
                    new Vector2(689f, 275f),
                    CollectibleItem.ItemType.Strawberry));

                _collectibles.Add(CreateCollectible(
                    "room3_crystal_0",
                    ItemAnimationFactory.CreateCrystal(_catalog),
                    new Vector2(701f, 330f),
                    CollectibleItem.ItemType.Crystal));
            }

            if (_currentRoom == 4)
            {
                _collectibles.Add(CreateCollectible(
                    "room4_straw_0",
                    ItemAnimationFactory.CreateFlyStaw(_catalog),
                    new Vector2(279f, 132f),
                    CollectibleItem.ItemType.Strawberry,
                    fliesAwayOnDash: true));
            }

             if (_currentRoom == 6)
            {
                _collectibles.Add(CreateCollectible(
                    "roomCustom_straw_0",
                    ItemAnimationFactory.CreateFlyStaw(_catalog),
                    new Vector2(37f, 365f),
                    CollectibleItem.ItemType.Strawberry,
                    fliesAwayOnDash: true));
            }
        }

        private void TriggerDashScaredCollectibles()
        {
            foreach (var collectible in _collectibles)
            {
                collectible.TriggerFlyAway();
            }
        }

        private CollectibleItem CreateCollectible(
            string collectibleId,
            ItemAnimation animation,
            Vector2 position,
            CollectibleItem.ItemType itemType = CollectibleItem.ItemType.Strawberry,
            bool fliesAwayOnDash = false)
        {
            animation.Position = position;
            animation.Scale = GlobalConstants.DefaultScale;

            var collectible = new CollectibleItem(collectibleId, animation, itemType, fliesAwayOnDash);

            if (itemType == CollectibleItem.ItemType.Strawberry)
            {
                collectible.SetPreviouslyCollected(_sessionCollectedStrawberryIds.Contains(collectibleId));
            }

            return collectible;
        }

        private void UpdateCollectibles(GameTime gameTime)
        {
            foreach (var collectible in _collectibles)
            {
                collectible.Update(gameTime);

                if (collectible.TryCollect(_player.Bounds))
                {
                    if (collectible.Type == CollectibleItem.ItemType.Strawberry &&
                        !_sessionCollectedStrawberryIds.Contains(collectible.CollectibleId))
                    {
                        _sessionCollectedStrawberryIds.Add(collectible.CollectibleId);
                        _sessionCollectedStrawberryCount++;
                        collectible.SetPreviouslyCollected(true);
                    }

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
                1 => new Vector2(45f, 336f),
                2 => new Vector2(150f, 378f),
                3 => new Vector2(150f, 396f),
                4 => new Vector2(190f, 390f),
                5 => new Vector2(160f, 376f),
                6 => new Vector2(700f, 300f),
                _ => new Vector2(200f, 150f),
            };
        }

        private static void StartGameplayBgm()
        {
            MediaPlayer.IsRepeating = false;
            if (!string.Equals(BgmAudioPlayer.CurrentTrackName, GameplayTrackOne, System.StringComparison.OrdinalIgnoreCase))
            {
                BgmAudioPlayer.bgmSwitchTo(GameplayTrackOne);
            }
            else
            {
                BgmAudioPlayer.bgmPlay();
            }
        }

        private static void UpdateGameplayBgm()
        {
            MediaPlayer.IsRepeating = false;

            string currentTrack = BgmAudioPlayer.CurrentTrackName;
            bool isGameplayTrack =
                string.Equals(currentTrack, GameplayTrackOne, System.StringComparison.OrdinalIgnoreCase) ||
                string.Equals(currentTrack, GameplayTrackTwo, System.StringComparison.OrdinalIgnoreCase);

            if (!isGameplayTrack)
            {
                BgmAudioPlayer.bgmSwitchTo(GameplayTrackOne);
                return;
            }

            if (MediaPlayer.State != MediaState.Stopped)
            {
                return;
            }

            string nextTrack = string.Equals(currentTrack, GameplayTrackOne, System.StringComparison.OrdinalIgnoreCase)
                ? GameplayTrackTwo
                : GameplayTrackOne;
            BgmAudioPlayer.bgmSwitchTo(nextTrack);
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

        private void DrawRewindDarkOverlay(SpriteBatch spriteBatch)
        {
            Rectangle screenRect = new Rectangle(
                0,
                0,
                Game.GraphicsDevice.Viewport.Width,
                Game.GraphicsDevice.Viewport.Height);

            spriteBatch.Draw(_pixelTexture, screenRect, Color.Black * 0.6f);
        }
    }
}
