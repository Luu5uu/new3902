using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Celeste.Blocks
{
    public class BlockFactory
    {
        private static readonly BlockFactory Instance = new BlockFactory();
        public static BlockFactory GetInstance => Instance;

        public Dictionary<string, Texture2D> BlockTextures { get; } = new Dictionary<string, Texture2D>();

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
            TryLoad(content, "spikeUp");
            TryLoad(content, "top_a00");
            TryLoad(content, "top_a01");
            TryLoad(content, "top_a02");
            TryLoad(content, "top_a03");
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

        public IBlocks CreateSnowBlock(Vector2 position) => CreateBlock("snow", position);
        public IBlocks CreateCementBlock(Vector2 position) => CreateBlock("cement", position);
        public IBlocks CreateDirtBlock(Vector2 position) => CreateBlock("dirt", position);
        public IBlocks CreateGirderBlock(Vector2 position) => CreateBlock("girder", position);

        public IBlocks CreateBlock(string type, Vector2 position)
        {
            return BlockTextures.TryGetValue(type, out var tex)
                ? new Blocks(type, position, tex)
                : null;
        }
    }
}
