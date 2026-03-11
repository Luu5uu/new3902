using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Celeste.Blocks.Rooms
{
    public class RoomOne
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;


        public RoomOne(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void PlaceRoomOneBlocks()
        {
            _mapBuilder.ClearBlocks();

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 1
            _mapBuilder.PlaceBlock("snow", 0, 0, 83);
            _mapBuilder.PlaceBlock("snow", 1, 0, 77);
            _mapBuilder.PlaceBlock("snow", 2, 0, 47);
            _mapBuilder.PlaceBlock("snow", 3, 0, 41);

            _mapBuilder.PlaceBlock("snow", 5, 0, 6);
            _mapBuilder.PlaceBlock("snow", 6, 0, 16);
            _mapBuilder.PlaceBlock("snow", 7, 0, 59);
            _mapBuilder.PlaceBlock("snow", 8, 0, 4);
            _mapBuilder.PlaceBlock("snow", 9, 0, 8);
            _mapBuilder.PlaceBlock("snow", 10, 0, 8);
            _mapBuilder.PlaceBlock("snow", 11, 0, 6);
            _mapBuilder.PlaceBlock("snow", 12, 0, 6);
            _mapBuilder.PlaceBlock("snow", 13, 0, 16);


            // ROW 2 (Y=1)
            _mapBuilder.PlaceBlock("snow", 0, 1, 77);
            _mapBuilder.PlaceBlock("snow", 1, 1, 4);
            _mapBuilder.PlaceBlock("snow", 2, 1, 8);
            _mapBuilder.PlaceBlock("snow", 3, 1, 6);


            _mapBuilder.PlaceBlock("snow", 6, 1, 78);
            _mapBuilder.PlaceBlock("snow", 7, 1, 9);
            _mapBuilder.PlaceBlock("snow", 8, 1, 19);

            _mapBuilder.PlaceBlock("snow", 13, 1, 13);
            _mapBuilder.PlaceBlock("snow", 14, 1, 29);

            _mapBuilder.PlaceBlock("snow", 38, 1, 83);
            _mapBuilder.PlaceBlock("snow", 39, 1, 89);

            // ROW 3 (Y=2)
            _mapBuilder.PlaceBlock("snow", 0, 2, 35);

            _mapBuilder.PlaceBlock("snow", 35, 2, 69);
            _mapBuilder.PlaceBlock("snow", 36, 2, 22);
            _mapBuilder.PlaceBlock("snow", 37, 2, 77);
            _mapBuilder.PlaceBlock("snow", 38, 2, 89);
            _mapBuilder.PlaceBlock("snow", 39, 2, 89);

            // ROW 4 
            _mapBuilder.PlaceBlock("snow", 35, 3, 2);
            _mapBuilder.PlaceBlock("snow", 36, 3, 11);
            _mapBuilder.PlaceBlock("snow", 37, 3, 77);
            _mapBuilder.PlaceBlock("snow", 38, 3, 89);
            _mapBuilder.PlaceBlock("snow", 39, 3, 89);

            // ROW 5
            _mapBuilder.PlaceBlock("snow", 34, 4, 56);
            _mapBuilder.PlaceBlock("snow", 35, 4, 40);
            _mapBuilder.PlaceBlock("snow", 36, 4, 77);
            _mapBuilder.PlaceBlock("snow", 37, 4, 77);
            _mapBuilder.PlaceBlock("snow", 38, 4, 89);
            _mapBuilder.PlaceBlock("snow", 39, 4, 89);

            // ROW 6
            _mapBuilder.PlaceBlock("snow", 35, 5, 15);
            _mapBuilder.PlaceBlock("snow", 36, 5, 17);
            _mapBuilder.PlaceBlock("snow", 37, 5, 83);
            _mapBuilder.PlaceBlock("snow", 38, 5, 89);
            _mapBuilder.PlaceBlock("snow", 39, 5, 89);

            // ROW 7
            _mapBuilder.PlaceBlock("snow", 35, 6, 14);
            _mapBuilder.PlaceBlock("snow", 36, 6, 71);
            _mapBuilder.PlaceBlock("snow", 37, 6, 77);
            _mapBuilder.PlaceBlock("snow", 38, 6, 89);
            _mapBuilder.PlaceBlock("snow", 39, 6, 89);

            // ROW 8
            _mapBuilder.PlaceBlock("snow", 31, 7, 68);
            _mapBuilder.PlaceBlock("snow", 32, 7, 0);
            _mapBuilder.PlaceBlock("snow", 33, 7, 3);
            _mapBuilder.PlaceBlock("snow", 34, 7, 0);
            _mapBuilder.PlaceBlock("snow", 35, 7, 22);
            _mapBuilder.PlaceBlock("snow", 36, 7, 77);
            _mapBuilder.PlaceBlock("snow", 37, 7, 83);
            _mapBuilder.PlaceBlock("snow", 38, 7, 89);
            _mapBuilder.PlaceBlock("snow", 39, 7, 89);

            // ROW 9
            _mapBuilder.PlaceBlock("snow", 31, 8, 15);
            _mapBuilder.PlaceBlock("snow", 32, 8, 41);
            _mapBuilder.PlaceBlock("snow", 33, 8, 35);
            _mapBuilder.PlaceBlock("snow", 34, 8, 11);
            _mapBuilder.PlaceBlock("snow", 35, 8, 77);
            _mapBuilder.PlaceBlock("snow", 36, 8, 83);
            _mapBuilder.PlaceBlock("snow", 37, 8, 89);
            _mapBuilder.PlaceBlock("snow", 38, 8, 89);
            _mapBuilder.PlaceBlock("snow", 39, 8, 89);

            // ROW 10
            _mapBuilder.PlaceBlock("snow", 31, 9, 78);
            _mapBuilder.PlaceBlock("snow", 32, 9, 8);
            _mapBuilder.PlaceBlock("snow", 33, 9, 8);
            _mapBuilder.PlaceBlock("snow", 34, 9, 16);
            _mapBuilder.PlaceBlock("snow", 35, 9, 77);
            _mapBuilder.PlaceBlock("snow", 36, 9, 83);
            _mapBuilder.PlaceBlock("snow", 37, 9, 89);
            _mapBuilder.PlaceBlock("snow", 38, 9, 89);
            _mapBuilder.PlaceBlock("snow", 39, 9, 89);

            // ROW 11
            _mapBuilder.PlaceBlock("snow", 34, 10, 13);
            _mapBuilder.PlaceBlock("snow", 35, 10, 53);
            _mapBuilder.PlaceBlock("snow", 36, 10, 77);
            _mapBuilder.PlaceBlock("snow", 37, 10, 83);
            _mapBuilder.PlaceBlock("snow", 38, 10, 89);
            _mapBuilder.PlaceBlock("snow", 39, 10, 89);

            // ROW 12
            _mapBuilder.PlaceBlock("snow", 34, 11, 80);
            _mapBuilder.PlaceBlock("snow", 35, 11, 9);
            _mapBuilder.PlaceBlock("snow", 36, 11, 16);
            _mapBuilder.PlaceBlock("snow", 37, 11, 77);
            _mapBuilder.PlaceBlock("snow", 38, 11, 83);
            _mapBuilder.PlaceBlock("snow", 39, 11, 89);

            // ROW 13
            _mapBuilder.PlaceBlock("snow", 36, 12, 14);
            _mapBuilder.PlaceBlock("snow", 37, 12, 47);
            _mapBuilder.PlaceBlock("snow", 38, 12, 83);
            _mapBuilder.PlaceBlock("snow", 39, 12, 89);

            // ROW 14
            _mapBuilder.PlaceBlock("snow", 36, 13, 13);
            _mapBuilder.PlaceBlock("snow", 39, 13, 83);

            // ROW 15
            _mapBuilder.PlaceBlock("snow", 20, 14, 14);
            _mapBuilder.PlaceBlock("snow", 21, 14, 21);

            _mapBuilder.PlaceBlock("snow", 36, 14, 13);

            // ROW 16
            _mapBuilder.PlaceBlock("snow", 18, 15, 14);
            _mapBuilder.PlaceBlock("snow", 19, 15, 1);
            _mapBuilder.PlaceBlock("snow", 20, 15, 22);
            _mapBuilder.PlaceBlock("snow", 21, 15, 20);

            // ROW 17
            _mapBuilder.PlaceBlock("snow", 18, 16, 15);
            _mapBuilder.PlaceBlock("snow", 19, 16, 47);
            _mapBuilder.PlaceBlock("snow", 20, 16, 77);
            _mapBuilder.PlaceBlock("snow", 21, 16, 10);
            _mapBuilder.PlaceBlock("snow", 22, 16, 1);
            _mapBuilder.PlaceBlock("snow", 23, 16, 18);
            _mapBuilder.PlaceBlock("snow", 24, 16, 1);
            _mapBuilder.PlaceBlock("snow", 25, 16, 20);

            // ROW 18
            _mapBuilder.PlaceBlock("snow", 18, 17, 14);
            _mapBuilder.PlaceBlock("snow", 19, 17, 11);
            _mapBuilder.PlaceBlock("snow", 20, 17, 77);
            _mapBuilder.PlaceBlock("snow", 21, 17, 77);
            _mapBuilder.PlaceBlock("snow", 22, 17, 35);
            _mapBuilder.PlaceBlock("snow", 23, 17, 5);
            _mapBuilder.PlaceBlock("snow", 24, 17, 47);
            _mapBuilder.PlaceBlock("snow", 25, 17, 20);

            // ROW 20
            _mapBuilder.PlaceBlock("snow", 1, 18, 0);
            _mapBuilder.PlaceBlock("snow", 2, 18, 1);

            _mapBuilder.PlaceBlock("snow", 18, 18, 14);
            _mapBuilder.PlaceBlock("snow", 19, 18, 53);
            _mapBuilder.PlaceBlock("snow", 20, 18, 77);
            _mapBuilder.PlaceBlock("snow", 21, 18, 83);
            _mapBuilder.PlaceBlock("snow", 22, 18, 77);
            _mapBuilder.PlaceBlock("snow", 23, 18, 77);

            _mapBuilder.PlaceBlock("snow", 25, 18, 20);

            // ROW 21
            _mapBuilder.PlaceBlock("snow", 1, 19, 29);

            _mapBuilder.PlaceBlock("snow", 15, 19, 1);
            _mapBuilder.PlaceBlock("snow", 16, 19, 0);
            _mapBuilder.PlaceBlock("snow", 17, 19, 3);
            _mapBuilder.PlaceBlock("snow", 18, 19, 22);
            _mapBuilder.PlaceBlock("snow", 19, 19, 77);
            _mapBuilder.PlaceBlock("snow", 20, 19, 83);
            _mapBuilder.PlaceBlock("snow", 21, 19, 89);
            _mapBuilder.PlaceBlock("snow", 22, 19, 83);
            _mapBuilder.PlaceBlock("snow", 23, 19, 83);

            // ROW 22
            _mapBuilder.PlaceBlock("snow", 16, 20, 71);
            _mapBuilder.PlaceBlock("snow", 17, 20, 47);
            _mapBuilder.PlaceBlock("snow", 18, 20, 77);
            _mapBuilder.PlaceBlock("snow", 19, 20, 83);
            _mapBuilder.PlaceBlock("snow", 20, 20, 89);
            _mapBuilder.PlaceBlock("snow", 21, 20, 89);

            // ROW 23
            _mapBuilder.PlaceBlock("snow", 17, 21, 77);
            _mapBuilder.PlaceBlock("snow", 18, 21, 83);
            _mapBuilder.PlaceBlock("snow", 19, 21, 89);
            _mapBuilder.PlaceBlock("snow", 20, 21, 89);
            _mapBuilder.PlaceBlock("snow", 21, 21, 89);

            /* --------------- DIRT BLOCKS ---------------*/

            // the format: (SOURCE, IN-GAME X COORD, IN-GAME Y COORD, FRAME FROM TILESHEET)

            // ROW 0
            _mapBuilder.PlaceBlock("dirt", 14, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 16, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 17, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 18, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 19, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 20, 0, 0);

            _mapBuilder.PlaceBlock("dirt", 22, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 23, 0, 0);
            _mapBuilder.PlaceBlock("dirt", 24, 0, 0);

            // ROW 1
            _mapBuilder.PlaceBlock("dirt", 15, 1, 0);
            _mapBuilder.PlaceBlock("dirt", 16, 1, 0);
            _mapBuilder.PlaceBlock("dirt", 17, 1, 0);
            _mapBuilder.PlaceBlock("dirt", 18, 1, 0);
            _mapBuilder.PlaceBlock("dirt", 19, 1, 0);
            _mapBuilder.PlaceBlock("dirt", 20, 1, 0);

            // ROW 2 
            _mapBuilder.PlaceBlock("dirt", 1, 2, 0);

            _mapBuilder.PlaceBlock("dirt", 13, 2, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 2, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 2, 0);
            _mapBuilder.PlaceBlock("dirt", 16, 2, 0);
            _mapBuilder.PlaceBlock("dirt", 17, 2, 0);

            // ROW 3
            _mapBuilder.PlaceBlock("dirt", 0, 3, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 3, 0);

            _mapBuilder.PlaceBlock("dirt", 13, 3, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 3, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 3, 0);

            // ROW 4
            _mapBuilder.PlaceBlock("dirt", 0, 4, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 4, 0);

            _mapBuilder.PlaceBlock("dirt", 13, 4, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 4, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 4, 0);

            // ROW 5
            _mapBuilder.PlaceBlock("dirt", 0, 5, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 5, 0);

            _mapBuilder.PlaceBlock("dirt", 13, 5, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 5, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 5, 0);

            // ROW 6
            _mapBuilder.PlaceBlock("dirt", 0, 6, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 6, 0);

            // ROW 7
            _mapBuilder.PlaceBlock("dirt", 0, 7, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 7, 0);

            // ROW 8  
            _mapBuilder.PlaceBlock("dirt", 1, 8, 0);

            // ROW 13
            _mapBuilder.PlaceBlock("dirt", 37, 13, 0);
            _mapBuilder.PlaceBlock("dirt", 38, 13, 0);

            // ROW 14
            _mapBuilder.PlaceBlock("dirt", 37, 14, 0);
            _mapBuilder.PlaceBlock("dirt", 38, 14, 0);
            _mapBuilder.PlaceBlock("dirt", 39, 14, 0);

            // ROW 15
            _mapBuilder.PlaceBlock("dirt", 36, 15, 0);
            _mapBuilder.PlaceBlock("dirt", 37, 15, 0);
            _mapBuilder.PlaceBlock("dirt", 38, 15, 0);
            _mapBuilder.PlaceBlock("dirt", 39, 15, 0);

            // ROW 16
            _mapBuilder.PlaceBlock("dirt", 10, 16, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 16, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 16, 0);

            _mapBuilder.PlaceBlock("dirt", 37, 16, 0);
            _mapBuilder.PlaceBlock("dirt", 38, 16, 0);
            _mapBuilder.PlaceBlock("dirt", 39, 16, 0);

            // ROW 17
            _mapBuilder.PlaceBlock("dirt", 10, 17, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 17, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 17, 0);

            _mapBuilder.PlaceBlock("dirt", 37, 17, 0);
            _mapBuilder.PlaceBlock("dirt", 38, 17, 0);
            _mapBuilder.PlaceBlock("dirt", 39, 17, 0);

            // ROW 18
            _mapBuilder.PlaceBlock("dirt", 3, 18, 0);
            _mapBuilder.PlaceBlock("dirt", 4, 18, 0);

            _mapBuilder.PlaceBlock("dirt", 10, 18, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 18, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 18, 0);

            _mapBuilder.PlaceBlock("dirt", 24, 18, 0);

            _mapBuilder.PlaceBlock("dirt", 38, 18, 0);
            _mapBuilder.PlaceBlock("dirt", 39, 18, 0);

            // ROW 19
            _mapBuilder.PlaceBlock("dirt", 0, 19, 0);

            _mapBuilder.PlaceBlock("dirt", 2, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 3, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 4, 19, 0);

            _mapBuilder.PlaceBlock("dirt", 10, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 13, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 19, 0);

            _mapBuilder.PlaceBlock("dirt", 24, 19, 0);
            _mapBuilder.PlaceBlock("dirt", 25, 19, 0);

            _mapBuilder.PlaceBlock("dirt", 39, 19, 0);

            // ROW 20
            _mapBuilder.PlaceBlock("dirt", 0, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 2, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 3, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 4, 20, 0);

            _mapBuilder.PlaceBlock("dirt", 10, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 13, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 20, 0);

            _mapBuilder.PlaceBlock("dirt", 22, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 23, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 24, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 25, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 26, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 27, 20, 0);
            _mapBuilder.PlaceBlock("dirt", 28, 20, 0);

            _mapBuilder.PlaceBlock("dirt", 39, 20, 0);

            // ROW 21
            _mapBuilder.PlaceBlock("dirt", 0, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 1, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 2, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 3, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 4, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 5, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 6, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 7, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 8, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 9, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 10, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 11, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 12, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 13, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 14, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 15, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 16, 21, 0);


            _mapBuilder.PlaceBlock("dirt", 22, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 23, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 24, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 25, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 26, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 27, 21, 0);
            _mapBuilder.PlaceBlock("dirt", 28, 21, 0);

            _mapBuilder.PlaceBlock("dirt", 39, 21, 0);

            /* --------------- GIRDER BLOCKS ---------------*/

            // 4 IS PLACEHOLDER

            // ROW 0
            _mapBuilder.PlaceBlock("girder", 4, 0, 4);
            _mapBuilder.PlaceBlock("girder", 21, 0, 4);

            // ROW 1
            _mapBuilder.PlaceBlock("girder", 4, 1, 4);
            _mapBuilder.PlaceBlock("girder", 21, 1, 4);

            // ROW 2
            _mapBuilder.PlaceBlock("girder", 4, 2, 4);
            _mapBuilder.PlaceBlock("girder", 8, 2, 4);

            _mapBuilder.PlaceBlock("girder", 21, 2, 4);

            // ROW 3
            _mapBuilder.PlaceBlock("girder", 4, 3, 4);
            _mapBuilder.PlaceBlock("girder", 5, 3, 4);
            _mapBuilder.PlaceBlock("girder", 6, 3, 4);
            _mapBuilder.PlaceBlock("girder", 8, 3, 4);
            _mapBuilder.PlaceBlock("girder", 21, 3, 4);

            // ROW 4
            _mapBuilder.PlaceBlock("girder", 4, 4, 4);
            _mapBuilder.PlaceBlock("girder", 8, 4, 4);

            // ROW 5
            _mapBuilder.PlaceBlock("girder", 4, 5, 4);
            _mapBuilder.PlaceBlock("girder", 8, 5, 4);

            // ROW 6
            _mapBuilder.PlaceBlock("girder", 4, 6, 4);
            _mapBuilder.PlaceBlock("girder", 5, 6, 4);
            _mapBuilder.PlaceBlock("girder", 6, 6, 4);
            _mapBuilder.PlaceBlock("girder", 7, 6, 4);
            _mapBuilder.PlaceBlock("girder", 8, 6, 4);

            // ROW 8
            _mapBuilder.PlaceBlock("girder", 0, 8, 4);

            // ROW 9
            _mapBuilder.PlaceBlock("girder", 0, 9, 4);

            // ROW 10
            _mapBuilder.PlaceBlock("girder", 0, 10, 4);

            _mapBuilder.PlaceBlock("girder", 18, 10, 4);
            _mapBuilder.PlaceBlock("girder", 19, 10, 4);
            _mapBuilder.PlaceBlock("girder", 20, 10, 4);
            _mapBuilder.PlaceBlock("girder", 21, 10, 4);

            // ROW 11
            _mapBuilder.PlaceBlock("girder", 0, 11, 4);

            _mapBuilder.PlaceBlock("girder", 18, 11, 4);

            _mapBuilder.PlaceBlock("girder", 21, 11, 4);

            // ROW 12
            _mapBuilder.PlaceBlock("girder", 0, 12, 4);

            _mapBuilder.PlaceBlock("girder", 18, 12, 4);

            _mapBuilder.PlaceBlock("girder", 21, 12, 4);

            // ROW 13
            _mapBuilder.PlaceBlock("girder", 0, 13, 4);

            _mapBuilder.PlaceBlock("girder", 18, 13, 4);
            _mapBuilder.PlaceBlock("girder", 19, 13, 4);
            _mapBuilder.PlaceBlock("girder", 20, 13, 4);
            _mapBuilder.PlaceBlock("girder", 21, 13, 4);

            // ROW 14
            _mapBuilder.PlaceBlock("girder", 0, 14, 4);

            _mapBuilder.PlaceBlock("girder", 18, 14, 4);

            // ROW 15
            _mapBuilder.PlaceBlock("girder", 0, 15, 4);

            // ROW 16
            _mapBuilder.PlaceBlock("girder", 0, 16, 4);

            // ROW 17
            _mapBuilder.PlaceBlock("girder", 0, 17, 4);

            // ROW 18
            _mapBuilder.PlaceBlock("girder", 0, 18, 4);

            /* --------------- CEMENT BLOCKS ---------------*/

            // 12 IS PLACEHOLDER

            // ROW 0
            _mapBuilder.PlaceBlock("cement", 25, 0, 12);
            _mapBuilder.PlaceBlock("cement", 26, 0, 12);
            _mapBuilder.PlaceBlock("cement", 27, 0, 12);
            _mapBuilder.PlaceBlock("cement", 28, 0, 12);
            _mapBuilder.PlaceBlock("cement", 29, 0, 12);
            _mapBuilder.PlaceBlock("cement", 30, 0, 12);

            _mapBuilder.PlaceBlock("cement", 36, 0, 12);
            _mapBuilder.PlaceBlock("cement", 37, 0, 12);
            _mapBuilder.PlaceBlock("cement", 38, 0, 12);
            _mapBuilder.PlaceBlock("cement", 39, 0, 12);

            // ROW 1
            _mapBuilder.PlaceBlock("cement", 36, 1, 12);
            _mapBuilder.PlaceBlock("cement", 37, 1, 12);

            /* --------------- HAZARDS BLOCKS ---------------*/

            // SPIKES
            _mapBuilder.PlaceBlock("leftSpike", 33, 10, 0);
            _mapBuilder.PlaceBlock("leftSpike", 33, 11, 0);


            _mapBuilder.PlaceBlock("upSpike", 22, 15, 0);
            _mapBuilder.PlaceBlock("upSpike", 23, 15, 0);
            _mapBuilder.PlaceBlock("upSpike", 24, 15, 0);
            _mapBuilder.PlaceBlock("upSpike", 25, 15, 0);

            _mapBuilder.PlaceBlock("upSpike", 13, 18, 0);
            _mapBuilder.PlaceBlock("upSpike", 14, 18, 0);
            _mapBuilder.PlaceBlock("upSpike", 15, 18, 0);
            _mapBuilder.PlaceBlock("upSpike", 16, 18, 0);
            _mapBuilder.PlaceBlock("upSpike", 17, 18, 0);

            _mapBuilder.PlaceBlock("upSpike", 26, 19, 0);
            _mapBuilder.PlaceBlock("upSpike", 27, 19, 0);
            _mapBuilder.PlaceBlock("upSpike", 28, 19, 0);

            _mapBuilder.PlaceBlock("upSpike", 5, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 6, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 7, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 8, 20, 0);
            _mapBuilder.PlaceBlock("upSpike", 9, 20, 0);
        }
    }
}