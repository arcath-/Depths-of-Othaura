﻿using Depths_of_Othaura.Data.Entities.Actors;
using SadConsole;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Depths_of_Othaura.Data.Screens
{
    internal class PlayerStatsScreen : ScreenSurface
    {
        private static Player Player => ScreenContainer.Instance.World.Player;

        public PlayerStatsScreen(int width, int height) : base(width, height)
        { }

        public void UpdatePlayerStats()
        {
            Surface.Clear();
            Surface.DrawBorderWithTitle("Attributes", Color.Gray, Color.Magenta);
            DrawPlayerAttributes();
        }

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
