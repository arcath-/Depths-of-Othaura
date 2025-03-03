using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadRogue.Primitives;

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Represents the game world's tile map.
    /// </summary>
    internal class Tilemap
    {
        /// <summary>
        /// Gets the width of the tilemap.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// Gets the height of the tilemap.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// The Ascii ID of the Glyph
        /// </summary>
        public int AsciiID { get; set; }

        /// <summary>
        /// The Tiles ID of the Glyph
        /// </summary>
        public int TileID { get; set; }

        /// <summary>
        /// Gets the array of tiles in the tilemap.
        /// </summary>
        public readonly Tile[] Tiles;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tilemap"/> class.
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
        /// Gets the tile at the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        /// <param name="throwExceptionWhenOutOfBounds">If <c>true</c>, throws an exception if the coordinates are out of bounds.</param>
        /// <returns>The tile at the specified coordinates, or <c>null</c> if out of bounds and <paramref name="throwExceptionWhenOutOfBounds"/> is <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown if the coordinates are out of bounds and <paramref name="throwExceptionWhenOutOfBounds"/> is <c>true</c>.</exception>
        public Tile this[int x, int y, bool throwExceptionWhenOutOfBounds = true]
        {
            get => this[Point.ToIndex(x, y, Width), throwExceptionWhenOutOfBounds];
        }

        /// <summary>
        /// Gets the tile at the specified index.
        /// </summary>
        /// <param name="index">The index of the tile.</param>
        /// <param name="throwExceptionWhenOutOfBounds">If <c>true</c>, throws an exception if the index is out of bounds.</param>
        /// <returns>The tile at the specified index, or <c>null</c> if out of bounds and <paramref name="throwExceptionWhenOutOfBounds"/> is <c>false</c>.</returns>
        /// <exception cref="Exception">Thrown if the index is out of bounds and <paramref name="throwExceptionWhenOutOfBounds"/> is <c>true</c>.</exception>
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
        /// Checks if the given coordinates are within the bounds of the tilemap.
        /// </summary>
        /// <param name="x">The X coordinate to check.</param>
        /// <param name="y">The Y coordinate to check.</param>
        /// <returns><c>true</c> if the coordinates are within bounds; otherwise, <c>false</c>.</returns>
        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < Width && y < Height;
        }

        /// <summary>
        /// Resets the tilemap by clearing each tile and setting its obstruction type to <see cref="ObstructionType.FullyBlocked"/>.
        /// </summary>
        public void Reset()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tiles[Point.ToIndex(x, y, Width)].Clear();
                    Tiles[Point.ToIndex(x, y, Width)].Obstruction = ObstructionType.FullyBlocked;
                    //Tiles[Point.ToIndex(x, y, Width)].Glyph = (char)(Constants.AsciiRenderMode ? AsciiID : TileID);
                }
            }
        }

        /// <summary>
        /// Updates all tile glyphs based on the current rendering mode.
        /// </summary>
        public void UpdateGlyph()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    //update logic
                    int index = Point.ToIndex(x, y, Width);

                    // Ensure we reference the correct tile object
                    var tile = Tiles[index];

                    // Debug log to check previous glyph
                    //System.Console.WriteLine($"Tile[{x}, {y}] before update: {tile.Glyph}");

                    // Update glyph dynamically based on the current mode
                    tile.UpdateGlyph();
                    
                    // Debug log to verify update
                    //System.Console.WriteLine($"Tile[{x}, {y}] after update: {tile.Glyph}");
                }
            }

            //System.Console.WriteLine("Tile glyphs updated.");
        }
    }
}