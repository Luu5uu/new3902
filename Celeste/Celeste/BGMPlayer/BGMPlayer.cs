using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Celeste.BGMPlayer
{
    public static class BGMPlayer
    {
        private static ContentManager _content;
        private static readonly Dictionary<string, string> _trackMap = new(StringComparer.OrdinalIgnoreCase);
        private static readonly List<string> _trackOrder = new();

        private static Song _currentSong;
        private static string _currentTrackName;
        private static int _currentTrackIndex = -1;
        private static bool _initialized = false;

        /// <summary>
        /// initial BGMPlayer.only need one call in LoadContent
        /// xmlRelativePath example: "BGMPlayer/BGMLibrary.xml"
        /// </summary>
        public static void Initialize(ContentManager content, string xmlRelativePath = "BGMPlayer/BGMLibrary.xml", bool isRepeating = true, float volume = 0.5f)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));

            _content = content;
            _trackMap.Clear();
            _trackOrder.Clear();
            _currentSong = null;
            _currentTrackName = null;
            _currentTrackIndex = -1;

            string fullPath = ResolveContentFilePath(content.RootDirectory, xmlRelativePath);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"BGM library xml not found: {fullPath}");

            XmlSerializer serializer = new XmlSerializer(typeof(BGMLibrary));
            using FileStream fs = File.OpenRead(fullPath);
            BGMLibrary library = (BGMLibrary)serializer.Deserialize(fs);

            if (library?.Tracks == null || library.Tracks.Count == 0)
                throw new InvalidOperationException("BGM library is empty.");

            foreach (var track in library.Tracks)
            {
                if (string.IsNullOrWhiteSpace(track.Name) || string.IsNullOrWhiteSpace(track.Asset))
                    continue;

                _trackMap[track.Name] = track.Asset;
                _trackOrder.Add(track.Name);
            }

            if (_trackOrder.Count == 0)
                throw new InvalidOperationException("BGM library does not contain any valid tracks.");

            MediaPlayer.IsRepeating = isRepeating;
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);

            _initialized = true;
        }

        private static string ResolveContentFilePath(string rootDirectory, string relativePath)
        {
            string cwdPath = Path.Combine(rootDirectory, relativePath);
            if (File.Exists(cwdPath))
            {
                return cwdPath;
            }

            string outputPath = Path.Combine(AppContext.BaseDirectory, rootDirectory, relativePath);
            if (File.Exists(outputPath))
            {
                return outputPath;
            }

            return cwdPath;
        }

        /// <summary>
        /// switch music
        /// example: BGMPlayer.bgmSwitchTo("room1");
        /// </summary>
        public static void bgmSwitchTo(string trackName)
        {
            EnsureInitialized();

            if (string.IsNullOrWhiteSpace(trackName))
                throw new ArgumentException("trackName cannot be null or empty.", nameof(trackName));

            if (!_trackMap.TryGetValue(trackName, out string assetName))
                throw new KeyNotFoundException($"Track \"{trackName}\" not found in BGMLibrary.xml");

            if (string.Equals(_currentTrackName, trackName, StringComparison.OrdinalIgnoreCase))
            {
                bgmPlay();
                return;
            }

            Song nextSong = _content.Load<Song>(assetName);

            MediaPlayer.Stop();
            _currentSong = nextSong;
            _currentTrackName = trackName;
            _currentTrackIndex = _trackOrder.FindIndex(name => string.Equals(name, trackName, StringComparison.OrdinalIgnoreCase));
            MediaPlayer.Play(_currentSong);
        }

        public static void bgmNext()
        {
            EnsureInitialized();
            ChangeTrack(1);
        }

        public static void bgmPrevious()
        {
            EnsureInitialized();
            ChangeTrack(-1);
        }

        /// <summary>
        /// pause current BGM。
        /// </summary>
        public static void bgmPause()
        {
            EnsureInitialized();

            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// play BGM。
        /// </summary>
        public static void bgmPlay()
        {
            EnsureInitialized();

            if (_currentSong == null)
                return;

            if (MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
            else if (MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(_currentSong);
            }
        }

        /// <summary>
        /// stop play
        /// </summary>
        public static void bgmStop()
        {
            EnsureInitialized();
            MediaPlayer.Stop();
        }

        public static void SetVolume(float volume)
        {
            EnsureInitialized();
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
        }

        public static string CurrentTrackName => _currentTrackName;
        public static int TrackCount => _trackOrder.Count;

        private static void EnsureInitialized()
        {
            if (!_initialized)
                throw new InvalidOperationException("BGMPlayer is not initialized. Call BGMPlayer.Initialize(...) first.");
        }

        private static void ChangeTrack(int direction)
        {
            if (_trackOrder.Count == 0)
                throw new InvalidOperationException("BGM library does not contain any playable tracks.");

            if (_currentTrackIndex < 0)
            {
                _currentTrackIndex = 0;
            }
            else
            {
                _currentTrackIndex = (_currentTrackIndex + direction + _trackOrder.Count) % _trackOrder.Count;
            }

            bgmSwitchTo(_trackOrder[_currentTrackIndex]);
        }
    }
}
