using Depths_of_Othaura.Data.Screens;
using SadConsole;
using SadConsole.Configuration;
using SadRogue.Primitives;

namespace Depths_of_Othaura
{
    internal class Program
    {
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

        private static void GameStart(object sender, GameHost e)
        {
            var world = ScreenContainer.Instance.World;
            world.Generate();
            world.CreatePlayer();
            world.CreateNpcs();
        }
    }
}
