using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;

namespace Celeste.Blocks.Rooms
{
public class RoomLoader
    {
        private MapBuilder _mapBuilder;
        private BlockFactory _blockFactory;

        public RoomLoader(MapBuilder mapBuilder, BlockFactory blockFactory)
        {
            _mapBuilder = mapBuilder;
            _blockFactory = blockFactory;
        }

        public void LoadRoom(string filePath)
        {
            // reset room
            _mapBuilder.ClearBlocks();

            // store and parse in array
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string[] parts = line.Split(',');

                // the format: (source, x-coord, y-coord, frame)
                if (parts.Length == 4)
                {
                    string source = parts[0].Trim();
                    int x = int.Parse(parts[1].Trim());
                    int y = int.Parse(parts[2].Trim());
                    int frame = int.Parse(parts[3].Trim());

                    // draw
                    _mapBuilder.PlaceBlock(source, x, y, frame);
                }
            }
        }
    }
}