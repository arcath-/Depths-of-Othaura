﻿using Depths_of_Othaura.Data.World.Configuration;
using SadConsole;

// TODO: 

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Represents a single tile in the game world.
    /// </summary>
    internal class Tile : ColoredGlyph
    {
        // ========================= Properties =========================

        /// <summary>
        /// Gets the X coordinate of the tile.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the Y coordinate of the tile.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Gets or sets the obstruction type of the tile.
        /// </summary>
        public ObstructionType Obstruction { get; set; }

        /// <summary>
        /// The Ascii ID of the Glyph
        /// </summary>
        public int AsciiID { get; set; }

        /// <summary>
        /// The Tiles ID of the Glyph
        /// </summary>
        public int TileID { get; set; }

        /// <summary>
        /// Gets if it HasBeenLit
        /// </summary>
        public bool HasBeenLit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the tile is visible.
        /// </summary>
        public new bool IsVisible { get; set; }

        private TileType _tileType;
        /// <summary>
        /// Gets or sets the type of the tile.
        /// </summary>
        public TileType Type
        {
            get => _tileType;
            set
            {
                if (_tileType != value)
                {
                    _tileType = value;
                    CopyFromConfiguration();
                    IsDirty = true;
                }
            }
        }

        // ========================= Constructors =========================

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class.
        /// </summary>
        /// <param name="x">The X coordinate of the tile.</param>
        /// <param name="y">The Y coordinate of the tile.</param>
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            Foreground = Color.Black;
            Background = Color.Black; // Start fully blacked out
            CopyFromConfiguration();
        }

        /// <summary>
        /// Initializes a tile based on its type.
        /// This constructor is primarily used for tile configuration.
        /// </summary>
        /// <param name="type">The tile type.</param>
        internal Tile(TileType type)
        {
            _tileType = type;
        }

        // ========================= Configuration =========================

        /// <summary>
        /// Copies the tile configuration settings from the configuration data.
        /// Updates the tile's appearance and obstruction settings.
        /// </summary>
        public void CopyFromConfiguration()
        {
            // Copy over the appearance from the configuration tile
            var configurationTile = TilesConfig.Get(Type);
            configurationTile.CopyAppearanceTo(this);

            // Explicitly set the obstruction type as it's not part of the appearance
            Obstruction = configurationTile.Obstruction;

            Glyph = (Constants.AsciiRenderMode ? configurationTile.AsciiID : configurationTile.TileID);

            // Store AsciiID and TileID for dynamic glyph switching
            AsciiID = configurationTile.AsciiID;
            TileID = configurationTile.TileID;

            // Set the default for tiles
            HasBeenLit = false;
            IsVisible = false;
        }

        /// <summary>
        /// Updates the glyph of the tile based on the current rendering mode.
        /// This allows dynamic switching between ASCII and Tile graphics.
        /// </summary>
        public void UpdateGlyph()
        {
            Glyph = (char)(Constants.AsciiRenderMode ? AsciiID : TileID);
        }
    }
}