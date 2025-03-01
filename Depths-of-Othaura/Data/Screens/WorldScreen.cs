using Depths_of_Othaura.Data.Entities;
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

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Represents the main game world screen where gameplay takes place.
    /// Manages the tilemap, entities, and world generation.
    /// </summary>
    internal class WorldScreen : ScreenSurface
    {
        /// <summary>
        /// The tilemap representing the game's world.
        /// </summary>
        public readonly Tilemap Tilemap;

        /// <summary>
        /// Manages actors (players and NPCs) within the game world.
        /// </summary>
        public readonly ActorManager ActorManager;

        /// <summary>
        /// The pathfinding system used for AI navigation.
        /// </summary>
        public readonly FastAStar Pathfinder;

        /// <summary>
        /// The player character within the game world.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldScreen"/> class.
        /// </summary>
        /// <param name="width">The width of the world screen.</param>
        /// <param name="height">The height of the world screen.</param>
        public WorldScreen(int width, int height) : base(width, height)
        {
            // Setup tilemap
            Tilemap = new Tilemap(width, height);

            // Create a surface that matches the tilemap dimensions
            Surface = new CellSurface(width, height, Tilemap.Tiles);

            // Initialize entity manager to track actors
            ActorManager = new ActorManager();
            SadComponents.Add(ActorManager.EntityComponent);

            // Setup pathfinding system
            Pathfinder = new FastAStar(
                new LambdaGridView<bool>(
                    Tilemap.Width, Tilemap.Height,
                    (a) => !BlocksMovement(Tilemap[a.X, a.Y].Obstruction)),
                Distance.Manhattan);
        }

        /// <summary>
        /// Generates a new dungeon layout and places the player and NPCs in the world.
        /// </summary>
        public void Generate()
        {
            Surface.Clear();

            // Generate a new dungeon layout
            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out var dungeonRooms);
            if (dungeonRooms.Count == 0)
                throw new Exception("Faulty dungeon generation, no rooms!");

            var spawnPosition = dungeonRooms[0].Center;

            if (Player == null)
            {
                // Initialize the player if they do not exist
                CreatePlayer(spawnPosition);
            }
            else
            {
                // Move player to new spawn position
                Player.Position = spawnPosition;

                // Fully explore the new area since teleportation may cause missing tiles
                Player.ExploreCurrentFov();
            }

            // Spawn NPCs in the dungeon
            CreateNpcs(dungeonRooms);
        }

        /// <summary>
        /// Creates and adds the player to the game world.
        /// </summary>
        /// <param name="position">The starting position of the player.</param>
        public void CreatePlayer(Point position)
        {
            Player = new Player(position);
            ActorManager.Add(Player);

            // Initial update of player stats display
            ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        /// <summary>
        /// Generates and places NPCs throughout the dungeon.
        /// </summary>
        /// <param name="dungeonRooms">A list of dungeon rooms where NPCs may spawn.</param>
        public void CreateNpcs(IReadOnlyList<Rectangle> dungeonRooms)
        {
            // Remove old NPCs but keep the player
            ScreenContainer.Instance.World.ActorManager.Clear();
            ScreenContainer.Instance.World.ActorManager.Add(Player);

            // Set NPC levels within a range of the player's level
            var levelRequirement = (
                min: Math.Max(1, Player.Stats.Level - 2),
                max: Player.Stats.Level + 1
            );

            const int maxNpcPerRoom = 2;
            foreach (var room in dungeonRooms)
            {
                // Determine the number of NPCs in this room
                var npcs = ScreenContainer.Instance.Random.Next(0, maxNpcPerRoom + 1);

                // Get valid spawn positions excluding room perimeter and player's position
                var validPositions = room.Positions()
                    .Except(room.PerimeterPositions().Append(Player.Position))
                    .ToList();

                for (int i = 0; i < npcs; i++)
                {
                    // Select a random spawn position
                    var randomPosition = validPositions[
                        ScreenContainer.Instance.Random.Next(0, validPositions.Count)
                    ];

                    // Create a goblin NPC at the selected position
                    var goblin = new Goblin(randomPosition);
                    goblin.Stats.SetLevel(ScreenContainer.Instance.Random.Next(
                        levelRequirement.min, levelRequirement.max + 1));

                    ActorManager.Add(goblin);

                    // Ensure NPCs do not spawn on top of each other
                    validPositions.Remove(randomPosition);
                }
            }

            // Update visibility of actors in the world
            ScreenContainer.Instance.World.ActorManager.UpdateVisibility();
        }

        /// <summary>
        /// Determines whether a given obstruction type blocks movement.
        /// </summary>
        /// <param name="obstructionType">The type of obstruction.</param>
        /// <returns>Returns <c>true</c> if movement is blocked; otherwise, <c>false</c>.</returns>
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
