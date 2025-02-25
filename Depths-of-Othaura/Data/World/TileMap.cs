using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.World
{
    internal class Tilemap
    {
        public readonly int Width;
        public readonly int Height;
        public readonly Tile[] Tiles;

        public Tilemap(int width, int height)
        {
            Width = width;
            Height = height;

            // Initialize base tiles
            Tiles = new Tile[Width * Height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tiles[Point.ToIndex(x, y, width)] = new Tile(x, y)
                    {
                        Obstruction = ObstructionType.FullyBlocked
                    };
                }
            }
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public Tile this[int x, int y, bool throwExceptionWhenOutOfBounds = true]
        {
            get => this[Point.ToIndex(x, y, Width), throwExceptionWhenOutOfBounds];
        }

        public Tile this[int index, bool throwExceptionWhenOutOfBounds = true]
        {
            get
            {
                var point = Point.FromIndex(index, Width);
                if (!InBounds(point.X, point.Y))
                {
                    if (throwExceptionWhenOutOfBounds)
                        throw new Exception($"Position {point} is out of bounds of the tilemap.");
                    return null;
                }
                return Tiles[index];
            }
        }

        public void Reset()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[Point.ToIndex(x, y, Width)].Clear();
                    Tiles[Point.ToIndex(x, y, Width)].Obstruction = ObstructionType.FullyBlocked;
                }
            }
        }
    }
}
