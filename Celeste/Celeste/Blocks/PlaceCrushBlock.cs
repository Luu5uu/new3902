using Microsoft.Xna.Framework;
using Celeste.Animation;

namespace Celeste.Blocks
{
    public class PlaceCrushBlock
    {
        private readonly AnimationCatalog _catalog;
        private readonly float _blockSize;

        public PlaceCrushBlock(AnimationCatalog catalog, float blockSize = 8f * 2.5f)
        {
            _catalog = catalog;
            _blockSize = blockSize;
        }

        public CrushBlock CreateAtGrid(int gridX, int gridY, float scale = 2.5f)
        {
            Vector2 position = new Vector2(gridX * _blockSize, gridY * _blockSize);
            return new CrushBlock(position, _catalog, scale);
        }
    }
}
