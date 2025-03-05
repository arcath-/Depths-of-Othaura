using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: 

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Defines the different types of tiles in the game world.
    /// </summary>
    internal enum TileType
    {
        /// <summary>
        /// Represents an empty or undefined tile.
        /// </summary>
        None,

        /// <summary>
        /// A floor tile that can be walked on.
        /// </summary>
        Floor,

        /// <summary>
        /// A wall tile that blocks movement.
        /// </summary>
        Wall,

        /// <summary>
        /// A door tile that may be opened or closed.
        /// </summary>
        Door,

        /// <summary>
        /// A staircase leading downward to another level.
        /// </summary>
        StairsDown
    }
}
