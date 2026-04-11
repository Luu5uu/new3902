using Celeste.Input;
using Celeste.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using BgmAudioPlayer = Celeste.BGMPlayer.BGMPlayer;

namespace Celeste
{
    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private KeyboardState _previousKeyboardState;
        private bool _bgmInitialized;

        public SpriteBatch SpriteBatch { get; private set; }

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _previousKeyboardState = Keyboard.GetState();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            InitializeBgm();
            SceneManager.ChangeScene(new MainMenuScene(this));
        }

        protected override void Update(GameTime gameTime)
        {
            HandleGlobalHotkeys();
            SceneManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            SceneManager.Draw(SpriteBatch);
            base.Draw(gameTime);
        }

        public void Reset()
        {
            var gameplay = SceneManager.GetScene<GameplayScene>();
            gameplay?.Reset();
        }

        public void CycleGameScene(int direction)
        {
            var gameplay = SceneManager.GetScene<GameplayScene>();
            gameplay?.CycleGameScene(direction);
        }

        public void PlayPreviousBgm()
        {
            if (BgmAudioPlayer.TrackCount > 0)
            {
                BgmAudioPlayer.bgmPrevious();
            }
        }

        public void PlayNextBgm()
        {
            if (BgmAudioPlayer.TrackCount > 0)
            {
                BgmAudioPlayer.bgmNext();
            }
        }

        public void PauseBgm() => BgmAudioPlayer.bgmPause();

        public void ResumeBgm() => BgmAudioPlayer.bgmPlay();

        public static string GetBgmStatusText()
        {
            string trackName = string.IsNullOrWhiteSpace(BgmAudioPlayer.CurrentTrackName)
                ? "none"
                : BgmAudioPlayer.CurrentTrackName;

            return $"{trackName} ({MediaPlayer.State})";
        }

        private void InitializeBgm()
        {
            if (_bgmInitialized)
            {
                return;
            }

            BgmAudioPlayer.Initialize(Content);
            BgmAudioPlayer.bgmSwitchTo("prologue");
            _bgmInitialized = true;
        }

        private void HandleGlobalHotkeys()
        {
            var keyboard = Keyboard.GetState();

            if (IsNewKeyPress(keyboard, Keys.F5))
            {
                PlayPreviousBgm();
            }
            else if (IsNewKeyPress(keyboard, Keys.F6))
            {
                PlayNextBgm();
            }
            else if (IsNewKeyPress(keyboard, Keys.F7))
            {
                PauseBgm();
            }
            else if (IsNewKeyPress(keyboard, Keys.F8))
            {
                ResumeBgm();
            }

            _previousKeyboardState = keyboard;
        }

        private bool IsNewKeyPress(KeyboardState currentState, Keys key)
        {
            return currentState.IsKeyDown(key) && !_previousKeyboardState.IsKeyDown(key);
        }
    }
}
