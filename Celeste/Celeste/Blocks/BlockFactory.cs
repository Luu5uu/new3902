using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Celeste.Blocks
{
    public class BlockFactory
    {
        //use singleton method
        private static BlockFactory instance = new BlockFactory();
        public static BlockFactory Instance
        {
            get { return instance; }
        }
        public Dictionary<string, Texture2D> blockTextures;
        private BlockFactory()
        {
            blockTextures = new Dictionary<string, Texture2D>();
        }

        public void LoadAllTextures(ContentManager content)
        {
            // ADD EACH INDIVIDUALLY
            // mass texture blocks to be split
            blockTextures["snow"] = content.Load<Texture2D>("snow");
            blockTextures["cement"] = content.Load<Texture2D>("cement");
            blockTextures["dirt"] = content.Load<Texture2D>("dirt");
            blockTextures["girder"] = content.Load<Texture2D>("girder");
            //mini snows
            blockTextures["4"] = content.Load<Texture2D>("4");
            blockTextures["7"] = content.Load<Texture2D>("7");
            //spikes
            blockTextures["spikeUp"] = content.Load<Texture2D>("spikeUp");
            //blockTextures["spikeDown"] = content.Load<Texture2D>("spikeDown");
            //blockTextures["spikeLeft"] = content.Load<Texture2D>("spikeLeft");
            //blockTextures["spikeRight"] = content.Load<Texture2D>("spikeRight");
            // grass blocks (change name)
            blockTextures["top_a00"] = content.Load<Texture2D>("top_a00");
            blockTextures["top_a01"] = content.Load<Texture2D>("top_a01");
            blockTextures["top_a02"] = content.Load<Texture2D>("top_a02");
            blockTextures["top_a03"] = content.Load<Texture2D>("top_a03");

        }
        public IBlocks CreateSnowBlock(Vector2 position)
        {
            return new Blocks("snow", position, blockTextures["snow"]);
        }
        public IBlocks CreateCementBlock(Vector2 position)
        {
            return new Blocks("cement", position, blockTextures["cement"]);
        }
        public IBlocks CreateDirtBlock(Vector2 position)
        {
            return new Blocks("dirt", position, blockTextures["dirt"]);
        }
        public IBlocks CreateGirderBlock(Vector2 position)
        {
            return new Blocks("girder", position, blockTextures["girder"]);
        }

        // just for abstraction
        public IBlocks CreateAnyBlock(string type, Vector2 position)
        {
            return new Blocks(type, position, blockTextures[type]);

        }
    }


}



