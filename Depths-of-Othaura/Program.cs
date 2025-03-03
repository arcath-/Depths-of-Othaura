using Depths_of_Othaura.Data.Screens;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;

namespace Depths_of_Othaura
{
    /// <summary>
    ///  The main entry point for the Depths of Othaura game.
    /// </summary>
    internal class Program
    {
        /// <summary>
        ///  The main method that sets up and runs the game.
        /// </summary>
        private static void Main()
        {
            Settings.WindowTitle = Constants.GameTitle;
            Settings.ResizeMode = Settings.WindowResizeOptions.Stretch;

            Builder gameStartup = new Builder()
            .SetScreenSize(60, 40)
            .SetStartingScreen<ScreenContainer>()
            .OnStart(GameStart)
            .IsStartingScreenFocused(true)
            .ConfigureFonts((fontConfig, game) =>
            {
                fontConfig.UseCustomFont(Constants.Font);
            });

            Game.Create(gameStartup);
            Game.Instance.Run();
            Game.Instance.Dispose();
        }

        /// <summary>
        ///  Called when the game is starting up to perform initial setup.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="gameHost">The GameHost instance.</param>
        private static void OnStart(object sender, GameHost gameHost)
        {
            ColoredGlyph boxBorder = new ColoredGlyph(Color.White, Color.Black, 178);
            ColoredGlyph boxFill = new ColoredGlyph(Color.White, Color.Black);

            Game.Instance.StartingConsole.FillWithRandomGarbage(255);
            Game.Instance.StartingConsole.DrawBox(new Rectangle(2, 2, 26, 5), ShapeParameters.CreateFilled(boxBorder, boxFill));
            Game.Instance.StartingConsole.Print(4, 4, "Welcome to SadConsole!");
        }

        /// <summary>
        ///  Initializes the game world.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The GameHost instance.</param>
        private static void GameStart(object sender, GameHost e)
        {
            ScreenContainer.Instance.World.Generate();
            ScreenContainer.Instance.World.CreatePlayer();
        }


    }
}