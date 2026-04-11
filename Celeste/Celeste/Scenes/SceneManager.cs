using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Scenes
{
    public static class SceneManager
    {
        private static Stack<Scene> _activeScenes = new Stack<Scene>();
        private static bool _sceneChangedThisFrame = false;

        public static Scene ActiveScene => _activeScenes.Count > 0 ? _activeScenes.Peek() : null;

        public static T GetScene<T>() where T : Scene
        {
            return _activeScenes.OfType<T>().FirstOrDefault();
        }
        public static void PushScene(Scene scene)
        {
            scene.LoadContent();
            _activeScenes.Push(scene);
            _sceneChangedThisFrame = true;
        }

        public static void PopScene()
        {
            if (_activeScenes.Count > 0)
                _activeScenes.Pop();
        }

        public static void ChangeScene(Scene scene)
        {
            _activeScenes.Clear();
            PushScene(scene);
        }

        public static void Update(GameTime gameTime)
        {
            if (_sceneChangedThisFrame)
            {
                _sceneChangedThisFrame = false;
                return;
            }

            var scenes = _activeScenes.ToArray();
            foreach (var scene in scenes)
            {
                scene.Update(gameTime);
                if (scene.BlocksUpdate) break;
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            var scenes = _activeScenes.ToArray();
            System.Array.Reverse(scenes); // Draw bottom-to-top

            foreach (var scene in scenes)
            {
                scene.Draw(spriteBatch);
                if (scene.BlocksDraw) break;
            }
        }
    }
}