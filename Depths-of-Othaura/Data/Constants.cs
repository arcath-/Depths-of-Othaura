namespace Depths_of_Othaura
{

    /// <summary>
    /// Provides easy access to all constant game information.
    /// </summary
    internal static class Constants
    {
        /// <summary>
        /// The title of the game.
        /// </summary>
        public const string GameTitle = "Depths of Othaura";

        /// <summary>
        /// Path to the primary font used in the game.
        /// </summary>
        public const string Font = "Data/Assets/_othauraTest_32x32.font";

        /// <summary>
        /// Path to the tile configuration JSON file.
        /// </summary>
        public const string TileConfiguration = "Data/World/Configuration/tiles.json";

        /// <summary>
        /// Default radius for the player's field of view.
        /// </summary>
        public const int PlayerFieldOfViewRadius = 6;

        /// <summary>
        /// Toggle for Ascii/Tiles toggle. Default set to true.
        /// </summary>
        public static bool AsciiRenderMode = true;

        /// <summary>
        /// Toggle for Debug toggle. Default set to false.
        /// </summary>
        public static bool DebugMode = false;
    }
}