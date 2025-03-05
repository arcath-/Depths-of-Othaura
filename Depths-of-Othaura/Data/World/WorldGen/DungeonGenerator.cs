using Depths_of_Othaura.Data.Screens;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: Implement non-square room generation (caves).
// TODO: Implement different level types (dungeon, town, tower).
// TODO: Implement non-linear tunnels (digging toward the closest point of the destination room, rather than a straight line).
// TODO: Implement room weighting. So a small % chance for a special room or a set piece.
// TODO: Implement LevelGenerator class with different subclasses for each level type (e.g., DungeonLevelGenerator, TownLevelGenerator, TowerLevelGenerator).

namespace Depths_of_Othaura.Data.World.WorldGen
{
    /// <summary>
    /// Provides methods for generating dungeon maps.
    /// </summary>
    internal static class DungeonGenerator
    {
        // Sometimes our random position(s) won't work, so we need a few attempts
        private const int MaxAttempts = 100;
        // Do you want doors in every room or only a smaller percentage, 60% seems nice
        private const int ChanceForDoorPlacement = 60;

        /// <summary>
        /// Generates a dungeon map within the given tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to generate the dungeon in.</param>
        /// <param name="maxRooms">The maximum number of rooms to generate.</param>
        /// <param name="minRoomSize">The minimum size of a room.</param>
        /// <param name="maxRoomSize">The maximum size of a room.</param>
        /// <param name="rooms">A list of rectangles representing the generated rooms.</param>
        public static void Generate(Tilemap tilemap, int maxRooms, int minRoomSize, int maxRoomSize, out IReadOnlyList<Rectangle> dungeonRooms)
        {
            tilemap.Reset();

            var random = ScreenContainer.Instance.Random;
            var rooms = new List<Rectangle>();
            dungeonRooms = rooms;

            const int borderSize = 2;

            // Generate rooms
            for (int attempts = 0; attempts < MaxAttempts; attempts++)
            {
                if (rooms.Count == maxRooms) break;

                int width = random.Next(minRoomSize, maxRoomSize);
                int height = random.Next(minRoomSize, maxRoomSize);

                // Exclude border tiles so we can set walls properly + keep an empty space as the border
                int x = random.Next(borderSize, tilemap.Width - width - borderSize);
                int y = random.Next(borderSize, tilemap.Height - height - borderSize);

                Rectangle newRoom = new(x, y, width, height);

                // Check for overlap with existing rooms
                if (!rooms.Any(r => r.Intersects(newRoom)))
                {
                    rooms.Add(newRoom);
                    CarveRoom(tilemap, newRoom);
                }
            }


            // Connect rooms with tunnels
            for (int i = 1; i < rooms.Count; i++)
            {
                Rectangle roomA = rooms[i - 1];
                Rectangle roomB = rooms[i];

                CarveTunnel(tilemap, roomA.Center, roomB.Center);
            }

            AddWalls(tilemap);
            AddDoors(tilemap, rooms);
        }

        /// <summary>
        /// Carves a room into the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to carve the room into.</param>
        /// <param name="room">The rectangle representing the room.</param>
        private static void CarveRoom(Tilemap tilemap, Rectangle room)
        {
            for (int x = room.X; x < room.X + room.Width; x++)
            {
                for (int y = room.Y; y < room.Y + room.Height; y++)
                {
                    // Set floor tile
                    tilemap[x, y].Type = TileType.Floor;
                    tilemap[x, y].Foreground = Color.Black; // Add this line
                }
            }
        }

        /// <summary>
        /// Carves a tunnel between two points in the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to carve the tunnel in.</param>
        /// <param name="start">The starting point of the tunnel.</param>
        /// <param name="end">The ending point of the tunnel.</param>
        private static void CarveTunnel(Tilemap tilemap, Point start, Point end)
        {
            Point current = start;

            while (current.X != end.X)
            {
                // Set floor tile
                tilemap[current.X, current.Y].Type = TileType.Floor;
                tilemap[current.X, current.Y].Foreground = Color.Black;  //Add This line
                current = new Point(current.X + (current.X < end.X ? 1 : -1), current.Y);
            }

            while (current.Y != end.Y)
            {
                // Set floor tile
                tilemap[current.X, current.Y].Type = TileType.Floor;
                tilemap[current.X, current.Y].Foreground = Color.Black; // Add this line
                current = new Point(current.X, current.Y + (current.Y < end.Y ? 1 : -1));
            }
        }

        /// <summary>
        /// Adds walls around the floor tiles in the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to add walls to.</param>
        private static void AddWalls(Tilemap tilemap)
        {
            for (int x = 0; x < tilemap.Width; x++)
            {
                for (int y = 0; y < tilemap.Height; y++)
                {
                    // Check if the current tile is a floor
                    if (tilemap[x, y].Type == TileType.Floor)
                    {
                        // Get neighbors of the current tile
                        var neighbors = new Point(x, y).GetNeighborPoints(true);
                        foreach (var neighbor in neighbors)
                        {
                            // If a neighbor is inbounds and not a floor, set it as a wall
                            if (tilemap.InBounds(neighbor.X, neighbor.Y) && tilemap[neighbor.X, neighbor.Y].Type != TileType.Floor)
                            {
                                // Set wall glyph and make obstruction fully blocked
                                tilemap[neighbor.X, neighbor.Y].Type = TileType.Wall;
                                tilemap[neighbor.X, neighbor.Y].Foreground = Color.Black; // Add this line
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Adds doors to the rooms in the tilemap.
        /// </summary>
        /// <param name="tilemap">The tilemap to add doors to.</param>
        /// <param name="rooms">The list of rooms to add doors to.</param>
        private static void AddDoors(Tilemap tilemap, List<Rectangle> rooms)
        {
            foreach (var room in rooms)
            {
                var wallPositions = room.Expand(1, 1).PerimeterPositions();

                // Check if tile is a floor and if both horizontal / vertical neighbors are a match with eachother
                foreach (var position in wallPositions)
                {
                    if (!tilemap.InBounds(position.X, position.Y)) continue;

                    // If tile is a floor
                    if (tilemap[position.X, position.Y].Type == TileType.Floor)
                    {
                        // Get directional neighbors
                        var north = tilemap[position.X, position.Y + 1, false];
                        var south = tilemap[position.X, position.Y - 1, false];
                        var east = tilemap[position.X + 1, position.Y, false];
                        var west = tilemap[position.X - 1, position.Y, false];

                        if ((north?.Type == TileType.Floor && south?.Type == TileType.Floor && east?.Type == TileType.Wall && west?.Type == TileType.Wall) ||
                            (north?.Type == TileType.Wall && south?.Type == TileType.Wall && east?.Type == TileType.Floor && west?.Type == TileType.Floor))
                        {
                            // % chance to place a door
                            if (ScreenContainer.Instance.Random.Next(100) < ChanceForDoorPlacement)
                            {
                                tilemap[position.X, position.Y].Type = TileType.Door;
                                tilemap[position.X, position.Y].Foreground = Color.Black; // Add this line
                            }
                        }
                    }
                }
            }
        }


    }
}