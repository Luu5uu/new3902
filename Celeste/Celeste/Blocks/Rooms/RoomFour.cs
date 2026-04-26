using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomFour
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;


        public RoomFour(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomFourBlocks()
        {

            _mapBuilder.ClearBlocks();

            // csv  format:
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomFour.csv");
            _roomLoader.LoadRoom(filePath);

            _mapBuilder.PlaceCrushBlock(30, 7);
            _mapBuilder.PlaceCrushBlock(28, 10);
            _mapBuilder.PlaceCrushBlock(33, 13);
            _mapBuilder.PlaceCrushBlock(29, 16);

        }
    }
}
