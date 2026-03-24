using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Blocks.Rooms
{
    public class RoomFour
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;


        public RoomFour(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void PlaceRoomFourBlocks()
        {
            _mapBuilder.ClearBlocks();

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)


            /* --------------- SNOW BLOCKS ---------------*/

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 0
            for (int x = 0; x <= 21; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 0, 1);
            }
            for (int x = 0; x <= 5; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 1, 1);
            }
            for (int x = 26; x <= 39; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 2; y <= 4; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 5; y <= 12; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int y = 13; y <= 19; y++)
            {
                _mapBuilder.PlaceBlock("snow", 0, y, 1);
            }
            for (int x = 0; x <= 2; x++)
            {
                for (int y = 20; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 9; x <= 13; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 21, 1);
            }
            for (int x = 16; x <= 26; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 21, 1);
            }
            for (int x = 36; x <= 39; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 2, 1);
            }
            for (int x = 38; x <= 39; x++)
            {
                for (int y = 3; y <= 4; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int y = 5; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("snow", 39, y, 1);
            }

            //middle platform
            for (int x = 20; x <= 24; x++)
            {
                for (int y = 7; y <= 11; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 23; x <= 24; x++)
            {
                for (int y = 12; y <= 14; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            _mapBuilder.PlaceBlock("snow", 18, 7, 1);
            _mapBuilder.PlaceBlock("snow", 19, 7, 1);
            _mapBuilder.PlaceBlock("snow", 19, 8, 1);
            _mapBuilder.PlaceBlock("snow", 23, 5, 1);
            _mapBuilder.PlaceBlock("snow", 24, 5, 1);
            _mapBuilder.PlaceBlock("snow", 24, 15, 1);
            _mapBuilder.PlaceBlock("snow", 24, 16, 1);



            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER

            //  HORIZONTAL
            for (int x = 18; x <= 23; x++)
            {
                _mapBuilder.PlaceBlock("girder", x, 6, 4);
            }

            // VERTICAL
            for (int y = 5; y <= 12; y++)
            {
                _mapBuilder.PlaceBlock("girder", 25, y, 4);
            }
            for (int y = 19; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 3, y, 4);
            }

            _mapBuilder.PlaceBlock("girder", 8, 21, 4);

            for (int y = 19; y <= 21; y++)
            {
                for (int x = 14; x <= 15; x++)
                {
                    _mapBuilder.PlaceBlock("girder", x, y, 4);
                }
            }


            /* --------------- HAZARDS BLOCKS ---------------*/

            // PUT DISAPPEARING PLATFORMS HERE

        }
    }
}