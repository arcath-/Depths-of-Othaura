using Depths_of_Othaura.Data.Screens;
using SadConsole;
using SadConsole.Configuration;

namespace Depths_of_Othaura
{
    /// <summary>
    /// Handles the startup process for the game.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The main entry point of the application. 
        /// It initializes the game settings, creates the game instance, and starts the game loop.
        /// </summary>
        private static void Main()
        {
            // Set up the game window settings
            Settings.WindowTitle = Constants.GameTitle;
            Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

            // Configure the game startup
            Builder gameStartup = new Builder()
                .SetScreenSize(60, 40)
                .SetStartingScreen<ScreenContainer>()
                .OnStart(GameStart)
                .IsStartingScreenFocused(true)
                .ConfigureFonts((fontConfig, game) =>
                {
                    fontConfig.UseCustomFont(Constants.Font);
                });

            // Create, run, and dispose of the game instance
            Game.Create(gameStartup);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        /// <summary>
        /// Initializes the game world upon startup.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The game host instance.</param>
        private static void GameStart(object sender, GameHost e)
        {
            var world = ScreenContainer.Instance.World;
            world.Generate();
        }
    }
}
