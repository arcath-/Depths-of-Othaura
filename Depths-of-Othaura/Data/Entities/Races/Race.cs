using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Depths_of_Othaura.Data.Entities.Races
{
    /// <summary>
    /// Represents a character race, providing base stat adjustments and other racial traits.
    /// </summary>
    internal abstract class Race
    {
        /// <summary>
        /// Gets the name of the race.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the description of the race.
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the base size of the race.
        /// </summary>
        public abstract CharacterSize Size { get; }

        /// <summary>
        /// Gets the modifier to Strength based on the race.
        /// </summary>
        public virtual int StrengthModifier => 0;

        /// <summary>
        /// Gets the modifier to Dexterity based on the race.
        /// </summary>
        public virtual int DexterityModifier => 0;

        /// <summary>
        /// Gets the modifier to Constitution based on the race.
        /// </summary>
        public virtual int ConstitutionModifier => 0;

        /// <summary>
        /// Gets the modifier to Intelligence based on the race.
        /// </summary>
        public virtual int IntelligenceModifier => 0;

        /// <summary>
        /// Gets the modifier to Wisdom based on the race.
        /// </summary>
        public virtual int WisdomModifier => 0;

        /// <summary>
        /// Gets the modifier to Charisma based on the race.
        /// </summary>
        public virtual int CharismaModifier => 0;

        /// <summary>
        /// Gets the racial traits the class provides
        /// </summary>
        public virtual string[] RacialTraits => [];
    }

    public enum CharacterSize
    {
        Fine,
        Diminutive,
        Tiny,
        Small,
        Medium,
        Large,
        Huge,
        Gargantuan,
        Colossal
    }
}
