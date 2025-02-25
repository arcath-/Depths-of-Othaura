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
        public int X { get; }
        public int Y { get; }
        public ObstructionType Obstruction { get; set; }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        private TileType _tileType;
        public TileType Type
        {
            get => _tileType;
            set
            {
                _tileType = value;

                // Copy over the appearance of the config tile to this tile on tile type change.
                var configurationTile = TilesConfig.Get(Type);
                configurationTile.CopyAppearanceTo(this);

                // We must set this one explicitly because its not part of the appearance, its our custom property.
                Obstruction = configurationTile.Obstruction;
            }
        }

        // Used for tile config initialization
        internal Tile(TileType type)
        {
            _tileType = type;
        }
    }

    

    

    

}


