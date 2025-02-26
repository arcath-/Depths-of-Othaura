using Depths_of_Othaura.Data.World.Configuration;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.World
{
    internal class Tile : ColoredGlyph
    {
        private bool _inFov;
        private TileType _tileType;


        public int X { get; }
        public int Y { get; }
        public ObstructionType Obstruction { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
            IsVisible = false;
            CopyFromConfiguration();
        }


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

        // Used for tile config initialization
        internal Tile(TileType type)
        {
            _tileType = type;
        }

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

        private void SetColorsForFOV()
        {
            _seenForeground = Foreground;
            _seenBackground = Background;
            _unseenForeground = Color.Lerp(_seenForeground, Color.Black, 0.5f);
            _unseenBackground = Color.Lerp(_seenBackground, Color.Black, 0.5f);
        }

        private void CopyFromConfiguration()
        {
            // Copy over the appearance of the config tile to this tile on tile type change.
            var configurationTile = TilesConfig.Get(Type);
            configurationTile.CopyAppearanceTo(this);

            // We must set this one explicitly because its not part of the appearance, its our custom property.
            Obstruction = configurationTile.Obstruction;

            // Set colors for FOV
            SetColorsForFOV();
        }



    }

    

    

    

}


