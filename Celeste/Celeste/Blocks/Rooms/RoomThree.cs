using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Blocks.Rooms
{
    public class RoomThree
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;


        public RoomThree(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void PlaceRoomThreeBlocks()
        {

            _mapBuilder.ClearBlocks();

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 0
            _mapBuilder.PlaceBlock("snow", 27, 0, 41);
            _mapBuilder.PlaceBlock("snow", 28, 0, 53);
            _mapBuilder.PlaceBlock("snow", 29, 0, 47);
            _mapBuilder.PlaceBlock("snow", 30, 0, 35);
            _mapBuilder.PlaceBlock("snow", 31, 0, 11);

            _mapBuilder.PlaceBlock("snow", 38, 0, 17);
            _mapBuilder.PlaceBlock("snow", 39, 0, 77);

            // ROW 1
            _mapBuilder.PlaceBlock("snow", 27, 1, 7);
            _mapBuilder.PlaceBlock("snow", 28, 1, 9);
            _mapBuilder.PlaceBlock("snow", 29, 1, 7);
            _mapBuilder.PlaceBlock("snow", 30, 1, 8);
            _mapBuilder.PlaceBlock("snow", 31, 1, 16);

            _mapBuilder.PlaceBlock("snow", 38, 1, 1);
            _mapBuilder.PlaceBlock("snow", 39, 1, 1);


            // ROW 3
            _mapBuilder.PlaceBlock("snow", 31, 2, 78);

            // VERTICAL SNOW ALONG RIGHT WALL (starting  y =  2, x = 38)

            _mapBuilder.PlaceBlock("snow", 38, 2, 17);
            _mapBuilder.PlaceBlock("snow", 38, 3, 29);
            _mapBuilder.PlaceBlock("snow", 38, 4, 35);
            _mapBuilder.PlaceBlock("snow", 38, 5, 53);
            _mapBuilder.PlaceBlock("snow", 38, 6, 16);
            _mapBuilder.PlaceBlock("snow", 38, 7, 12);
            _mapBuilder.PlaceBlock("snow", 38, 8, 13);
            _mapBuilder.PlaceBlock("snow", 38, 9, 1);
            _mapBuilder.PlaceBlock("snow", 38, 10, 12);
            _mapBuilder.PlaceBlock("snow", 38, 11, 1);
            _mapBuilder.PlaceBlock("snow", 38, 912, 1);

            // x = 39 wall
            _mapBuilder.PlaceBlock("snow", 39, 2, 1);
            _mapBuilder.PlaceBlock("snow", 39, 3, 1);
            _mapBuilder.PlaceBlock("snow", 39, 4, 1);
            _mapBuilder.PlaceBlock("snow", 39, 5, 1);
            _mapBuilder.PlaceBlock("snow", 39, 6, 1);
            _mapBuilder.PlaceBlock("snow", 39, 7, 1);
            _mapBuilder.PlaceBlock("snow", 39, 8, 1);
            _mapBuilder.PlaceBlock("snow", 39, 9, 1);
            _mapBuilder.PlaceBlock("snow", 39, 10, 1);
            _mapBuilder.PlaceBlock("snow", 39, 11, 1);
            _mapBuilder.PlaceBlock("snow", 39, 12, 1);
            _mapBuilder.PlaceBlock("snow", 39, 13, 1);
            _mapBuilder.PlaceBlock("snow", 39, 14, 1);
            _mapBuilder.PlaceBlock("snow", 39, 15, 1);
            _mapBuilder.PlaceBlock("snow", 39, 16, 1);
            _mapBuilder.PlaceBlock("snow", 39, 17, 1);
            _mapBuilder.PlaceBlock("snow", 39, 18, 1);


            //ROW 8 BLOCKS
            for (int y = 9; y <= 11; y++)
            {
                for (int x = 8; x <= 9; x++)
                {
                    _mapBuilder.PlaceBlock("snow", y, x, 1);
                }
            }

            for (int x = 31; x <= 33; x++)
            {
                for (int y = 8; y <= 10; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            _mapBuilder.PlaceBlock("snow", 32, 14, 1);

            for (int y = 11; y <= 13; y++)
            {
                for (int x = 31; x <= 32; x++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }

            for (int x = 17; x <= 19; x++)
            {
                for (int y = 14; y <= 16; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }

            for (int x = 18; x <= 19; x++)
            {
                for (int y = 17; y <= 18; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 1);
                }
            }
            _mapBuilder.PlaceBlock("snow", 19, 19, 1);



            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER

            // VERTICAL 
            for (int y = 1; y <= 3; y++)
            {
                _mapBuilder.PlaceBlock("girder", 5, y, 4);
            }
            for (int y = 1; y <= 4; y++)
            {
                _mapBuilder.PlaceBlock("girder", 26, y, 4);
            }
            for (int y = 0; y <= 3; y++)
            {
                _mapBuilder.PlaceBlock("girder", 32, y, 4);
            }
            for (int y = 0; y <= 6; y++)
            {
                _mapBuilder.PlaceBlock("girder", 37, y, 4);
            }
            for (int y = 6; y <= 19; y++)
            {
                _mapBuilder.PlaceBlock("girder", 0, y, 4);
            }
            for (int y = 10; y <= 12; y++)
            {
                _mapBuilder.PlaceBlock("girder", 11, y, 4);
            }
            for (int y = 12; y <= 16; y++)
            {
                _mapBuilder.PlaceBlock("girder", 16, y, 4);
            }
            for (int y = 20; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 23, y, 4);
            }
            for (int y = 19; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 1, y, 4);
            }


            /* --------------- CEMENT BLOCKS ---------------*/

            // 12 IS PLACEHOLDER

            // ROW 0
            for (int x = 0; x <= 4; x++)
            {
                for (int y = 0; y <= 3; y++)
                {
                    _mapBuilder.PlaceBlock("cement", x, y, 12);
                }
            }

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 4; y <= 5; y++)
                {
                    _mapBuilder.PlaceBlock("cement", x, y, 12);
                }
            }

            _mapBuilder.PlaceBlock("cement", 5, 0, 12);

            for (int x = 6; x <= 25; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    _mapBuilder.PlaceBlock("cement", x, y, 12);
                }
            }

            for (int x = 15; x <= 25; x++)
            {
                _mapBuilder.PlaceBlock("cement", x, 2, 12);
            }
            _mapBuilder.PlaceBlock("cement", 26, 0, 12);

            _mapBuilder.PlaceBlock("cement", 15, 3, 12);
            _mapBuilder.PlaceBlock("cement", 16, 3, 12);

            _mapBuilder.PlaceBlock("cement", 0, 20, 12);
            _mapBuilder.PlaceBlock("cement", 0, 21, 12);

            for (int x = 6; x <= 8; x++)
            {
                _mapBuilder.PlaceBlock("cement", x, 21, 12);
            }

            _mapBuilder.PlaceBlock("cement", 26, 17, 12);
            _mapBuilder.PlaceBlock("cement", 27, 17, 12);
            _mapBuilder.PlaceBlock("cement", 28, 17, 12);
            for (int x = 26; x <= 33; x++)
            {
                for (int y = 18; y <= 19; y++)
                {
                    _mapBuilder.PlaceBlock("cement", x, y, 12);
                }
            }
            _mapBuilder.PlaceBlock("cement", 38, 19, 12);
            _mapBuilder.PlaceBlock("cement", 39, 19, 12);
            for (int x = 24; x <= 39; x++)
            {
                for (int y = 20; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("cement", x, y, 12);
                }
            }


            /* --------------- HAZARDS BLOCKS ---------------*/

            // SPIKES
            _mapBuilder.PlaceBlock("upSpike", 38, 18, 0);

            for (int x = 34; x <= 37; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 19, 12);
            }
            for (int x = 29; x <= 33; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 17, 12);
            }
            for (int x = 26; x <= 28; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 16, 12);
            }
            for (int x = 23; x <= 25; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 19, 12);
            }

        }
    }
}