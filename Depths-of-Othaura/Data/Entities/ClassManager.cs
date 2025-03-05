using System.Collections.Generic;
using Depths_of_Othaura.Data.Entities.Classes;

namespace Depths_of_Othaura.Data.Entities
{
    internal static class ClassManager
    {
        public static List<Class> AvailableClasses { get; } = new List<Class>()
        {
            new Fighter(), // Add more classes here as you implement them
        };

        public static Class GetClass(string className)
        {
            return AvailableClasses.Find(c => c.Name == className);
        }
    }
}
