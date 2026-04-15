using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomOne
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;


        public RoomOne(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomOneBlocks()
        {
            _mapBuilder.ClearBlocks();

            // csv  format:
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomOne.csv");
            _roomLoader.LoadRoom(filePath);

        }
    }
}
