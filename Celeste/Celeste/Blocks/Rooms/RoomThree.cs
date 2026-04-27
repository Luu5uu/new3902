using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomThree
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;


        public RoomThree(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomThreeBlocks()
        {

            _mapBuilder.ClearBlocks();

            // csv  format:
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomThree.csv");
            _roomLoader.LoadRoom(filePath);

            _mapBuilder.PlaceBlock("wood", 2, 21, 0);
            _mapBuilder.PlaceBlock("wood", 3, 21, 1);
            _mapBuilder.PlaceBlock("wood", 4, 21, 1);
            _mapBuilder.PlaceBlock("wood", 5, 21, 2);

        }
    }
}
