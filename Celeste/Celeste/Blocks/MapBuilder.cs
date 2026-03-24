using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Celeste.Blocks
{
    public class MapBuilder
    {
        public List<IBlocks> _blocks = new List<IBlocks>();
        public List<IHazard> _hazards = new List<IHazard>();
        private BlockFactory _blockFactory;
        // size * scale
        private float blockSize = 8 * 2.5f;
        private IBlocks[,] grid;
        private int w, h;

        public int Width => w;
        public int Height => h;

        public MapBuilder(BlockFactory blockFactory, int width = 48, int height = 120)
        {
            this._blockFactory = blockFactory;
            this.w = width;
            this.h = height;
            grid = new IBlocks[width, height];
        }
        public void PlaceBlock(string type, int gridX, int gridY, int frameNum = 0)
        {
            if (gridX < 0 || gridX >= w || gridY < 0 || gridY >= h) { return; }

            Vector2 position = new Vector2(gridX * blockSize, gridY * blockSize);
            if (type == "upSpike")
            {
                IHazard hazard = _blockFactory.CreateHazard(type, position);
                if (hazard != null)
                {
                    RemoveBlock(gridX, gridY);
                    _hazards.Add(hazard);
                }
            }
            else
            {
                IBlocks block = _blockFactory.CreateBlock(type, position, frameNum);
                if (block != null)
                {
                    RemoveBlock(gridX, gridY);
                    _blocks.Add(block);
                    grid[gridX, gridY] = block;
                }
            }
        }

        public void RemoveBlock(int gridX, int gridY)
        {
            if (grid[gridX, gridY] != null)
            {
                _blocks.Remove(grid[gridX, gridY]);
                grid[gridX, gridY] = null;
            }
        }

        public void ClearBlocks()
        {
            _blocks.Clear();
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    grid[x, y] = null;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var block in _blocks)
            {
                block.Draw(spriteBatch);
            }

            foreach (var hazard in _hazards)
            {
                hazard.Draw(spriteBatch);
            }
        }

        public IBlocks getBlock(int gridX, int gridY)
        {
            if ((gridX >= 0 && gridX < w) && (gridY >= 0 && gridY < h))
            {
                return grid[gridX, gridY];
            }
            return null;
        }
    }
}