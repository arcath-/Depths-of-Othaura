using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Actors
{
    /// <summary>
    /// Represents a goblin enemy in the game.
    /// </summary>
    internal class Goblin : Actor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Goblin"/> class at the specified position.
        /// </summary>
        /// <param name="position">The starting position of the goblin in the game world.</param>
        public Goblin(Point position) :
            base(Color.Green, Color.Transparent, 'g', 1, maxHealth: 5)
        {
            Name = "Goblin";
            Position = position;

            // Set base stats
            Stats.Set(atk: 2, dodge: 10);
        }
    }
}
