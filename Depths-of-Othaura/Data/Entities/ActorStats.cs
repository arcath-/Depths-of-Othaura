using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.Screens;
using System;

namespace Depths_of_Othaura.Data.Entities
{
    /// <summary>
    /// Represents the statistics and attributes of an actor, including health, attack, defense, and experience.
    /// </summary>
    internal sealed class ActorStats
    {
        /// <summary>
        /// The current level of the actor.
        /// </summary>
        public int Level { get; private set; } = 1;

        /// <summary>
        /// The amount of experience the actor has accumulated.
        /// </summary>
        public int Experience { get; private set; } = 0;

        /// <summary>
        /// The maximum health points the actor can have.
        /// </summary>
        public int MaxHealth { get; private set; }

        /// <summary>
        /// The attack power of the actor.
        /// </summary>
        public int Attack { get; private set; } = 1;

        /// <summary>
        /// The defense power of the actor.
        /// </summary>
        public int Defense { get; private set; } = 0;

        /// <summary>
        /// The actor's chance to dodge an attack, represented as a percentage.
        /// </summary>
        public int DodgeChance { get; private set; } = 0;

        /// <summary>
        /// The actor's chance to land a critical hit, represented as a percentage.
        /// </summary>
        public int CritChance { get; private set; } = 0;

        private int _health = 1;

        /// <summary>
        /// The current health points of the actor.
        /// Ensures that health does not exceed <see cref="MaxHealth"/> or drop below zero.
        /// </summary>
        public int Health
        {
            get => _health;
            set => _health = Math.Max(0, Math.Min(value, MaxHealth));
        }

        /// <summary>
        /// The parent actor that owns these stats.
        /// </summary>
        public Actor Parent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorStats"/> class.
        /// </summary>
        /// <param name="actor">The parent actor.</param>
        /// <param name="maxHealth">The maximum health of the actor.</param>
        public ActorStats(Actor actor, int maxHealth)
        {
            Parent = actor;
            MaxHealth = Math.Max(1, maxHealth);
            Health = MaxHealth;
        }

        /// <summary>
        /// The base experience value used in level-up calculations.
        /// </summary>
        private const float _baseExperience = 20;

        /// <summary>
        /// The required experience needed to level up.
        /// </summary>
        public int RequiredExperience => (int)Math.Round(_baseExperience * Level * (Level + 1) / 2);

        /// <summary>
        /// The amount of experience this actor is worth when defeated.
        /// </summary>
        public int ExperienceWorth => (int)Math.Round(MaxHealth * (1 + (Level - ScreenContainer.Instance.World.Player.Stats.Level) * 0.1));

        /// <summary>
        /// Sets the actor's attributes, allowing optional parameters for attack, defense, dodge chance, and critical chance.
        /// </summary>
        /// <param name="atk">The new attack value (optional).</param>
        /// <param name="def">The new defense value (optional).</param>
        /// <param name="dodge">The new dodge chance (optional).</param>
        /// <param name="crit">The new critical hit chance (optional).</param>
        public void Set(int? atk = null, int? def = null, int? dodge = null, int? crit = null)
        {
            Attack = Math.Max(1, atk ?? Attack);
            Defense = Math.Max(0, def ?? Defense);
            DodgeChance = Math.Max(0, Math.Min(dodge ?? DodgeChance, 50)); // 50% max dodge chance
            CritChance = Math.Max(0, crit ?? CritChance);

            // Update the stats window if the parent is the player.
            if (Parent is Player)
                ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        /// <summary>
        /// Adds experience points to the actor, triggering level-ups if necessary.
        /// </summary>
        /// <param name="experience">The amount of experience to add.</param>
        public void AddExperience(int experience)
        {
            Experience += experience;

            // Keep leveling up while the experience exceeds the required amount.
            while (Experience >= RequiredExperience)
            {
                // Subtract the required experience for the current level.
                Experience -= RequiredExperience;

                // Increase level.
                Level += 1;

                // Restore health on level-up.
                Health = MaxHealth;

                // Increase attack and defense every 2 levels.
                if (Level % 2 == 0)
                    Set(atk: Attack + 1, def: Defense + 1);
            }

            // Update the player stats on the UI if the actor is the player.
            if (Parent is Player)
                ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        /// <summary>
        /// Sets the actor's level by adding experience points until the target level is reached.
        /// This ensures the actor receives appropriate stat upgrades.
        /// </summary>
        /// <param name="level">The target level.</param>
        public void SetLevel(int level)
        {
            Level = 1;
            Experience = 0;

            while (Level != level && level > 0)
            {
                AddExperience(RequiredExperience);
            }
        }
    }
}
