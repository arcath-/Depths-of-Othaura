using Depths_of_Othaura.Data.Actors;
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
        private IReadOnlyList<Rectangle> _dungeonRooms;

        public readonly Tilemap Tilemap;
        public readonly ActorManager ActorManager;

        public Player Player { get; private set; }

        public WorldScreen(int width, int height) : base(width, height)
        {
            // Setup tilemap
            Tilemap = new Tilemap(width, height);

            // Setup a new surface matching with our tiles
            Surface = new CellSurface(width, height, Tilemap.Tiles);

            // Add the entity component to the world screen, so we can track entities
            ActorManager = new ActorManager();
            SadComponents.Add(ActorManager.EntityComponent);
        }

        public void Generate()
        {
            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out _dungeonRooms);
            if (_dungeonRooms.Count == 0)
                throw new Exception("Faulty dungeon generation, no rooms!");
        }

        public void CreatePlayer()
        {
            Player = new Player { Position = _dungeonRooms[0].Center };
            ActorManager.Add(Player);
        }
    }
}
