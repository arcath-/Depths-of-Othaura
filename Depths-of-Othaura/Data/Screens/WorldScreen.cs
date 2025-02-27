﻿using Depths_of_Othaura.Data.Entities;
using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Data.World.WorldGen;
using GoRogue.Pathing;
using SadConsole;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
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
        public readonly FastAStar Pathfinder;

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

            // Setup the pathfinder
            Pathfinder = new FastAStar(new LambdaGridView<bool>(Tilemap.Width, Tilemap.Height, (a) => !BlocksMovement(Tilemap[a.X, a.Y].Obstruction)), Distance.Manhattan);
        }

        public void Generate()
        {
            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out _dungeonRooms);
            if (_dungeonRooms.Count == 0)
                throw new Exception("Faulty dungeon generation, no rooms!");
        }

        public void CreatePlayer()
        {
            Player = new Player(_dungeonRooms[0].Center);
            ActorManager.Add(Player);

            // Initial player stats draw
            ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        // Creates NPCs by looping through the generated rooms.
        public void CreateNpcs()
        {
            const int maxNpcPerRoom = 2;
            foreach (var room in _dungeonRooms)
            {
                // Define how many npcs will be in this room
                var npcs = ScreenContainer.Instance.Random.Next(0, maxNpcPerRoom + 1);

                // All positions within the room except the perimeter positions and the player position
                var validPositions = room.Positions()
                    .Except(room.PerimeterPositions().Append(Player.Position))
                    .ToList();

                for (int i = 0; i < npcs; i++)
                {
                    // Select a random position from the list
                    var randomPosition = validPositions[ScreenContainer.Instance.Random.Next(0, validPositions.Count)];

                    // Create the goblin npc with the given position and add it to the actor manager
                    var goblin = new Goblin(randomPosition);
                    ActorManager.Add(goblin);

                    // Make sure we don't spawn another at this position
                    validPositions.Remove(randomPosition);
                }
            }

            // Update the visibility of actors
            ScreenContainer.Instance.World.ActorManager.UpdateVisibility();
        }

        // Checking for movement blocking - any obstruction that is “MovementBlocked” or
        // “FullyBlocked” cannot be moved onto, anythign else does not block movement.
        private static bool BlocksMovement(ObstructionType obstructionType)
        {
            return obstructionType switch
            {
                ObstructionType.MovementBlocked or ObstructionType.FullyBlocked => true,
                _ => false,
            };
        }
    }
}
