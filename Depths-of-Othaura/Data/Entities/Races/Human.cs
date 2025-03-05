using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Races
{
    internal class Human : Race
    {
        public override string Name => "Human";
        public override string Description => "Adaptable and ambitious, humans are a versatile race.";
        public override CharacterSize Size => CharacterSize.Medium;

        // Humans get +1 to all stats
        public override int StrengthModifier => 1;
        public override int DexterityModifier => 1;
        public override int ConstitutionModifier => 1;
        public override int IntelligenceModifier => 1;
        public override int WisdomModifier => 1;
        public override int CharismaModifier => 1;

        public override string[] RacialTraits => ["Bonus Feat", "Skilled"];
    }
}
