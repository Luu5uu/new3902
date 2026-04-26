using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomFive
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;


        public RoomFive(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomFiveBlocks()
        {
            _mapBuilder.ClearBlocks();

            // csv  format:
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomFive.csv");
            _roomLoader.LoadRoom(filePath);

            _mapBuilder.PlaceMoveBlock(
                position: new Vector2(250f, 200f),
                distance: 200f,
                speed: 25f,
                angleDegrees: 5f);

        }
    }
}
