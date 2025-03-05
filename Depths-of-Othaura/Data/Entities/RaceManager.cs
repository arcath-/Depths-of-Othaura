using System.Collections.Generic;
using Depths_of_Othaura.Data.Entities.Races;
using Depths_of_Othaura.Data.Entities.Races;

namespace Depths_of_Othaura.Data.Entities
{
    internal static class RaceManager
    {
        public static List<Race> AvailableRaces { get; } = new List<Race>()
        {
            new Human(), // Add more races here as you implement them
        };

        public static Race GetRace(string raceName)
        {
            return AvailableRaces.Find(r => r.Name == raceName);
        }
    }
}
