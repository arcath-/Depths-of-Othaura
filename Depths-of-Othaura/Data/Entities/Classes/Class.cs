using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Classes
{
    /// <summary>
    /// Represents a character class, providing base hit dice, skill points and more.
    /// </summary>
    internal abstract class Class
    {
        /// <summary>
        /// Gets the name of the class.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the class.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the die used to calculate health per level.
        /// </summary>
        public abstract int HitDie { get; }
    }
}
