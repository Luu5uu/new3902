using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Blocks.Rooms
{
    public class RoomTwo
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;

        public RoomTwo(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void PlaceRoomTwoBlocks()
        {
            _mapBuilder.ClearBlocks();

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 1
            _mapBuilder.PlaceBlock("snow", 24, 1, 0);

            // ROW 2
            _mapBuilder.PlaceBlock("snow", 23, 2, 0);
            _mapBuilder.PlaceBlock("snow", 24, 2, 0);

            // ROW 3
            _mapBuilder.PlaceBlock("snow", 23, 3, 0);
            _mapBuilder.PlaceBlock("snow", 24, 3, 0);

            // ROW 4
            _mapBuilder.PlaceBlock("snow", 20, 4, 0);
            _mapBuilder.PlaceBlock("snow", 21, 4, 0);

            _mapBuilder.PlaceBlock("snow", 23, 4, 0);
            _mapBuilder.PlaceBlock("snow", 24, 4, 0);

            //ROW 5
            for (int x = 21; x <= 24; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 5, 0);
            }

            // snow cube
            for (int x = 21; x <= 23; x++)
            {
                for (int y = 6; y <= 8; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 0);
                }
            }

            //ROW 9
            _mapBuilder.PlaceBlock("snow", 38, 9, 0);

            //ROW 10
            _mapBuilder.PlaceBlock("snow", 38, 10, 0);
            _mapBuilder.PlaceBlock("snow", 39, 10, 0);

            //ROW 11
            _mapBuilder.PlaceBlock("snow", 38, 11, 0);
            _mapBuilder.PlaceBlock("snow", 39, 11, 0);

            //ROW 12
            _mapBuilder.PlaceBlock("snow", 38, 12, 0);
            _mapBuilder.PlaceBlock("snow", 39, 12, 0);

            //ROW 13
            _mapBuilder.PlaceBlock("snow", 38, 13, 0);
            _mapBuilder.PlaceBlock("snow", 39, 13, 0);

            //ROW 14
            _mapBuilder.PlaceBlock("snow", 38, 14, 0);
            _mapBuilder.PlaceBlock("snow", 39, 14, 0);

            //ROW 15
            for (int x = 20; x <= 24; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 15, 0);
            }

            // GIANT BLOCK IN BOTTOM RIGHT CORNER
            for (int x = 32; x <= 39; x++)
            {
                for (int y = 15; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 0);
                }
            }

            //ROW 16
            for (int x = 20; x <= 24; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 16, 0);
            }

            //ROW 17
            for (int x = 20; x <= 23; x++)
            {
                _mapBuilder.PlaceBlock("snow", x, 17, 0);
            }

            // ROW 18
            _mapBuilder.PlaceBlock("snow", 21, 18, 0);
            _mapBuilder.PlaceBlock("snow", 22, 18, 0);

            _mapBuilder.PlaceBlock("snow", 30, 18, 0);
            _mapBuilder.PlaceBlock("snow", 31, 18, 0);
            // the rest are in the block  loop above

            //ROW 19
            _mapBuilder.PlaceBlock("snow", 21, 19, 0);

            //ROW 20
            for (int x = 7; x <= 11; x++)
            {
                for (int y = 20; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 0);
                }
            }

            _mapBuilder.PlaceBlock("snow", 21, 20, 0);

            _mapBuilder.PlaceBlock("snow", 31, 20, 0);

            // ROW 21
            _mapBuilder.PlaceBlock("snow", 30, 21, 0);
            _mapBuilder.PlaceBlock("snow", 31, 21, 0);


            /* --------------- CEMENT BLOCKS ---------------*/

            // 12 IS PLACEHOLDER

            // ROW 0

            _mapBuilder.PlaceBlock("cement", 0, 0, 89);
            _mapBuilder.PlaceBlock("cement", 1, 0, 89);
            _mapBuilder.PlaceBlock("cement", 2, 0, 83);
            _mapBuilder.PlaceBlock("cement", 3, 0, 71);
            _mapBuilder.PlaceBlock("cement", 4, 0, 41);
            _mapBuilder.PlaceBlock("cement", 5, 0, 4);
            _mapBuilder.PlaceBlock("cement", 6, 0, 9);
            _mapBuilder.PlaceBlock("cement", 7, 0, 7);
            _mapBuilder.PlaceBlock("cement", 8, 0, 16);
            _mapBuilder.PlaceBlock("cement", 9, 0, 4);
            _mapBuilder.PlaceBlock("cement", 10, 0, 7);
            _mapBuilder.PlaceBlock("cement", 11, 0, 9);
            _mapBuilder.PlaceBlock("cement", 12, 0, 9);
            _mapBuilder.PlaceBlock("cement", 13, 0, 7);

            _mapBuilder.PlaceBlock("cement", 15, 0, 8);
            _mapBuilder.PlaceBlock("cement", 16, 0, 16);
            _mapBuilder.PlaceBlock("cement", 17, 0, 35);
            _mapBuilder.PlaceBlock("cement", 18, 0, 71);
            _mapBuilder.PlaceBlock("cement", 19, 0, 59);
            _mapBuilder.PlaceBlock("cement", 20, 0, 83);
            _mapBuilder.PlaceBlock("cement", 21, 0, 83);
            _mapBuilder.PlaceBlock("cement", 22, 0, 89);
            _mapBuilder.PlaceBlock("cement", 23, 0, 89);
            _mapBuilder.PlaceBlock("cement", 24, 0, 83);
            _mapBuilder.PlaceBlock("cement", 25, 0, 11);
            _mapBuilder.PlaceBlock("cement", 26, 0, 23);
            _mapBuilder.PlaceBlock("cement", 27, 0, 83);
            _mapBuilder.PlaceBlock("cement", 28, 0, 83);
            _mapBuilder.PlaceBlock("cement", 29, 0, 89);
            _mapBuilder.PlaceBlock("cement", 30, 0, 89);
            _mapBuilder.PlaceBlock("cement", 31, 0, 89);
            _mapBuilder.PlaceBlock("cement", 32, 0, 71);

            _mapBuilder.PlaceBlock("cement", 38, 0, 12);
            _mapBuilder.PlaceBlock("cement", 39, 0, 71);

            // ROW 1
            _mapBuilder.PlaceBlock("cement", 0, 1, 89);
            _mapBuilder.PlaceBlock("cement", 1, 1, 83);
            _mapBuilder.PlaceBlock("cement", 2, 1, 4);
            _mapBuilder.PlaceBlock("cement", 3, 1, 7);
            _mapBuilder.PlaceBlock("cement", 4, 1, 7);
            _mapBuilder.PlaceBlock("cement", 5, 1, 86);

            _mapBuilder.PlaceBlock("cement", 9, 1, 18);

            _mapBuilder.PlaceBlock("cement", 16, 1, 81);
            _mapBuilder.PlaceBlock("cement", 17, 1, 7);
            _mapBuilder.PlaceBlock("cement", 18, 1, 7);
            _mapBuilder.PlaceBlock("cement", 19, 1, 8);
            _mapBuilder.PlaceBlock("cement", 20, 1, 3);
            _mapBuilder.PlaceBlock("cement", 21, 1, 7);
            _mapBuilder.PlaceBlock("cement", 22, 1, 16);
            _mapBuilder.PlaceBlock("cement", 23, 1, 83);

            _mapBuilder.PlaceBlock("cement", 25, 1, 6);
            _mapBuilder.PlaceBlock("cement", 26, 1, 7);
            _mapBuilder.PlaceBlock("cement", 27, 1, 16);
            _mapBuilder.PlaceBlock("cement", 28, 1, 17);
            _mapBuilder.PlaceBlock("cement", 29, 1, 77);
            _mapBuilder.PlaceBlock("cement", 30, 1, 83);
            _mapBuilder.PlaceBlock("cement", 31, 1, 89);
            _mapBuilder.PlaceBlock("cement", 32, 1, 71);

            _mapBuilder.PlaceBlock("cement", 38, 1, 13);
            _mapBuilder.PlaceBlock("cement", 39, 1, 41);

            // ROW 2
            _mapBuilder.PlaceBlock("cement", 0, 2, 83);
            _mapBuilder.PlaceBlock("cement", 1, 2, 53);

            _mapBuilder.PlaceBlock("cement", 9, 2, 18);

            _mapBuilder.PlaceBlock("cement", 20, 2, 14);
            _mapBuilder.PlaceBlock("cement", 21, 2, 65);
            _mapBuilder.PlaceBlock("cement", 22, 2, 83);

            _mapBuilder.PlaceBlock("cement", 27, 2, 78);
            _mapBuilder.PlaceBlock("cement", 28, 2, 9);
            _mapBuilder.PlaceBlock("cement", 29, 2, 16);
            _mapBuilder.PlaceBlock("cement", 30, 2, 83);
            _mapBuilder.PlaceBlock("cement", 31, 2, 89);
            _mapBuilder.PlaceBlock("cement", 32, 2, 41);

            _mapBuilder.PlaceBlock("cement", 38, 2, 13);
            _mapBuilder.PlaceBlock("cement", 39, 2, 35);

            // ROW 3
            _mapBuilder.PlaceBlock("cement", 0, 3, 12);
            _mapBuilder.PlaceBlock("cement", 1, 3, 12);

            _mapBuilder.PlaceBlock("cement", 9, 3, 12);

            _mapBuilder.PlaceBlock("cement", 20, 3, 13);
            _mapBuilder.PlaceBlock("cement", 21, 3, 5);
            _mapBuilder.PlaceBlock("cement", 22, 3, 83);

            _mapBuilder.PlaceBlock("cement", 29, 3, 12);
            _mapBuilder.PlaceBlock("cement", 30, 3, 12);
            _mapBuilder.PlaceBlock("cement", 31, 3, 12);
            _mapBuilder.PlaceBlock("cement", 32, 3, 12);

            _mapBuilder.PlaceBlock("cement", 38, 3, 12);
            _mapBuilder.PlaceBlock("cement", 39, 3, 12);

            // ROW 4
            _mapBuilder.PlaceBlock("cement", 0, 4, 12);
            _mapBuilder.PlaceBlock("cement", 1, 4, 12);

            _mapBuilder.PlaceBlock("cement", 22, 4, 12);

            _mapBuilder.PlaceBlock("cement", 29, 4, 12);
            _mapBuilder.PlaceBlock("cement", 30, 4, 12);
            _mapBuilder.PlaceBlock("cement", 31, 4, 12);
            _mapBuilder.PlaceBlock("cement", 32, 4, 12);

            _mapBuilder.PlaceBlock("cement", 38, 4, 12);
            _mapBuilder.PlaceBlock("cement", 39, 4, 12);

            // ROW 5
            _mapBuilder.PlaceBlock("cement", 0, 5, 12);
            _mapBuilder.PlaceBlock("cement", 1, 5, 12);

            _mapBuilder.PlaceBlock("cement", 29, 5, 12);
            _mapBuilder.PlaceBlock("cement", 30, 5, 12);
            _mapBuilder.PlaceBlock("cement", 31, 5, 12);
            _mapBuilder.PlaceBlock("cement", 32, 5, 12);

            _mapBuilder.PlaceBlock("cement", 38, 5, 12);
            _mapBuilder.PlaceBlock("cement", 39, 5, 12);

            // ROW 6
            _mapBuilder.PlaceBlock("cement", 0, 6, 12);
            _mapBuilder.PlaceBlock("cement", 1, 6, 12);

            _mapBuilder.PlaceBlock("cement", 32, 6, 12);

            _mapBuilder.PlaceBlock("cement", 38, 6, 12);
            _mapBuilder.PlaceBlock("cement", 39, 6, 12);

            // ROW 7
            _mapBuilder.PlaceBlock("cement", 0, 7, 12);
            _mapBuilder.PlaceBlock("cement", 1, 7, 12);

            _mapBuilder.PlaceBlock("cement", 32, 7, 12);

            _mapBuilder.PlaceBlock("cement", 38, 7, 12);
            _mapBuilder.PlaceBlock("cement", 39, 7, 12);

            // ROW 8
            _mapBuilder.PlaceBlock("cement", 0, 8, 12);
            _mapBuilder.PlaceBlock("cement", 1, 8, 12);

            _mapBuilder.PlaceBlock("cement", 32, 8, 12);

            _mapBuilder.PlaceBlock("cement", 38, 8, 12);
            _mapBuilder.PlaceBlock("cement", 39, 8, 12);

            // ROW 9
            _mapBuilder.PlaceBlock("cement", 0, 9, 12);
            _mapBuilder.PlaceBlock("cement", 1, 9, 12);

            _mapBuilder.PlaceBlock("cement", 32, 9, 12);

            _mapBuilder.PlaceBlock("cement", 39, 9, 12);

            // ROW 10
            _mapBuilder.PlaceBlock("cement", 0, 10, 12);
            _mapBuilder.PlaceBlock("cement", 1, 10, 12);

            // ROW 11
            _mapBuilder.PlaceBlock("cement", 0, 11, 12);
            _mapBuilder.PlaceBlock("cement", 1, 11, 12);

            // SINGLE LINE X=0
            for (int y = 12; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("cement", 0, y, 12);
            }

            // ROW 18 (cont.)
            _mapBuilder.PlaceBlock("cement", 23, 18, 12);

            _mapBuilder.PlaceBlock("cement", 28, 18, 12);
            _mapBuilder.PlaceBlock("cement", 29, 18, 12);

            // ROW 19
            _mapBuilder.PlaceBlock("cement", 22, 19, 12);
            _mapBuilder.PlaceBlock("cement", 23, 19, 12);

            _mapBuilder.PlaceBlock("cement", 28, 19, 12);
            _mapBuilder.PlaceBlock("cement", 29, 19, 12);
            _mapBuilder.PlaceBlock("cement", 30, 19, 12);
            _mapBuilder.PlaceBlock("cement", 31, 19, 12);

            // ROW 20
            _mapBuilder.PlaceBlock("cement", 6, 20, 12);

            _mapBuilder.PlaceBlock("cement", 22, 20, 12);
            _mapBuilder.PlaceBlock("cement", 23, 20, 12);
            _mapBuilder.PlaceBlock("cement", 24, 20, 12);
            _mapBuilder.PlaceBlock("cement", 25, 20, 12);

            _mapBuilder.PlaceBlock("cement", 28, 20, 12);
            _mapBuilder.PlaceBlock("cement", 29, 20, 12);
            _mapBuilder.PlaceBlock("cement", 30, 20, 12);

            // ROW 21
            _mapBuilder.PlaceBlock("cement", 6, 21, 12);

            _mapBuilder.PlaceBlock("cement", 21, 21, 12);
            _mapBuilder.PlaceBlock("cement", 22, 21, 12);
            _mapBuilder.PlaceBlock("cement", 23, 21, 12);
            _mapBuilder.PlaceBlock("cement", 24, 21, 12);
            _mapBuilder.PlaceBlock("cement", 25, 21, 12);
            _mapBuilder.PlaceBlock("cement", 26, 21, 12);
            _mapBuilder.PlaceBlock("cement", 27, 21, 12);
            _mapBuilder.PlaceBlock("cement", 28, 21, 12);
            _mapBuilder.PlaceBlock("cement", 29, 21, 12);

            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER, USING STRAIGHT LINES FOR NOW

            // ROW 0
            for (int x = 0; x <= 4; x++)
            {
                _mapBuilder.PlaceBlock("girder", 14, x, 4);
            }

            for (int x = 0; x < 11; x++)
            {
                _mapBuilder.PlaceBlock("girder", 33, x, 4);
            }

            // ROW 1
            for (int x = 0; x < 7; x++)
            {
                _mapBuilder.PlaceBlock("girder", 8, x, 4);
            }

            // ROW 2
            for (int x = 2; x < 7; x++)
            {
                _mapBuilder.PlaceBlock("girder", 2, x, 4);
            }

            // HORIZONTALS (3, 5, & 7)
            for (int y = 3; y <= 7; y += 2)
            {
                for (int x = 2; x <= 8; x++)
                {
                    _mapBuilder.PlaceBlock("girder", x, y, 4);
                }
            }

            // ROW 18
            for (int y = 18; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 14, y, 4);
            }
            for (int y = 18; y <= 21; y++)
            {
                _mapBuilder.PlaceBlock("girder", 15, y, 4);
            }
             
            // ROW 19
            for (int x = 12; x <= 15; x++)
            {
                _mapBuilder.PlaceBlock("girder", x, 20, 4);
            }

            /* --------------- HAZARD BLOCKS ---------------*/

            // SPIKES
            for (int x = 20; x <= 24; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 14, 0);
            }
            _mapBuilder.PlaceBlock("upSpike", 24, 19, 0);
            _mapBuilder.PlaceBlock("upSpike", 25, 19, 0);

            _mapBuilder.PlaceBlock("upSpike", 26, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 27, 20, 0);

            // SPRING
            _mapBuilder.PlaceBlock("spring", 17, 14, 0);

        }
    }
}