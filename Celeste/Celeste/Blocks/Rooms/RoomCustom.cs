using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.IO;

namespace Celeste.Blocks.Rooms
{
    public class RoomCustom
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;
        private RoomLoader _roomLoader;


        public RoomCustom(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
            _roomLoader = new RoomLoader(_mapBuilder, _blockFactory);
        }

        public void PlaceRoomCustomBlocks()
        {
            _mapBuilder.ClearBlocks();

            // csv  format: COMMENT BACK IN WHEN DONE
            string filePath = Path.Combine(AppContext.BaseDirectory, "Content", "rooms", "RoomCustom.csv");
            _roomLoader.LoadRoom(filePath);

            /* 
            // CEMENT 

            // ROW 0
            _mapBuilder.PlaceBlock("cement", 0, 0, 4);
            _mapBuilder.PlaceBlock("cement", 1, 0, 7);
            _mapBuilder.PlaceBlock("cement", 2, 0, 6);
            _mapBuilder.PlaceBlock("cement", 3, 0, 87);

            _mapBuilder.PlaceBlock("cement", 24, 0, 66);
            _mapBuilder.PlaceBlock("cement", 25, 0, 16);
            _mapBuilder.PlaceBlock("cement", 26, 0, 11);
            _mapBuilder.PlaceBlock("cement", 27, 0, 83);
            _mapBuilder.PlaceBlock("cement", 28, 0, 83);
            _mapBuilder.PlaceBlock("cement", 29, 0, 83);
            _mapBuilder.PlaceBlock("cement", 30, 0, 35);
            _mapBuilder.PlaceBlock("cement", 31, 0, 11);
            _mapBuilder.PlaceBlock("cement", 32, 0, 19);

            // ROW 1
            _mapBuilder.PlaceBlock("cement", 0, 1, 21);

            _mapBuilder.PlaceBlock("cement", 23, 1, 66);

            _mapBuilder.PlaceBlock("cement", 25, 1, 8);
            _mapBuilder.PlaceBlock("cement", 26, 1, 7);
            _mapBuilder.PlaceBlock("cement", 27, 1, 16);
            _mapBuilder.PlaceBlock("cement", 28, 1, 35);
            _mapBuilder.PlaceBlock("cement", 29, 1, 65);
            _mapBuilder.PlaceBlock("cement", 30, 1, 83);
            _mapBuilder.PlaceBlock("cement", 31, 1, 4);
            _mapBuilder.PlaceBlock("cement", 32, 1, 86);

            // LEFT WALL
            _mapBuilder.PlaceBlock("cement", 0, 2, 21);
            _mapBuilder.PlaceBlock("cement", 0, 3, 21);
            _mapBuilder.PlaceBlock("cement", 0, 4, 20);
            _mapBuilder.PlaceBlock("cement", 0, 5, 21);
            _mapBuilder.PlaceBlock("cement", 0, 6, 20);
            _mapBuilder.PlaceBlock("cement", 0, 7, 19);
            _mapBuilder.PlaceBlock("cement", 0, 8, 19);
            _mapBuilder.PlaceBlock("cement", 0, 9, 21);
            _mapBuilder.PlaceBlock("cement", 0, 10, 21);
            _mapBuilder.PlaceBlock("cement", 0, 11, 21);
            _mapBuilder.PlaceBlock("cement", 0, 12, 18);
            _mapBuilder.PlaceBlock("cement", 0, 13, 18);
            _mapBuilder.PlaceBlock("cement", 0, 14, 21);
            _mapBuilder.PlaceBlock("cement", 0, 15, 18);
            _mapBuilder.PlaceBlock("cement", 0, 16, 21);
            _mapBuilder.PlaceBlock("cement", 0, 17, 21);
            _mapBuilder.PlaceBlock("cement", 0, 18, 19);
            _mapBuilder.PlaceBlock("cement", 0, 19, 20);
            _mapBuilder.PlaceBlock("cement", 0, 20, 20);
            _mapBuilder.PlaceBlock("cement", 0, 21, 21);

            // ROW 2
            _mapBuilder.PlaceBlock("cement", 22, 2, 66);

            _mapBuilder.PlaceBlock("cement", 27, 2, 78);
            _mapBuilder.PlaceBlock("cement", 28, 2, 8);
            _mapBuilder.PlaceBlock("cement", 29, 2, 16);
            _mapBuilder.PlaceBlock("cement", 30, 2, 4);
            _mapBuilder.PlaceBlock("cement", 31, 2, 86);

            // ROW 3

            _mapBuilder.PlaceBlock("cement", 22, 3, 13);

            _mapBuilder.PlaceBlock("cement", 29, 3, 78);
            _mapBuilder.PlaceBlock("cement", 30, 3, 87);

            // ROW 4

            _mapBuilder.PlaceBlock("cement", 22, 4, 5);

            // RIGHT WALL
            _mapBuilder.PlaceBlock("cement", 39, 5, 66);
            _mapBuilder.PlaceBlock("cement", 39, 6, 14);
            _mapBuilder.PlaceBlock("cement", 39, 7, 13);
            _mapBuilder.PlaceBlock("cement", 39, 8, 22);
            _mapBuilder.PlaceBlock("cement", 38, 8, 67);
            _mapBuilder.PlaceBlock("cement", 39, 9, 41);

            // BOTTOM CEMENT, IN ROWS FROM TOP
            _mapBuilder.PlaceBlock("cement", 23, 17, 21);

            _mapBuilder.PlaceBlock("cement", 22, 18, 35);
            _mapBuilder.PlaceBlock("cement", 23, 18, 21);

            _mapBuilder.PlaceBlock("cement", 22, 19, 65);
            _mapBuilder.PlaceBlock("cement", 23, 19, 20);
            _mapBuilder.PlaceBlock("cement", 31, 19, 1);

            _mapBuilder.PlaceBlock("cement", 22, 20, 83);
            _mapBuilder.PlaceBlock("cement", 23, 20, 10);
            _mapBuilder.PlaceBlock("cement", 24, 20, 1);
            _mapBuilder.PlaceBlock("cement", 25, 20, 2);
            _mapBuilder.PlaceBlock("cement", 30, 20, 1);

            _mapBuilder.PlaceBlock("cement", 21, 21, 12);
            _mapBuilder.PlaceBlock("cement", 22, 21, 11);
            _mapBuilder.PlaceBlock("cement", 23, 21, 53);
            _mapBuilder.PlaceBlock("cement", 24, 21, 59);
            _mapBuilder.PlaceBlock("cement", 25, 21, 10);
            _mapBuilder.PlaceBlock("cement", 26, 21, 1);
            _mapBuilder.PlaceBlock("cement", 27, 21, 3);
            _mapBuilder.PlaceBlock("cement", 28, 21, 2);
            _mapBuilder.PlaceBlock("cement", 29, 21, 1);



            // SNOW

            // CEILING  BLOCKS
            _mapBuilder.PlaceBlock("snow", 24, 1, 59);

            _mapBuilder.PlaceBlock("snow", 23, 2, 29);
            _mapBuilder.PlaceBlock("snow", 24, 2, 20);

            _mapBuilder.PlaceBlock("snow", 23, 3, 23);
            _mapBuilder.PlaceBlock("snow", 24, 3, 21);

            _mapBuilder.PlaceBlock("snow", 21, 4, 68);
            _mapBuilder.PlaceBlock("snow", 23, 4, 47);
            _mapBuilder.PlaceBlock("snow", 24, 4, 18);

            _mapBuilder.PlaceBlock("snow", 21, 5, 13);
            _mapBuilder.PlaceBlock("snow", 22, 5, 16);
            _mapBuilder.PlaceBlock("snow", 23, 5, 4);
            _mapBuilder.PlaceBlock("snow", 24, 5, 86);

            _mapBuilder.PlaceBlock("snow", 21, 6, 12);
            _mapBuilder.PlaceBlock("snow", 22, 6, 35);
            _mapBuilder.PlaceBlock("snow", 23, 6, 21);

            _mapBuilder.PlaceBlock("snow", 21, 7, 13);
            _mapBuilder.PlaceBlock("snow", 22, 7, 29);
            _mapBuilder.PlaceBlock("snow", 23, 7, 18);

            _mapBuilder.PlaceBlock("snow", 21, 8, 78);
            _mapBuilder.PlaceBlock("snow", 22, 8, 7);
            _mapBuilder.PlaceBlock("snow", 23, 8, 84);

            // MIDDLE BOTTOM SNOW
            _mapBuilder.PlaceBlock("snow", 21, 14, 68);
            _mapBuilder.PlaceBlock("snow", 22, 14, 2);
            _mapBuilder.PlaceBlock("snow", 23, 14, 75);

            _mapBuilder.PlaceBlock("snow", 21, 15, 12);
            _mapBuilder.PlaceBlock("snow", 22, 15, 41);
            _mapBuilder.PlaceBlock("snow", 23, 15, 19);

            _mapBuilder.PlaceBlock("snow", 21, 16, 14);
            _mapBuilder.PlaceBlock("snow", 22, 16, 71);
            _mapBuilder.PlaceBlock("snow", 23, 16, 21);

            _mapBuilder.PlaceBlock("snow", 21, 17, 15);
            _mapBuilder.PlaceBlock("snow", 22, 17, 65);

            _mapBuilder.PlaceBlock("snow", 21, 18, 15);
            _mapBuilder.PlaceBlock("snow", 21, 19, 14);
            _mapBuilder.PlaceBlock("snow", 21, 20, 13);

            // RIGHT WALL SNOW

            _mapBuilder.PlaceBlock("snow", 38, 9, 12);
            _mapBuilder.PlaceBlock("snow", 38, 10, 14);
            _mapBuilder.PlaceBlock("snow", 38, 11, 14);
            _mapBuilder.PlaceBlock("snow", 38, 12, 13);
            _mapBuilder.PlaceBlock("snow", 38, 13, 15);
            _mapBuilder.PlaceBlock("snow", 38, 14, 13);
            _mapBuilder.PlaceBlock("snow", 38, 15, 12);
            _mapBuilder.PlaceBlock("snow", 38, 16, 13);
            _mapBuilder.PlaceBlock("snow", 38, 17, 12);
            _mapBuilder.PlaceBlock("snow", 38, 18, 22);
            _mapBuilder.PlaceBlock("snow", 38, 19, 83);
            _mapBuilder.PlaceBlock("snow", 38, 20, 83);
            _mapBuilder.PlaceBlock("snow", 38, 21, 83);

            _mapBuilder.PlaceBlock("snow", 39, 10, 35);
            _mapBuilder.PlaceBlock("snow", 39, 11, 23);
            _mapBuilder.PlaceBlock("snow", 39, 12, 23);
            _mapBuilder.PlaceBlock("snow", 39, 13, 17);
            _mapBuilder.PlaceBlock("snow", 39, 14, 11);
            _mapBuilder.PlaceBlock("snow", 39, 15, 47);
            _mapBuilder.PlaceBlock("snow", 39, 16, 17);
            _mapBuilder.PlaceBlock("snow", 39, 17, 59);
            _mapBuilder.PlaceBlock("snow", 39, 18, 83);
            _mapBuilder.PlaceBlock("snow", 39, 19, 83);
            _mapBuilder.PlaceBlock("snow", 39, 20, 83);
            _mapBuilder.PlaceBlock("snow", 39, 21, 83);

            //ROW 37 VERTICAL WALL
            _mapBuilder.PlaceBlock("snow", 37, 18, 68);
            _mapBuilder.PlaceBlock("snow", 37, 19, 22);
            _mapBuilder.PlaceBlock("snow", 37, 20, 83);
            _mapBuilder.PlaceBlock("snow", 37, 21, 83);

            // BACK TO ROWS (19)
            _mapBuilder.PlaceBlock("snow", 33, 19, 2);
            _mapBuilder.PlaceBlock("snow", 34, 19, 1);
            _mapBuilder.PlaceBlock("snow", 35, 19, 3);
            _mapBuilder.PlaceBlock("snow", 36, 19, 0);

            // ROW 20
            _mapBuilder.PlaceBlock("snow", 31, 20, 83);
            _mapBuilder.PlaceBlock("snow", 32, 20, 83);
            _mapBuilder.PlaceBlock("snow", 33, 20, 35);
            _mapBuilder.PlaceBlock("snow", 34, 20, 23);
            _mapBuilder.PlaceBlock("snow", 35, 20, 17);
            _mapBuilder.PlaceBlock("snow", 36, 20, 41);

            // ROW 21
            _mapBuilder.PlaceBlock("snow", 30, 21, 83);
            _mapBuilder.PlaceBlock("snow", 31, 21, 83);
            _mapBuilder.PlaceBlock("snow", 32, 21, 83);
            _mapBuilder.PlaceBlock("snow", 33, 21, 83);
            _mapBuilder.PlaceBlock("snow", 34, 21, 83);
            _mapBuilder.PlaceBlock("snow", 35, 21, 83);
            _mapBuilder.PlaceBlock("snow", 36, 21, 83);


            // GIRDER
            _mapBuilder.PlaceBlock("girder", 22, 12, 67);
            _mapBuilder.PlaceBlock("girder", 22, 13, 13);

            _mapBuilder.PlaceBlock("girder", 26, 14, 74);
            _mapBuilder.PlaceBlock("girder", 26, 15, 13);
            _mapBuilder.PlaceBlock("girder", 26, 16, 14);
            _mapBuilder.PlaceBlock("girder", 26, 17, 13);
            _mapBuilder.PlaceBlock("girder", 26, 18, 13);
            _mapBuilder.PlaceBlock("girder", 26, 19, 14);
            _mapBuilder.PlaceBlock("girder", 26, 20, 14);

            _mapBuilder.PlaceBlock("girder", 29, 16, 4);
            _mapBuilder.PlaceBlock("girder", 29, 17, 13);
            _mapBuilder.PlaceBlock("girder", 29, 18, 13);
            _mapBuilder.PlaceBlock("girder", 29, 19, 14);
            _mapBuilder.PlaceBlock("girder", 29, 20, 14);

            _mapBuilder.PlaceBlock("girder", 32, 17, 75);
            _mapBuilder.PlaceBlock("girder", 32, 18, 13);
            _mapBuilder.PlaceBlock("girder", 32, 19, 14);

            _mapBuilder.PlaceBlock("girder", 37, 15, 66);
            _mapBuilder.PlaceBlock("girder", 37, 16, 12);
            _mapBuilder.PlaceBlock("girder", 37, 17, 12);

            // MIDDLE PLATFORM

            _mapBuilder.PlaceBlock("girder", 14, 15, 22);
            _mapBuilder.PlaceBlock("girder", 15, 15, 1);
            _mapBuilder.PlaceBlock("girder", 16, 15, 25);
            _mapBuilder.PlaceBlock("girder", 17, 15, 3);
            _mapBuilder.PlaceBlock("girder", 18, 15, 4);

            _mapBuilder.PlaceBlock("girder", 14, 16, 4);
            _mapBuilder.PlaceBlock("girder", 15, 16, 6);
            _mapBuilder.PlaceBlock("girder", 16, 16, 7);
            _mapBuilder.PlaceBlock("girder", 17, 16, 7);
            _mapBuilder.PlaceBlock("girder", 18, 16, 4);

            _mapBuilder.PlaceBlock("girder", 7, 17, 75);
            _mapBuilder.PlaceBlock("girder", 8, 17, 3);
            _mapBuilder.PlaceBlock("girder", 9, 17, 6);
            _mapBuilder.PlaceBlock("girder", 10, 17, 7);
            _mapBuilder.PlaceBlock("girder", 11, 17, 1);
            _mapBuilder.PlaceBlock("girder", 12, 17, 2);
            _mapBuilder.PlaceBlock("girder", 13, 17, 6);
            _mapBuilder.PlaceBlock("girder", 14, 17, 7);
            _mapBuilder.PlaceBlock("girder", 15, 17, 4);

            _mapBuilder.PlaceBlock("girder", 7, 18, 4);
            _mapBuilder.PlaceBlock("girder", 8, 18, 6);
            _mapBuilder.PlaceBlock("girder", 9, 18, 3);
            _mapBuilder.PlaceBlock("girder", 10, 18, 7);
            _mapBuilder.PlaceBlock("girder", 11, 18, 7);
            _mapBuilder.PlaceBlock("girder", 12, 18, 7);
            _mapBuilder.PlaceBlock("girder", 13, 18, 6);
            _mapBuilder.PlaceBlock("girder", 14, 18, 8);
            _mapBuilder.PlaceBlock("girder", 15, 18, 4);

            _mapBuilder.PlaceBlock("girder", 14, 19, 13);
            _mapBuilder.PlaceBlock("girder", 14, 20, 14);
            _mapBuilder.PlaceBlock("girder", 14, 21, 4);

            _mapBuilder.PlaceBlock("girder", 15, 19, 14);
            _mapBuilder.PlaceBlock("girder", 15, 20, 13);
            _mapBuilder.PlaceBlock("girder", 15, 21, 4);

            // SPIKES
            _mapBuilder.PlaceBlock("upSpike", 21, 13, 1);
            _mapBuilder.PlaceBlock("upSpike", 23, 13, 1);

            _mapBuilder.PlaceBlock("upSpike", 24, 19, 1);
            _mapBuilder.PlaceBlock("upSpike", 25, 19, 1);

            _mapBuilder.PlaceBlock("upSpike", 27, 20, 1);
            _mapBuilder.PlaceBlock("upSpike", 28, 20, 1);

            _mapBuilder.PlaceBlock("upSpike", 31, 18, 1);

            _mapBuilder.PlaceBlock("upSpike", 30, 19, 1);
    */


        }
    }
}
