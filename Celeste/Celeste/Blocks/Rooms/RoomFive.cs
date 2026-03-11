using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Blocks.Rooms
{
    public class RoomFive
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;


        public RoomFive(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void PlaceRoomFiveBlocks()
        {
            _mapBuilder.ClearBlocks();

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            /* --------------- SNOW BLOCKS ---------------*/
            // ROW 1
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 0; x <= 4; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 6; x <= 11; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 2, 1);
            }
            for (int y = 5; y <= 13; y++)
            {
                _mapBuilder.PlaceBlock("snow", 2, y, 1);
            }
            for (int y = 0; y <= 3; y++)
            {
                _mapBuilder.PlaceBlock("snow", 5, y, 1);
            }
            for (int x = 6; x <= 11; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 12; x <= 19; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 18; x <= 19; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 8; x <= 10; x++)
            {
                for (int y = 20; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 26; x <= 34; x++)
            {
                for (int y = 0; y <= 2; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 27; x <= 31; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 3, 1);
            }
            for (int x = 28; x <= 31; x++)
            {
                for (int y = 4; y <= 5; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            _mapBuilder.PlaceBlock("snow", 28, 6, 1);
            _mapBuilder.PlaceBlock("snow", 30, 6, 1);
            _mapBuilder.PlaceBlock("snow", 31, 6, 1);
            _mapBuilder.PlaceBlock("snow", 28, 7, 1);

            _mapBuilder.PlaceBlock("snow", 35, 0, 1);
            _mapBuilder.PlaceBlock("snow", 35, 1, 1);

            for (int x = 36; x <= 39; x++)
            {
                for (int y = 0; y <= 4; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int x = 38; x <= 39; x++)
            {
                for (int y = 0; y <= 12; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            for (int y = 13; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("snow", 39, y, 1);
            }


            /* --------------- DIRT BLOCKS ---------------*/

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 0
            _mapBuilder.PlaceBlock("dirt", 8, 10, 0);
            _mapBuilder.PlaceBlock("dirt", 8, 11, 0);
            _mapBuilder.PlaceBlock("dirt", 10, 15, 0);


            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER

            //  HORIZONTAL
            for (int x = 10; x <= 11; x++)
            {
                _mapBuilder.PlaceBlock("girder", x, 14, 4);
            }
            for (int x = 8; x <= 9; x++)
            {
                _mapBuilder.PlaceBlock("girder", x, 9, 4);
            }

            // VERTICAL
            for (int y = 18; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 2, y, 4);
            }
            for (int y = 9; y <= 15; y++)
            {
                _mapBuilder.PlaceBlock("girder", 9, y, 4);
            }
            for (int y = 20; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 7, y, 4);
            }
            for (int y = 0; y <= 7; y++)
            {
                _mapBuilder.PlaceBlock("girder", 20, y, 4);
            }
            for (int y = 0; y <= 2; y++)
            {
                _mapBuilder.PlaceBlock("girder", 25, y, 4);
            }
            for (int y = 6; y <= 11; y++)
            {
                _mapBuilder.PlaceBlock("girder", 29, y, 4);
            }


            /* --------------- HAZARDS BLOCKS ---------------*/

            // SPIKES
            for (int y = 4; y <= 7; y++)
            {
                _mapBuilder.PlaceBlock("leftSpike", 27, y, 4);
            }
            for (int y = 12; y <= 15; y++)
            {
                _mapBuilder.PlaceBlock("leftSpike", 8, y, 4);
            }
            for (int y = 14; y <= 17; y++)
            {
                _mapBuilder.PlaceBlock("rightSpike", 2, y, 4);
            }

            for (int x = 10; x <= 11; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 13, 4);
            }

        }
    }
}