using Depths_of_Othaura.Data.World.Configuration;
using static Depths_of_Othaura.Data.World.Configuration.TilesConfig;
using SadRogue.Primitives;
using System;
using static Microsoft.Xna.Framework.Graphics.SpriteFont;
using static SadConsole.Readers.Playscii;

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Represents the game's tile-based map, storing tile data and providing access methods.
    /// </summary>
    internal class Tilemap
    {
        /// <summary>
        /// The width of the tilemap.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The height of the tilemap.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The collection of tiles that make up the tilemap.
        /// </summary>
        public readonly Tile[] Tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tilemap"/> class with a specified width and height.
        /// </summary>
        /// <param name="width">The width of the tilemap.</param>
        /// <param name="height">The height of the tilemap.</param>
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

        /// <summary>
        /// Gets the tile at the specified (x, y) position.
        /// </summary>
        /// <param name="x">The X-coordinate of the tile.</param>
        /// <param name="y">The Y-coordinate of the tile.</param>
        /// <param name="throwExceptionWhenOutOfBounds">Determines whether an exception is thrown when accessing an out-of-bounds tile.</param>
        /// <returns>The tile at the specified position.</returns>
        public Tile this[int x, int y, bool throwExceptionWhenOutOfBounds = true]
        {
            get => this[Point.ToIndex(x, y, Width), throwExceptionWhenOutOfBounds];
        }

        /// <summary>
        /// Gets the tile at the specified index in the tile array.
        /// </summary>
        /// <param name="index">The index position in the tile array.</param>
        /// <param name="throwExceptionWhenOutOfBounds">Determines whether an exception is thrown when accessing an out-of-bounds tile.</param>
        /// <returns>The tile at the specified index.</returns>
        /// <exception cref="Exception">Thrown if the tile index is out of bounds and exception throwing is enabled.</exception>
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

        /// <summary>
        /// Determines whether the specified (x, y) position is within the bounds of the tilemap.
        /// </summary>
        /// <param name="x">The X-coordinate to check.</param>
        /// <param name="y">The Y-coordinate to check.</param>
        /// <returns>Returns <c>true</c> if the position is within bounds; otherwise, <c>false</c>.</returns>
        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        /// <summary>
        /// Resets all tiles in the tilemap to their default state.
        /// </summary>
        public void Reset()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = Point.ToIndex(x, y, Width);
                    Tiles[index].Type = TileType.None;
                    Tiles[index].InFov = false;
                    Tiles[index].IsVisible = false;
                    Tiles[index].Obstruction = ObstructionType.FullyBlocked;
                    Tiles[index].Clear();
                }
            }
        }

        /// <summary>
        /// Updates all tile glyphs based on the current rendering mode.
        /// </summary>
        /// <summary>
        /// Updates all tile glyphs based on the current rendering mode.
        /// </summary>
        /// <summary>
        /// Updates all tile glyphs based on the current rendering mode.
        /// </summary>
        public void UpdateGlyph()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int index = Point.ToIndex(x, y, Width);
                    var tile = Tiles[index];

                    // Debug log to check previous glyph
                    System.Console.WriteLine($"Tile[{x}, {y}] before update: {tile.Glyph}");

                    // Update glyph dynamically based on the current mode
                    tile.UpdateGlyph();

                    // Debug log to verify update
                    System.Console.WriteLine($"Tile[{x}, {y}] after update: {tile.Glyph}");
                }
            }

            System.Console.WriteLine("Tile glyphs updated.");
        }




    }
}
