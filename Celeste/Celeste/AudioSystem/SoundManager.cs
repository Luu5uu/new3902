using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Celeste.AudioSystem
{
    public static class SoundManager
    {
        private static Dictionary<string, SoundEffect> effects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, List<SoundEffect>> footstepEffects = new Dictionary<string, List<SoundEffect>>();
        private static Dictionary<string, int> footstepIndex = new Dictionary<string, int>();

        private static XmlDocument soundConfig = new XmlDocument();

        public static void Load(ContentManager content)
        {
            effects.Clear();
            footstepEffects.Clear();
            footstepIndex.Clear();

            soundConfig.Load("Content/SoundConfig.xml");

            LoadCharacterMovementSE(content);
            LoadFootStepSE(content);
        }

        public static void LoadCharacterMovementSE(ContentManager content)
        {
            XmlNodeList effectNodes = soundConfig.SelectNodes("/SoundConfig/Effects/Effect");

            foreach (XmlNode node in effectNodes)
            {
                string key = node["Key"].InnerText;
                string asset = node["Asset"].InnerText;

                effects[key] = content.Load<SoundEffect>(asset);
            }
        }

        public static void LoadFootStepSE(ContentManager content)
        {
            XmlNodeList footstepNodes = soundConfig.SelectNodes("/SoundConfig/Footsteps/Footstep");

            foreach (XmlNode node in footstepNodes)
            {
                string blockType = node["BlockType"].InnerText;
                string folderPath = node["FolderPath"].InnerText;
                string filePrefix = node["FilePrefix"].InnerText;
                int start = int.Parse(node["Start"].InnerText);
                int end = int.Parse(node["End"].InnerText);

                footstepEffects[blockType] = SequentialSoundLoadHelper(content, folderPath, filePrefix, start, end);
                footstepIndex[blockType] = 0;
            }
        }

        private static List<SoundEffect> SequentialSoundLoadHelper(ContentManager content, string folderPath, string filePrefix, int start, int end)
        {
            List<SoundEffect> sounds = new List<SoundEffect>();

            for (int i = start; i <= end; i++)
            {
                string assetName = folderPath + "/" + filePrefix + "_" + i.ToString("D2");
                sounds.Add(content.Load<SoundEffect>(assetName));
            }

            return sounds;
        }

        public static void Play(string key)
        {
            if (effects.ContainsKey(key))
            {
                if(key == "collect")
                {
                    var instance = effects[key].CreateInstance();
                    instance.Volume = 0.25f; // Set the desired volume
                    instance.Play();
                    return;
                }
                effects[key].Play();
            }
        }

        public static void PlayFootstep(string blockType)
        {
            if (footstepEffects.ContainsKey(blockType))
            {
                List<SoundEffect> sounds = footstepEffects[blockType];

                if (sounds.Count > 0)
                {
                    int index = footstepIndex[blockType];
                    sounds[index].Play();

                    index = index + 1;

                    if (index >= sounds.Count)
                    {
                        index = 0;
                    }

                    footstepIndex[blockType] = index;
                }
            }
        }
    }
}