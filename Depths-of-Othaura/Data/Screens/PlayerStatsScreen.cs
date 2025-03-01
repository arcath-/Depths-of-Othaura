using Depths_of_Othaura.Data.Entities.Actors;
using SadConsole;
using System;

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Represents the player's stats screen, displaying attributes such as health, attack, and experience.
    /// </summary>
    internal class PlayerStatsScreen : ScreenSurface
    {
        /// <summary>
        /// Gets the current player instance.
        /// </summary>
        private static Player Player => ScreenContainer.Instance.World.Player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerStatsScreen"/> class.
        /// </summary>
        /// <param name="width">The width of the player stats screen.</param>
        /// <param name="height">The height of the player stats screen.</param>
        public PlayerStatsScreen(int width, int height) : base(width, height)
        { }

        /// <summary>
        /// Updates the displayed player statistics.
        /// </summary>
        public void UpdatePlayerStats()
        {
            Surface.Clear();
            Surface.DrawBorderWithTitle("Attributes", Color.Gray, Color.Magenta);
            DrawPlayerAttributes();
        }

        /// <summary>
        /// Draws the player's attributes onto the screen.
        /// </summary>
        private void DrawPlayerAttributes()
        {
            Surface.Print(2, 2, $"HP:    {Player.Stats.Health}/{Player.Stats.MaxHealth}");
            Surface.Print(2, 3, $"ATK:   {Player.Stats.Attack}");
            Surface.Print(2, 4, $"DEF:   {Player.Stats.Defense}");
            Surface.Print(2, 5, $"AGI:   {Player.Stats.DodgeChance}");
            Surface.Print(2, 6, $"CRIT:  {Player.Stats.CritChance}");
            Surface.Print(2, 8, $"LVL:   {Player.Stats.Level}");
            Surface.Print(2, 9, $"EXP:   {Player.Stats.Experience}/{Player.Stats.RequiredExperience}");
        }
    }
}
