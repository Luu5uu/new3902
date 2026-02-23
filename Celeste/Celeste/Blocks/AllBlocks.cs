using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;

namespace Celeste.Blocks
{
    /// <summary>
    /// Gallery view that displays all block types for debugging/size reference.
    /// </summary>
    public class AllBlocks : IBlocks
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public string Type => "gallery";
        public float Scale { get; set; } = 0.75f;

        private readonly List<IBlocks> _staticBlocks;
        private readonly List<IBlocks> _movingBlocks;

        private const int Columns = 10;
        private const int StartX = 15;
        private const int StartY = 165;
        private const int SpaceX = 100;
        private const int SpaceY = 250;

        public AllBlocks(List<IBlocks> blocks, AnimationCatalog catalog)
        {
            _staticBlocks = blocks.Where(b => !(b is Spring || b is CrushBlock || b is MoveBlock)).ToList();
            _movingBlocks = blocks.Where(b => b is Spring || b is CrushBlock || b is MoveBlock).ToList();
        }

        public void Update(GameTime gameTime)
        {
            foreach (var block in _movingBlocks)
            {
                if (block is Spring spring)
                    spring.Update(gameTime);
                else if (block is CrushBlock crushBlock)
                    crushBlock.Update(gameTime);
                else if (block is MoveBlock moveBlock)
                    moveBlock.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch) { }

        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            int width = graphicsDevice.Viewport.Width;
            int x = StartX;
            int y = StartY;
            int columnCurrent = 0;

            for (int i = 0; i < _staticBlocks.Count; i++)
            {
                if (columnCurrent >= Columns || x + SpaceX > width)
                {
                    x = StartX;
                    y += SpaceY;
                    columnCurrent = 0;
                }
                var gridPosition = new Vector2(x, y);
                var originalPosition = _staticBlocks[i].Position;
                _staticBlocks[i].Position = gridPosition;
                _staticBlocks[i].Draw(spriteBatch);
                _staticBlocks[i].Position = originalPosition;
                x += SpaceX;
                columnCurrent++;
            }

            for (int i = 0; i < _movingBlocks.Count; i++)
            {
                var gridPosition = new Vector2(x, y);
                var originalPosition = _movingBlocks[i].Position;
                _movingBlocks[i].Position = gridPosition;
                _movingBlocks[i].Draw(spriteBatch);
                _movingBlocks[i].Position = originalPosition;
                x += SpaceX;
            }
        }
    }
}
