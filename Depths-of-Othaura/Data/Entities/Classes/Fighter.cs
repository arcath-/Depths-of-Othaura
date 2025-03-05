using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Classes
{
    internal class Fighter : Class
    {
        public override string Name => "Fighter";
        public override string Description => "A master of weapons and tactics, the fighter excels in combat.";
        public override int HitDie => 10;
    }
}
