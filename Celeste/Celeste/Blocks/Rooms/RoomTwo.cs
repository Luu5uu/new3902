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
            _mapBuilder.PlaceBlock("snow", 24, 1, 4);


            // ROW 2
            _mapBuilder.PlaceBlock("snow", 23, 2, 65);
            _mapBuilder.PlaceBlock("snow", 24, 2, 19);

            // ROW 3
            _mapBuilder.PlaceBlock("snow", 23, 3, 11);
            _mapBuilder.PlaceBlock("snow", 24, 3, 19);

            // ROW 4
            _mapBuilder.PlaceBlock("snow", 20, 4, 80);
            _mapBuilder.PlaceBlock("snow", 21, 4, 16);

            _mapBuilder.PlaceBlock("snow", 23, 4, 71);
            _mapBuilder.PlaceBlock("snow", 24, 4, 21);

            //ROW 5
            _mapBuilder.PlaceBlock("snow", 21, 5, 15);
            _mapBuilder.PlaceBlock("snow", 22, 5, 17);
            _mapBuilder.PlaceBlock("snow", 23, 5, 4);
            _mapBuilder.PlaceBlock("snow", 24, 5, 86);

            //ROW 6
            _mapBuilder.PlaceBlock("snow", 21, 6, 15);
            _mapBuilder.PlaceBlock("snow", 22, 6, 35);
            _mapBuilder.PlaceBlock("snow", 23, 6, 19);

            //ROW 7
            _mapBuilder.PlaceBlock("snow", 21, 7, 76);
            _mapBuilder.PlaceBlock("snow", 22, 7, 41);
            _mapBuilder.PlaceBlock("snow", 23, 7, 18);

            //ROW 8
            _mapBuilder.PlaceBlock("snow", 21, 8, 78);
            _mapBuilder.PlaceBlock("snow", 22, 8, 8);
            _mapBuilder.PlaceBlock("snow", 23, 8, 86);

            //ROW 9
            _mapBuilder.PlaceBlock("snow", 38, 9, 14);

            //ROW 10
            _mapBuilder.PlaceBlock("snow", 38, 10, 14);
            _mapBuilder.PlaceBlock("snow", 39, 10, 59);

            //ROW 11
            _mapBuilder.PlaceBlock("snow", 38, 11, 15);
            _mapBuilder.PlaceBlock("snow", 39, 11, 53);

            //ROW 12
            _mapBuilder.PlaceBlock("snow", 38, 12, 15);
            _mapBuilder.PlaceBlock("snow", 39, 12, 65);

            //ROW 13
            _mapBuilder.PlaceBlock("snow", 38, 13, 15);
            _mapBuilder.PlaceBlock("snow", 39, 13, 47);

            //ROW 14
            _mapBuilder.PlaceBlock("snow", 20, 14, 68);
            _mapBuilder.PlaceBlock("snow", 21, 14, 0);
            _mapBuilder.PlaceBlock("snow", 22, 14, 0);
            _mapBuilder.PlaceBlock("snow", 23, 14, 0);
            _mapBuilder.PlaceBlock("snow", 24, 14, 75);

            _mapBuilder.PlaceBlock("snow", 38, 14, 14);
            _mapBuilder.PlaceBlock("snow", 39, 14, 53);

            // UPDATED

            //ROW 15
            _mapBuilder.PlaceBlock("snow", 20, 15, 14);
            _mapBuilder.PlaceBlock("snow", 21, 15, 65);
            _mapBuilder.PlaceBlock("snow", 22, 15, 53);
            _mapBuilder.PlaceBlock("snow", 23, 15, 4);
            _mapBuilder.PlaceBlock("snow", 24, 15, 84);

            _mapBuilder.PlaceBlock("snow", 32, 15, 69);
            _mapBuilder.PlaceBlock("snow", 33, 15, 3);
            _mapBuilder.PlaceBlock("snow", 34, 15, 1);
            _mapBuilder.PlaceBlock("snow", 35, 15, 1);
            _mapBuilder.PlaceBlock("snow", 36, 15, 1);
            _mapBuilder.PlaceBlock("snow", 37, 15, 2);
            _mapBuilder.PlaceBlock("snow", 38, 15, 22);
            _mapBuilder.PlaceBlock("snow", 39, 15, 89);


            //ROW 16
            _mapBuilder.PlaceBlock("snow", 20, 16, 78);
            _mapBuilder.PlaceBlock("snow", 21, 16, 16);
            _mapBuilder.PlaceBlock("snow", 22, 16, 35);
            _mapBuilder.PlaceBlock("snow", 23, 16, 19);

            _mapBuilder.PlaceBlock("snow", 32, 16, 15);
            _mapBuilder.PlaceBlock("snow", 33, 16, 11);
            _mapBuilder.PlaceBlock("snow", 34, 16, 47);
            _mapBuilder.PlaceBlock("snow", 35, 16, 11);
            _mapBuilder.PlaceBlock("snow", 36, 16, 71);
            _mapBuilder.PlaceBlock("snow", 37, 16, 83);
            _mapBuilder.PlaceBlock("snow", 38, 16, 89);
            _mapBuilder.PlaceBlock("snow", 39, 16, 89);

            // ROW 17
            _mapBuilder.PlaceBlock("snow", 21, 17, 13);
            _mapBuilder.PlaceBlock("snow", 22, 17, 35);

            _mapBuilder.PlaceBlock("snow", 32, 17, 15);
            _mapBuilder.PlaceBlock("snow", 33, 17, 65);

            // ROW 18
            _mapBuilder.PlaceBlock("snow", 21, 18, 12);

            _mapBuilder.PlaceBlock("snow", 30, 18, 1);
            _mapBuilder.PlaceBlock("snow", 31, 18, 2);
            _mapBuilder.PlaceBlock("snow", 32, 18, 22);
            _mapBuilder.PlaceBlock("snow", 33, 18, 83);
            // the rest are in the block  loop below

            // GIANT BLOCK IN BOTTOM RIGHT CORNER
            for (int x = 34; x <= 39; x++)
            {
                for (int y = 17; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 83);
                }
            }

            // smaller rectangle of black blocks
            for (int x = 32; x <= 33; x++)
            {
                for (int y = 19; y <= 21; y++)
                {
                    _mapBuilder.PlaceBlock("snow", x, y, 83);
                }
            }

            //ROW 19
            _mapBuilder.PlaceBlock("snow", 21, 19, 14);

            //ROW 20
            _mapBuilder.PlaceBlock("snow", 7, 20, 1);
            _mapBuilder.PlaceBlock("snow", 8, 20, 2);
            _mapBuilder.PlaceBlock("snow", 9, 20, 0);
            _mapBuilder.PlaceBlock("snow", 10, 20, 1);
            _mapBuilder.PlaceBlock("snow", 11, 20, 1);

            _mapBuilder.PlaceBlock("snow", 21, 20, 12);

            _mapBuilder.PlaceBlock("snow", 31, 20, 83);

            // ROW 21
            _mapBuilder.PlaceBlock("snow", 8, 21, 29);
            _mapBuilder.PlaceBlock("snow", 9, 21, 29);
            _mapBuilder.PlaceBlock("snow", 10, 21, 17);
            _mapBuilder.PlaceBlock("snow", 11, 21, 20);

            _mapBuilder.PlaceBlock("snow", 30, 21, 83);
            _mapBuilder.PlaceBlock("snow", 31, 21, 83);


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
            _mapBuilder.PlaceBlock("cement", 20, 1, 16);
            _mapBuilder.PlaceBlock("cement", 21, 1, 83);
            _mapBuilder.PlaceBlock("cement", 22, 1, 83);
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
            _mapBuilder.PlaceBlock("cement", 0, 3, 83);
            _mapBuilder.PlaceBlock("cement", 1, 3, 83);

            _mapBuilder.PlaceBlock("cement", 9, 3, 85);

            _mapBuilder.PlaceBlock("cement", 20, 3, 13);
            _mapBuilder.PlaceBlock("cement", 21, 3, 5);
            _mapBuilder.PlaceBlock("cement", 22, 3, 83);

            _mapBuilder.PlaceBlock("cement", 29, 3, 15);
            _mapBuilder.PlaceBlock("cement", 30, 3, 17);
            _mapBuilder.PlaceBlock("cement", 31, 3, 83);
            _mapBuilder.PlaceBlock("cement", 32, 3, 47);

            _mapBuilder.PlaceBlock("cement", 38, 3, 15);
            _mapBuilder.PlaceBlock("cement", 39, 3, 5);

            // ROW 4
            _mapBuilder.PlaceBlock("cement", 0, 4, 83);
            _mapBuilder.PlaceBlock("cement", 1, 4, 11);

            _mapBuilder.PlaceBlock("cement", 22, 4, 83);

            _mapBuilder.PlaceBlock("cement", 29, 4, 12);
            _mapBuilder.PlaceBlock("cement", 30, 4, 17);
            _mapBuilder.PlaceBlock("cement", 31, 4, 41);
            _mapBuilder.PlaceBlock("cement", 32, 4, 41);

            _mapBuilder.PlaceBlock("cement", 38, 4, 14);
            _mapBuilder.PlaceBlock("cement", 39, 4, 65);

            // ROW 5
            _mapBuilder.PlaceBlock("cement", 0, 5, 89);
            _mapBuilder.PlaceBlock("cement", 1, 5, 83);

            _mapBuilder.PlaceBlock("cement", 29, 5, 81);
            _mapBuilder.PlaceBlock("cement", 30, 5, 6);
            _mapBuilder.PlaceBlock("cement", 31, 5, 7);
            _mapBuilder.PlaceBlock("cement", 32, 5, 16);

            _mapBuilder.PlaceBlock("cement", 38, 5, 15);
            _mapBuilder.PlaceBlock("cement", 39, 5, 71);

            // ROW 6
            _mapBuilder.PlaceBlock("cement", 0, 6, 83);
            _mapBuilder.PlaceBlock("cement", 1, 6, 65);

            _mapBuilder.PlaceBlock("cement", 32, 6, 14);

            _mapBuilder.PlaceBlock("cement", 38, 6, 15);
            _mapBuilder.PlaceBlock("cement", 39, 6, 23);

            // ROW 7
            _mapBuilder.PlaceBlock("cement", 0, 7, 83);
            _mapBuilder.PlaceBlock("cement", 1, 7, 4);

            _mapBuilder.PlaceBlock("cement", 32, 7, 12);

            _mapBuilder.PlaceBlock("cement", 38, 7, 13);
            _mapBuilder.PlaceBlock("cement", 39, 7, 47);

            // ROW 8
            _mapBuilder.PlaceBlock("cement", 0, 8, 29);
            _mapBuilder.PlaceBlock("cement", 1, 8, 18);

            _mapBuilder.PlaceBlock("cement", 32, 8, 13);

            _mapBuilder.PlaceBlock("cement", 38, 8, 14);
            _mapBuilder.PlaceBlock("cement", 39, 8, 47);

            // ROW 9
            _mapBuilder.PlaceBlock("cement", 0, 9, 17);
            _mapBuilder.PlaceBlock("cement", 1, 9, 18);

            _mapBuilder.PlaceBlock("cement", 32, 9, 80);

            _mapBuilder.PlaceBlock("cement", 39, 9, 59);

            // ROW 10
            _mapBuilder.PlaceBlock("cement", 0, 10, 65);
            _mapBuilder.PlaceBlock("cement", 1, 10, 19);

            // ROW 11
            _mapBuilder.PlaceBlock("cement", 0, 11, 4);
            _mapBuilder.PlaceBlock("cement", 1, 11, 87);

            // ROW 12
            _mapBuilder.PlaceBlock("cement", 0, 12, 18);

            // ROW 13
            _mapBuilder.PlaceBlock("cement", 0, 13, 20);

            // ROW 14
            _mapBuilder.PlaceBlock("cement", 0, 14, 19);

            // ROW 15
            _mapBuilder.PlaceBlock("cement", 0, 15, 19);

            // ROW 16
            _mapBuilder.PlaceBlock("cement", 0, 16, 19);

            // ROW 17
            _mapBuilder.PlaceBlock("cement", 0, 17, 21);
            _mapBuilder.PlaceBlock("cement", 23, 17, 18);

            // ROW 18
            _mapBuilder.PlaceBlock("cement", 0, 18, 18);

            _mapBuilder.PlaceBlock("cement", 22, 18, 11);
            _mapBuilder.PlaceBlock("cement", 23, 18, 19);

            _mapBuilder.PlaceBlock("cement", 28, 18, 66);
            _mapBuilder.PlaceBlock("cement", 29, 18, 0);

            // ROW 19
            _mapBuilder.PlaceBlock("cement", 0, 19, 21);

            _mapBuilder.PlaceBlock("cement", 22, 19, 71);
            _mapBuilder.PlaceBlock("cement", 23, 19, 19);

            _mapBuilder.PlaceBlock("cement", 28, 19, 14);
            _mapBuilder.PlaceBlock("cement", 29, 19, 47);
            _mapBuilder.PlaceBlock("cement", 30, 19, 5);
            _mapBuilder.PlaceBlock("cement", 31, 19, 29);

            // ROW 20
            _mapBuilder.PlaceBlock("cement", 0, 20, 21);

            _mapBuilder.PlaceBlock("cement", 6, 20, 66);

            _mapBuilder.PlaceBlock("cement", 22, 20, 35);
            _mapBuilder.PlaceBlock("cement", 23, 20, 10);
            _mapBuilder.PlaceBlock("cement", 24, 20, 3);
            _mapBuilder.PlaceBlock("cement", 25, 20, 72);

            _mapBuilder.PlaceBlock("cement", 28, 20, 15);
            _mapBuilder.PlaceBlock("cement", 29, 20, 65);
            _mapBuilder.PlaceBlock("cement", 30, 20, 83);

            // ROW 21

            _mapBuilder.PlaceBlock("cement", 0, 21, 20);

            _mapBuilder.PlaceBlock("cement", 6, 21, 15);
            _mapBuilder.PlaceBlock("cement", 7, 21, 71);

            _mapBuilder.PlaceBlock("cement", 21, 21, 12);
            _mapBuilder.PlaceBlock("cement", 22, 21, 71);
            _mapBuilder.PlaceBlock("cement", 23, 21, 83);
            _mapBuilder.PlaceBlock("cement", 24, 21, 11);
            _mapBuilder.PlaceBlock("cement", 25, 21, 10);
            _mapBuilder.PlaceBlock("cement", 26, 21, 3);
            _mapBuilder.PlaceBlock("cement", 27, 21, 2);
            _mapBuilder.PlaceBlock("cement", 28, 21, 22);
            _mapBuilder.PlaceBlock("cement", 29, 21, 83);
            _mapBuilder.PlaceBlock("cement", 30, 21, 89);

            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER, USING STRAIGHT LINES FOR NOW

            // ROW 0

            _mapBuilder.PlaceBlock("girder", 14, 0, 4);
            _mapBuilder.PlaceBlock("girder", 14, 1, 15);
            _mapBuilder.PlaceBlock("girder", 14, 2, 15);
            _mapBuilder.PlaceBlock("girder", 14, 3, 15);
            _mapBuilder.PlaceBlock("girder", 14, 4, 4);

            _mapBuilder.PlaceBlock("girder", 33, 0, 15);
            _mapBuilder.PlaceBlock("girder", 33, 1, 15);
            _mapBuilder.PlaceBlock("girder", 33, 2, 15);
            _mapBuilder.PlaceBlock("girder", 33, 3, 15);
            _mapBuilder.PlaceBlock("girder", 33, 4, 15);
            _mapBuilder.PlaceBlock("girder", 33, 5, 15);
            _mapBuilder.PlaceBlock("girder", 33, 6, 15);
            _mapBuilder.PlaceBlock("girder", 33, 7, 15);
            _mapBuilder.PlaceBlock("girder", 33, 8, 15);
            _mapBuilder.PlaceBlock("girder", 33, 9, 15);
            _mapBuilder.PlaceBlock("girder", 33, 10, 15);
            _mapBuilder.PlaceBlock("girder", 33, 11, 4);


            _mapBuilder.PlaceBlock("girder", 8, 1, 15);
            _mapBuilder.PlaceBlock("girder", 8, 2, 15);
            _mapBuilder.PlaceBlock("girder", 8, 3, 4);
            _mapBuilder.PlaceBlock("girder", 8, 4, 15);
            _mapBuilder.PlaceBlock("girder", 8, 5, 15);
            _mapBuilder.PlaceBlock("girder", 8, 6, 15);
            _mapBuilder.PlaceBlock("girder", 8, 7, 4);


            _mapBuilder.PlaceBlock("girder", 2, 2, 15);
            _mapBuilder.PlaceBlock("girder", 2, 3, 4);
            _mapBuilder.PlaceBlock("girder", 2, 4, 15);
            _mapBuilder.PlaceBlock("girder", 2, 5, 4);
            _mapBuilder.PlaceBlock("girder", 2, 6, 15);
            _mapBuilder.PlaceBlock("girder", 2, 7, 7);


            // 3 
            _mapBuilder.PlaceBlock("girder", 3, 3, 25);
            _mapBuilder.PlaceBlock("girder", 4, 3, 24);
            _mapBuilder.PlaceBlock("girder", 5, 3, 26);
            _mapBuilder.PlaceBlock("girder", 6, 3, 26);
            _mapBuilder.PlaceBlock("girder", 7, 3, 27);

            // 5
            _mapBuilder.PlaceBlock("girder", 3, 5, 25);
            _mapBuilder.PlaceBlock("girder", 4, 5, 27);
            _mapBuilder.PlaceBlock("girder", 5, 5, 24);
            _mapBuilder.PlaceBlock("girder", 6, 5, 24);
            _mapBuilder.PlaceBlock("girder", 7, 5, 26);

            // 7
            _mapBuilder.PlaceBlock("girder", 3, 7, 25);
            _mapBuilder.PlaceBlock("girder", 4, 7, 24);
            _mapBuilder.PlaceBlock("girder", 5, 7, 25);
            _mapBuilder.PlaceBlock("girder", 6, 7, 25);
            _mapBuilder.PlaceBlock("girder", 7, 7, 25);

            // VERTICAL 14 & 15

            _mapBuilder.PlaceBlock("girder", 14, 18, 66);
            _mapBuilder.PlaceBlock("girder", 14, 19, 15);
            _mapBuilder.PlaceBlock("girder", 14, 20, 4);
            _mapBuilder.PlaceBlock("girder", 14, 21, 15);

            _mapBuilder.PlaceBlock("girder", 15, 18, 68);
            _mapBuilder.PlaceBlock("girder", 15, 19, 15);
            _mapBuilder.PlaceBlock("girder", 15, 20, 15);
            _mapBuilder.PlaceBlock("girder", 15, 21, 15);


            _mapBuilder.PlaceBlock("girder", 12, 20, 27);
            _mapBuilder.PlaceBlock("girder", 13, 20, 24);

            /* --------------- HAZARD BLOCKS ---------------*/

            // UPDATED

            // SPIKES
            for (int x = 20; x <= 24; x++)
            {
                _mapBuilder.PlaceBlock("upSpike", x, 13, 0);
            }
            _mapBuilder.PlaceBlock("upSpike", 24, 19, 0);
            _mapBuilder.PlaceBlock("upSpike", 25, 19, 0);

            _mapBuilder.PlaceBlock("upSpike", 26, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 27, 20, 0);

            // SPRING
            _mapBuilder.PlaceBlock("spring", 14, 16, 0);

        }
    }
}
