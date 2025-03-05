using Depths_of_Othaura.Data.Entities;
using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Data.World.WorldGen;
using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Represents the game world screen, displaying the tilemap.
    /// </summary>
    internal class WorldScreen : ScreenSurface
    {
        /// <summary>
        /// Gets the tilemap associated with this screen.
        /// </summary>
        public readonly Tilemap Tilemap;

        /// <summary>
        /// Manages the actors in the game world.
        /// </summary>
        public readonly ActorManager ActorManager;

        /// <summary>
        /// Gets the player character.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// The list of dungeon rooms generated during world generation.
        /// </summary>
        private IReadOnlyList<Rectangle> _dungeonRooms;

        /// <summary>
        /// Manages the field of view for the game.
        /// </summary>
        private readonly FOVManager _fovManager;

        /// <summary>
        /// Exposes the field of view manager.
        /// </summary>
        public FOVManager FovManager => _fovManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorldScreen"/> class.
        /// </summary>
        /// <param name="width">The width of the screen in cells.</param>
        /// <param name="height">The height of the screen in cells.</param>
        public WorldScreen(int width, int height) : base(width, height)
        {
            Tilemap = new Tilemap(width, height);
            Surface = new CellSurface(width, height, Tilemap.Tiles);

            ActorManager = new ActorManager();
            SadComponents.Add(ActorManager.EntityComponent);

            _fovManager = new FOVManager(this);

            // Set all tiles as "Not Seen"
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = Tilemap[x, y];
                    tile.IsVisible = false;
                    tile.HasBeenLit = false;  // Never seen before
                    tile.Foreground = Color.Black; // Initial color for unseen
                }
            }
        }


        /// <summary>
        /// Generates the initial world content.
        /// </summary>
        public void Generate()
        {
            // TODO: Implement character creation screen before generating the world.
            // TODO: Use RaceManager and ClassManager to populate race/class options.

            DungeonGenerator.Generate(Tilemap, 10, 4, 10, out _dungeonRooms);
            if (_dungeonRooms.Count == 0)
                throw new Exception("Faulty dungeon generation, no rooms!");
        }

        /// <summary>
        /// Creates the player character and adds it to the world.
        /// </summary>
        public void CreatePlayer()
        {
            Player = new Player(_dungeonRooms[0].Center, Tilemap);
            ActorManager.Add(Player);
            _fovManager.CalculateFOV(Player);
        }

        /// <summary>
        /// Toggles between ASCII mode and Tile mode and updates the map accordingly
        /// </summary>
        public void ToggleRenderMode()
        {
            // Change rendering modes.
            Constants.AsciiRenderMode = !Constants.AsciiRenderMode;
            //MessagesScreen.WriteLine($"Tile mode toggled: {(Constants.AsciiRenderMode ? "ASCII Mode" : "Tile Mode")}");
            System.Console.WriteLine($"Tile mode toggled: {(Constants.AsciiRenderMode ? "ASCII Mode" : "Tile Mode")}");

            Tilemap.UpdateGlyph();

            // Force the screen to refresh to reflect changes
            Surface.IsDirty = true;
        }

        /// <summary>
        /// Toggles Debug mode
        /// </summary>
        public void ToggleDebugMode()
        {
            Constants.DebugMode = !Constants.DebugMode;
            System.Console.WriteLine($"Debug mode toggled: {(Constants.DebugMode ? "On" : "Off")}");

            _fovManager.ToggleVisibility(Constants.DebugMode);

            // TODO: Add a red outline around the screen to show you are in debug.
        }

        /// <summary>
        /// Toggles the visibility of the entire map
        /// </summary>
        /// <param name="point"></param>
        /// <param name="isVisible"></param>
        public void SetTileVisibility(Point point, bool isVisible)
        {
            if (isVisible)
            {
                Surface.SetCellAppearance(point.X, point.Y, Tilemap[point.X, point.Y]);
            }
            else if (Tilemap[point.X, point.Y].HasBeenLit) // Previously seen but not in FOV
            {
                Surface.SetCellAppearance(point.X, point.Y, new ColoredGlyph(Color.DarkGray, Color.Black, Tilemap[point.X, point.Y].Glyph));
            }
            else // Completely unexplored
            {
                Surface.SetCellAppearance(point.X, point.Y, new ColoredGlyph(Color.Black, Color.Black, ' '));
            }
        }

    }
}