using SadConsole;
using SadRogue.Primitives;
using System;

// TODO: 

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Represents the main screen container, holding the world screen, player stats screen, and messages screen.
    /// </summary>
    internal class ScreenContainer : ScreenObject
    {
        // ========================= Static Fields =========================

        private static ScreenContainer _instance;

        /// <summary>
        /// Gets the singleton instance of the <see cref="ScreenContainer"/> class.
        /// </summary>
        public static ScreenContainer Instance => _instance ?? throw new Exception("ScreenContainer is not yet initialized.");

        // ========================= Properties =========================

        /// <summary>
        /// Gets the world screen.
        /// </summary>
        public WorldScreen World { get; }

        /// <summary>
        /// Gets the player stats screen.
        /// </summary>
        public ScreenSurface PlayerStats { get; }

        /// <summary>
        /// Gets the messages screen.
        /// </summary>
        public ScreenSurface Messages { get; }

        /// <summary>
        /// Provides a random number generator for the game.
        /// </summary>
        public Random Random { get; }

        // ========================= Constructor =========================

        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenContainer"/> class.
        /// </summary>
        /// <exception cref="Exception">Thrown if more than one instance of <see cref="ScreenContainer"/> is created.</exception>
        public ScreenContainer()
        {
            if (_instance != null)
                throw new Exception("Only one ScreenContainer instance can exist.");
            _instance = this;

            Random = new Random();

            // World screen
            World = new WorldScreen(Game.Instance.ScreenCellsX.PercentageOf(70), Game.Instance.ScreenCellsY);
            Children.Add(World);

            // Player stats screen
            PlayerStats = new ScreenSurface(Game.Instance.ScreenCellsX.PercentageOf(30), Game.Instance.ScreenCellsY.PercentageOf(60))
            {
                Position = new Point(World.Position.X + World.Width, World.Position.Y)
            };
            Children.Add(PlayerStats);

            // Messages screen
            Messages = new ScreenSurface(Game.Instance.ScreenCellsX.PercentageOf(30), Game.Instance.ScreenCellsY.PercentageOf(40))
            {
                Position = new Point(World.Position.X + World.Width, PlayerStats.Position.Y + PlayerStats.Height)
            };
            Children.Add(Messages);

            // Temporary for visualization of the surfaces
            PlayerStats.Fill(background: Color.Green);
            Messages.Fill(background: Color.Yellow);
        }
    }
}