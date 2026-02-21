using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Celeste.Animation;
using System.Collections.Generic;
using System.Linq;

namespace Celeste.Blocks
{
    /// <summary>
    /// for debuggin  and getting correct sizes of blocks
    /// </summary>
    public class AllBlocks : IBlocks
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public string Type => "gallery";
        public float Scale { get; set; } = 0.75f;

        private List<IBlocks> _allBlocks;
        private List<IBlocks> _staticBlocks;
        private List<IBlocks> _movingBlocks;
        private AnimationCatalog _catalog;

        // layout scheme (and spacing)
        private int _columns = 10;
        private int _startX = 15;
        private int _startY = 165;
        private int _spaceX = 100;
        private int _spaceY = 250;

        public AllBlocks(List<IBlocks> blocks, AnimationCatalog catalog)
        {
            _allBlocks = blocks;
            _catalog = catalog;

            // exclude the moving ones
            _staticBlocks = blocks.Where(b => !(b is Spring || b is CrushBlock || b is MoveBlock)).ToList();
            _movingBlocks = blocks.Where(b => b is Spring || b is CrushBlock || b is MoveBlock).ToList();
        }
        public void Update(GameTime gameTime)
        {
            // update ALL
            foreach (var block in _movingBlocks)
            {
                if (block is Spring spring)
                {
                    spring.Update(gameTime);
                }
                else if (block is CrushBlock crushBlock)
                {
                    crushBlock.Update(gameTime);
                }
                else if (block is MoveBlock moveBlock)
                {
                    moveBlock.Update(gameTime);
                }
            }
        }
        // to make interface happy
        public void Draw(SpriteBatch spriteBatch) { }
        public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            //FIX FOR WRAPPING ISSUE
            int width = graphicsDevice.Viewport.Width;


            int x = _startX;
            int y = _startY;
            int columnCurrent = 0;

            // STATIC BLOCKS
            for (int i = 0; i < _staticBlocks.Count; i++)
            {
                // check if wrapping is needed
                if (columnCurrent >= _columns || x + _spaceX > width)
                {

                    x = _startX;
                    y += _spaceY;
                    columnCurrent = 0;

                }

                Vector2 gridPosition = new Vector2(x, y);


                Vector2 originalPosition = _staticBlocks[i].Position;
                _staticBlocks[i].Position = gridPosition;
                _staticBlocks[i].Draw(spriteBatch);
                _staticBlocks[i].Position = originalPosition;

                // shifting for the next block
                x += _spaceX;
                columnCurrent++;
            }

            for (int i = 0; i < _movingBlocks.Count; i++)
            {

                Vector2 gridPosition = new Vector2(x, y);
                Vector2 originalPosition = _movingBlocks[i].Position;

                _movingBlocks[i].Position = gridPosition;
                _movingBlocks[i].Draw(spriteBatch);
                _movingBlocks[i].Position = originalPosition;

                x += _spaceX;
            }
        }
    }
}