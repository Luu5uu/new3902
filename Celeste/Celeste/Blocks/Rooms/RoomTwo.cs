using System;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomTwo
    {
        private readonly MapBuilder _mapBuilder;
        private readonly RoomLoader _roomLoader;

        public RoomTwo(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _roomLoader = new RoomLoader(_mapBuilder, blockFactory);
        }

        public void PlaceRoomTwoBlocks()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomTwo.csv");
            _roomLoader.LoadRoom(filePath);
        }
    }
}
