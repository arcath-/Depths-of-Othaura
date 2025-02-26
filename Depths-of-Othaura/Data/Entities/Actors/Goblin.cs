using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Actors
{
    internal class Goblin : Actor
    {
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
