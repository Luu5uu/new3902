using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;
using Celeste.Animation;

namespace Celeste.Blocks
{
    public class MapBuilder
    {
        public List<IBlocks> _blocks = new List<IBlocks>();
        public List<IHazard> _hazards = new List<IHazard>();
        private BlockFactory _blockFactory;
        private readonly AnimationCatalog _catalog;
        private readonly PlaceCrushBlock _placeCrushBlock;
        // size * scale
        private float blockSize = 8 * 2.5f;
        private IBlocks[,] grid;
        private IHazard[,] hazard_grid;
        private int w, h;

        public int Width => w;
        public int Height => h;

        public MapBuilder(BlockFactory blockFactory, AnimationCatalog catalog, int width = 48, int height = 120)
        {
            this._blockFactory = blockFactory;
            _catalog = catalog;
            _placeCrushBlock = new PlaceCrushBlock(catalog, blockSize);
            this.w = width;
            this.h = height;
            grid = new IBlocks[width, height];
            hazard_grid = new IHazard[width, height];
        }
        public void PlaceBlock(string type, int gridX, int gridY, int frameNum = 0)
        {
            if (gridX < 0 || gridX >= w || gridY < 0 || gridY >= h) { return; }

            Vector2 position = new Vector2(gridX * blockSize, gridY * blockSize);
            if (IsHazardType(type))
            {
                IHazard hazard = _blockFactory.CreateHazard(type, position);
                if (hazard != null)
                {
                     RemoveHazard(gridX, gridY);
                    _hazards.Add(hazard);
                    hazard_grid[gridX, gridY] = hazard;
                }
            }
            else
            {
                IBlocks block = type switch
                {
                    "crushBlock" => _placeCrushBlock.CreateAtGrid(gridX, gridY),
                    "spring" => new Spring(position, _catalog),
                    _ => _blockFactory.CreateBlock(type, position, frameNum),
                };
                if (block != null)
                {
                    RemoveBlock(gridX, gridY);
                    _blocks.Add(block);
                    grid[gridX, gridY] = block;
                }
            }
        }

        public void PlaceCrushBlock(int gridX, int gridY, float scale = 2.5f)
        {
            if (gridX < 0 || gridX >= w || gridY < 0 || gridY >= h)
            {
                return;
            }

            IBlocks block = _placeCrushBlock.CreateAtGrid(gridX, gridY, scale);
            RemoveBlock(gridX, gridY);
            _blocks.Add(block);
            grid[gridX, gridY] = block;
        }

        private static bool IsHazardType(string type)
        {
            return type == "upSpike"
                || type == "downSpike"
                || type == "leftSpike"
                || type == "rightSpike";
        }

        public void RemoveBlock(int gridX, int gridY)
        {
            if (grid[gridX, gridY] != null)
            {
                _blocks.Remove(grid[gridX, gridY]);
                grid[gridX, gridY] = null;
            }
        }

        public void RemoveHazard(int gridX, int gridY)
        {
            if (hazard_grid[gridX, gridY] != null)
            {
                _hazards.Remove(hazard_grid[gridX, gridY]);
                hazard_grid[gridX, gridY] = null;
            }
        }

        public void ClearBlocks()
        {
            _blocks.Clear();
            _hazards.Clear();
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    grid[x, y] = null;
                    hazard_grid[x, y] = null;
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

        public void Update(GameTime gameTime)
        {
            foreach (var block in _blocks)
            {
                if (block is Spring spring)
                {
                    spring.Update(gameTime);
                }
                else if (block is MoveBlock moveBlock)
                {
                    moveBlock.Update(gameTime);
                }
                else if (block is CrushBlock crushBlock)
                {
                    crushBlock.Update(gameTime);
                }
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
