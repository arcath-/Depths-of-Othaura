// TODO: 

namespace Depths_of_Othaura
{
    /// <summary>
    /// Provides easy access to all constant game information.
    /// </summary>
    internal static class Constants
    {
        // ========================= Game Information =========================

        /// <summary>
        /// The title of the game.
        /// </summary>
        public const string GameTitle = "Depths of Othaura";

        // ========================= File Paths =========================

        /// <summary>
        /// Path to the primary font used in the game.
        /// </summary>
        public const string Font = "Data/Assets/_othauraTest_32x32.font";

        /// <summary>
        /// Path to the tile configuration JSON file.
        /// </summary>
        public const string TileConfiguration = "Data/World/Configuration/tiles.json";

        // ========================= Game Settings =========================

        /// <summary>
        /// Toggle for Ascii/Tiles toggle. Default set to true.
        /// </summary>
        public static bool AsciiRenderMode = true;

        /// <summary>
        /// Toggle for Debug toggle. Default set to false.
        /// </summary>
        public static bool DebugMode = false;

        // ========================= Field of View =========================

        /// <summary>
        /// Field of View
        /// </summary>
        public const int PlayerFieldOfViewRadius = 6;
    }
}