using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Data.World.WorldGen;
using SadConsole;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Screens
{
    internal class WorldScreen : ScreenSurface
    {
        public readonly Tilemap Tilemap;

        public WorldScreen(int width, int height) : base(width, height)
        {
            // Setup tilemap
            Tilemap = new Tilemap(width, height);

            // Setup a new surface matching with our tiles
            Surface = new CellSurface(width, height, Tilemap.Tiles);
        }

        public void Generate()
        {
            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out _);
        }
    }
}
