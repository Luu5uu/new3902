using System;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomThree
    {
        private readonly MapBuilder _mapBuilder;
        private readonly RoomLoader _roomLoader;

        public RoomThree(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _roomLoader = new RoomLoader(_mapBuilder, blockFactory);
        }

        public void PlaceRoomThreeBlocks()
        {
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomThree.csv");
            _roomLoader.LoadRoom(filePath);
        }
    }
}
