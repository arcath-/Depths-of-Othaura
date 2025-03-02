using SadConsole;
using SadRogue.Primitives;
using System;
using GoRogue.FOV;
using SadRogue.Primitives.GridViews;

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Container for all screen objects used by the roguelike game.
    /// Manages the world screen, player stats screen, and message log.
    /// </summary>
    internal class ScreenContainer : ScreenObject
    {
        private static ScreenContainer _instance;

        /// <summary>
        /// Gets the singleton instance of <see cref="ScreenContainer"/>.
        /// </summary>
        /// <exception cref="Exception">Thrown if the instance has not yet been initialized.</exception>
        public static ScreenContainer Instance => _instance ?? throw new Exception("ScreenContainer is not yet initialized.");

        /// <summary>
        /// Represents the world screen where the game takes place.
        /// </summary>
        public WorldScreen World { get; }

        /// <summary>
        /// Represents the screen displaying the player's stats.
        /// </summary>
        public PlayerStatsScreen PlayerStats { get; }

        /// <summary>
        /// Represents the message log screen displaying game messages.
        /// </summary>
        public MessagesScreen Messages { get; }

        /// <summary>
        /// Provides a random number generator for various game mechanics.
        /// </summary>
        public Random Random { get; }

        /// <summary>
        /// Represents a full map.
        /// </summary>
        public IFOV FullMapFOV { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenContainer"/> class.
        /// Ensures only one instance of the container exists.
        /// </summary>
        /// <exception cref="Exception">Thrown if an instance of <see cref="ScreenContainer"/> already exists.</exception>
        public ScreenContainer()
        {
            if (_instance != null)
                throw new Exception("Only one ScreenContainer instance can exist.");
            _instance = this;

            Random = new Random();

            // World screen - 70% of width and 70% of height.
            World = new WorldScreen(Game.Instance.ScreenCellsX.PercentageOf(70), Game.Instance.ScreenCellsY.PercentageOf(70));
            Children.Add(World);

            //FullMap Visibility
            FullMapFOV = new RecursiveShadowcastingFOV(new LambdaGridView<bool>(World.Tilemap.Width, World.Tilemap.Height, (point) => true));
            FullMapFOV.Calculate(new Point(0, 0), Math.Max(World.Tilemap.Width, World.Tilemap.Height)); // Calculate from arbitrary point with very large radius to make sure it will be complete.

            // Player stats screen - 30% width and 100% height.
            PlayerStats = new PlayerStatsScreen(Game.Instance.ScreenCellsX.PercentageOf(30), Game.Instance.ScreenCellsY)
            {
                Position = new Point(World.Position.X + World.Width, World.Position.Y)
            };
            Children.Add(PlayerStats);

            // Messages screen - 70% width and 30% height.
            Messages = new MessagesScreen(Game.Instance.ScreenCellsX.PercentageOf(70), Game.Instance.ScreenCellsY.PercentageOf(30))
            {
                Position = new Point(World.Position.X, World.Height)
            };
            Children.Add(Messages);
        }
    }
}