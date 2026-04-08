using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace Celeste.AudioSystem
{
    public static class SoundManager
    {

        private static Dictionary<string, SoundEffect> effects = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, List<SoundEffect>> footstepEffects = new();

        public static void Load(ContentManager content)
        {
           LoadCharacterMovementSE(content);
           LoadFootStepSE(content);



        }

        public static void LoadCharacterMovementSE(ContentManager content)
        {
            effects["collect"] = content.Load<SoundEffect>("Audio/collect_sound");
            effects["jump"] = content.Load<SoundEffect>("Audio/jump");
            effects["dash_left"] = content.Load<SoundEffect>("Audio/dash_left");
            effects["dash_right"] = content.Load<SoundEffect>("Audio/dash_right");
            effects["death"] = content.Load<SoundEffect>("Audio/death");


        }


        public static void LoadFootStepSE(ContentManager content)
        {
            footstepEffects["grass"] = SequentialSoundLoadHelper(content, "Audio/footstep/grass", "grass", 1, 7);
            
        }

        private static List<SoundEffect> SequentialSoundLoadHelper(ContentManager content,string folderPath,string filePrefix,int start,int end)
        {
            List<SoundEffect> sounds = new List<SoundEffect>();

            for (int i = start; i <= end; i++)
            {
                string assetName = $"{folderPath}/{filePrefix}_{i:D2}";
                sounds.Add(content.Load<SoundEffect>(assetName));
            }

            return sounds;
        }

        public static void Play(string key)
        {
            if (effects.ContainsKey(key))
            {
                effects[key].Play();
            }
        }

        public static void PlayFootstep(string blockType)
        {
            
        }


    }
}
