using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.Entities;
using Depths_of_Othaura.Data.Screens;
using SadRogue.Primitives;
using System.Linq;
using GoRogue.Pathing;

namespace Depths_of_Othaura.Data.Logic
{
    /// <summary>
    /// Handles the main game logic, including player movement, combat, and AI pathfinding.
    /// </summary>
    internal static class GameLogic
    {
        /// <summary>
        /// Reference to the player instance.
        /// </summary>
        private static Player Player => ScreenContainer.Instance.World.Player;

        /// <summary>
        /// Reference to the actor manager, which manages all actors in the game.
        /// </summary>
        private static ActorManager ActorManager => ScreenContainer.Instance.World.ActorManager;

        /// <summary>
        /// Reference to the pathfinding system used for AI movement.
        /// </summary>
        private static FastAStar Pathfinder => ScreenContainer.Instance.World.Pathfinder;

        /// <summary>
        /// Executes a game tick when the player attempts to move.
        /// This function determines interactions such as combat, movement, and AI behavior.
        /// </summary>
        /// <param name="intendedPosition">The position the player attempted to move to.</param>
        internal static void Tick(Point intendedPosition)
        {
            if (HandleStairs(intendedPosition))
                return;

            HandlePathfindingAndCombat(intendedPosition);
        }

        /// <summary>
        /// Handles logic related to stairs in the dungeon, such as transitioning to a new floor.
        /// </summary>
        /// <param name="intendedPosition">The position the player attempted to move to.</param>
        /// <returns>Returns <c>true</c> if the player moved onto a stair tile; otherwise, <c>false</c>.</returns>
        private static bool HandleStairs(Point intendedPosition)
        {
            var hasMoved = Player.Position == intendedPosition;
            if (!hasMoved) return false;

            // Check if we moved onto a stairs-down tile
            var tile = ScreenContainer.Instance.World.Tilemap[intendedPosition.ToIndex(ScreenContainer.Instance.World.Tilemap.Width)];
            if (tile.Type == World.TileType.StairsDown)
            {
                // Generate a new world/dungeon when going downstairs
                ScreenContainer.Instance.World.Generate();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Handles AI pathfinding and melee combat interactions for NPCs.
        /// </summary>
        /// <param name="intendedPosition">The position the player attempted to move to.</param>
        private static void HandlePathfindingAndCombat(Point intendedPosition)
        {
            var hasMoved = Player.Position == intendedPosition;
            var npcAtIntendedPosition = ActorManager.Get(intendedPosition);

            if (!hasMoved && npcAtIntendedPosition != null)
            {
                // The player didn't move but an actor is at the intended position, so we attempted to move into the actor
                // This counts as an attack from the player to the actor
                MeleeCombatLogic.Attack(Player, npcAtIntendedPosition);
            }

            // Calculate AI movement for each NPC in the player's field of view
            var npcsInFov = Player.FieldOfView.CurrentFOV
                .Where(ActorManager.ExistsAt)
                .Select(ActorManager.Get)
                .Where(a => a != Player)
                .ToArray();

            foreach (var npcInFov in npcsInFov)
            {
                var shortestPath = Pathfinder.ShortestPath(npcInFov.Position, Player.Position);
                if (shortestPath == null || shortestPath.Length == 0) continue;

                // Move NPC towards the player
                var nextStep = shortestPath.GetStep(0);

                if (!npcInFov.Move(nextStep.X, nextStep.Y))
                {
                    // If NPC ran into the player, initiate combat
                    if (nextStep == Player.Position)
                        MeleeCombatLogic.Attack(npcInFov, Player);
                }
            }
        }
    }
}
