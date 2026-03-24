using System;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Celeste.Blocks
{
    public class BlockFactory
    {
        private static readonly BlockFactory Instance = new BlockFactory();
        public static BlockFactory GetInstance => Instance;

        public Dictionary<string, Texture2D> BlockTextures { get; } = new Dictionary<string, Texture2D>();

        public Dictionary<string, int> frames = new Dictionary<string, int>();
        private BlockFactory() { }

        /// <summary>
        /// Load block textures that exist in Content. Skips missing assets so the game can run without all block art.
        /// </summary>
        public void LoadTextures(ContentManager content)
        {
            TryLoad(content, "snow");
            TryLoad(content, "cement");
            TryLoad(content, "dirt");
            TryLoad(content, "girder");
            TryLoad(content, "4");
            TryLoad(content, "7");
            TryLoad(content, "upSpike");
            TryLoad(content, "rightSpike");
            TryLoad(content, "downSpike");
            TryLoad(content, "leftSpike");
            TryLoad(content, "spring");
            TryLoad(content, "top_a00");
            TryLoad(content, "top_a01");
            TryLoad(content, "top_a02");
            TryLoad(content, "top_a03");

            // new for sprint 3
            TryLoad(content, "box");
            TryLoad(content, "boxAndBottle");
            TryLoad(content, "constructionSign");
            TryLoad(content, "ladder");
            TryLoad(content, "paintBuckets");
            TryLoad(content, "signE");
            TryLoad(content, "signGreenForward");
            TryLoad(content, "signNoDash");
            TryLoad(content, "signNoUp");
            TryLoad(content, "trafficLight");
            TryLoad(content, "trafficLightBroken");
            TryLoad(content, "wood");

            CalculateFrames();
        }

        private void TryLoad(ContentManager content, string name)
        {
            try
            {
                BlockTextures[name] = content.Load<Texture2D>(name);
            }
            catch
            {
                // Texture not in Content; block type will be skipped when building gallery
            }
        }

        private void CalculateFrames()
        {
            foreach (var kvp in BlockTextures)
            {
                string type = kvp.Key;
                Texture2D texture = kvp.Value;

                int framesPerRow = texture.Width / 8;
                int framesPerColumn = texture.Height / 8;
                frames[type] = framesPerRow * framesPerColumn;
            }
        }

        public IBlocks CreateSnowBlock(Vector2 position) => CreateBlock("snow", position);
        public IBlocks CreateCementBlock(Vector2 position) => CreateBlock("cement", position);
        public IBlocks CreateDirtBlock(Vector2 position) => CreateBlock("dirt", position);
        public IBlocks CreateGirderBlock(Vector2 position) => CreateBlock("girder", position);

        public IBlocks CreateBlock(string type, Vector2 position)
        {
            return CreateBlock(type, position, 0);
        }

        public IBlocks CreateBlock(string type, Vector2 position, int frameNum, float scale = 2.5f)
        {
            if (BlockTextures.TryGetValue(type, out var texture))
            {

                if (frames.TryGetValue(type, out int maxFrames))
                {
                    frameNum = frameNum % maxFrames;
                    // Loop the frame number if it exceeds available frames
                }
                return new Blocks(type, position, texture, frameNum, scale);

            }
            return null;
        }


        public int GetFrameCount(string type)
        {
            if (frames.TryGetValue(type, out int count))
            {
                return count;
            }
            return 0;
        }


        public IHazard CreateHazard(string type, Vector2 position)
        {
            return BlockTextures.TryGetValue(type, out var tex)
                ? new Hazard(type, position, tex)
                : null;
        }
    }
}
