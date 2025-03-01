﻿using Depths_of_Othaura.Data.World.Configuration;
using SadConsole;
using SadRogue.Primitives;

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
        private void CopyFromConfiguration()
        {
            // Copy over the appearance from the configuration tile
            var configurationTile = TilesConfig.Get(Type);
            configurationTile.CopyAppearanceTo(this);

            // Explicitly set the obstruction type as it's not part of the appearance
            Obstruction = configurationTile.Obstruction;

            // Set colors for field of view transitions
            SetColorsForFOV();
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
