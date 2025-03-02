using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World.Configuration;
using SadConsole;
using SadRogue.Primitives;
using System.Runtime.InteropServices;

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Represents an individual tile in the game world.
    /// </summary>
    internal class Tile : ColoredGlyph
    {
        /// <summary>
        /// The X-coordinate of the tile in the world.
        /// </summary>
        public int X { get; }

        /// <summary>
        /// The Y-coordinate of the tile in the world.
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// The obstruction type of the tile, which determines movement and visibility rules.
        /// </summary>
        public ObstructionType Obstruction { get; set; }

        private TileType _tileType;

        /// <summary>
        /// The type of tile, determining its properties and appearance.
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

        private bool _inFov;

        /// <summary>
        /// Indicates whether the tile is currently in the player's field of view.
        /// Updates foreground and background colors when toggled.
        /// </summary>
        public bool InFov
        {
            get => _inFov;
            set
            {
                if (_inFov != value)
                {
                    _inFov = value;

                    if (_inFov)
                    {
                        Foreground = _seenForeground;
                        Background = _seenBackground;
                    }
                    else
                    {
                        Foreground = _unseenForeground;
                        Background = _unseenBackground;
                    }
                }
            }
        }

        private Color _seenForeground, _seenBackground, _unseenForeground, _unseenBackground;

        /// <summary>
        /// The ASCII character ID for this tile.
        /// This is used when rendering in ASCII mode.
        /// </summary>
        public int AsciiID { get; private set; }

        /// <summary>
        /// The Tile ID for this tile.
        /// This is used when rendering in Tile mode.
        /// </summary>
        public int TileID { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tile"/> class at a specified position.
        /// </summary>
        /// <param name="x">The X-coordinate of the tile.</param>
        /// <param name="y">The Y-coordinate of the tile.</param>
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            IsVisible = false;
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

        /// <summary>
        /// Copies the tile configuration settings from the configuration data.
        /// Updates the tile's appearance and obstruction settings.
        /// </summary>
        /// <summary>
        /// Copies the tile configuration settings from the configuration data.
        /// Updates the tile's appearance, obstruction settings, and glyph.
        /// </summary>
        private void CopyFromConfiguration()
        {
            // Copy over the appearance from the configuration tile
            var configurationTile = TilesConfig.Get(Type);
            configurationTile.CopyAppearanceTo(this);

            // Explicitly set the obstruction type
            Obstruction = configurationTile.Obstruction;

            // Store AsciiID and TileID for dynamic glyph switching
            AsciiID = configurationTile.AsciiID;
            TileID = configurationTile.TileID;

            // Debug log to check assigned IDs
            System.Console.WriteLine($"Tile[{X}, {Y}] Config Loaded - AsciiID: {AsciiID}, TileID: {TileID}");

            // Update the glyph dynamically based on the current render mode
            UpdateGlyph();

            // Set colors for field of view transitions
            SetColorsForFOV();
        }


        /// <summary>
        /// Updates the glyph of the tile based on the current rendering mode.
        /// This allows dynamic switching between ASCII and Tile graphics.
        /// </summary>
        public void UpdateGlyph()
        {
            Glyph = (char)(Constants.UseAsciiMode ? AsciiID : TileID);
        }


        /// <summary>
        /// Sets the colors for the tile when seen and unseen in the field of view.
        /// </summary>
        private void SetColorsForFOV()
        {
            _seenForeground = Foreground;
            _seenBackground = Background;
            _unseenForeground = Color.Lerp(_seenForeground, Color.Black, 0.5f);
            _unseenBackground = Color.Lerp(_seenBackground, Color.Black, 0.5f);
        }

        
    }
}
