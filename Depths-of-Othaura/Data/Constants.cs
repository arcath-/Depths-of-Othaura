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
        /// Toggle for Ascii/Tiles toggle. Default set to true.
        /// </summary>
        public static bool AsciiRenderMode = true;

        /// <summary>
        /// Toggle for Debug toggle. Default set to false.
        /// </summary>
        public static bool DebugMode = false;

        /// <summary>
        /// The width of the game screen in cells.
        /// </summary>
        public const int ScreenWidth = 80;

        /// <summary>
        /// The height of the game screen in cells.
        /// </summary>
        public const int ScreenHeight = 25;

        /// <summary>
        /// Field of View
        public const int PlayerFieldOfViewRadius = 6;
    }
}