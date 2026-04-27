using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomTwo
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;

        public RoomTwo(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomTwoBlocks()
        {
            _mapBuilder.ClearBlocks();
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomTwo.csv");
            _roomLoader.LoadRoom(filePath);


            _mapBuilder.PlaceBlock("wood", 1, 20, 0);
            _mapBuilder.PlaceBlock("wood", 2, 20, 1);
            _mapBuilder.PlaceBlock("wood", 3, 20, 1);
            _mapBuilder.PlaceBlock("wood", 4, 20, 1);
            _mapBuilder.PlaceBlock("wood", 5, 20, 2);
        }
    }
}
