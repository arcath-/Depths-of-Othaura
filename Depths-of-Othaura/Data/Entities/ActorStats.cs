using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.Screens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Depths_of_Othaura.Data.Entities
{
    internal sealed class ActorStats
    {
        public int Level { get; private set; } = 1;
        public int Experience { get; private set; } = 0;

        public int MaxHealth { get; private set; }
        public int Attack { get; private set; } = 1;
        public int Defense { get; private set; } = 0;
        public int DodgeChance { get; private set; } = 0;
        public int CritChance { get; private set; } = 0;

        private int _health = 1;
        public int Health
        {
            get => _health;
            set => _health = Math.Max(0, Math.Min(value, MaxHealth));
        }

        public Actor Parent { get; }
        public ActorStats(Actor actor, int maxHealth)
        {
            Parent = actor;
            MaxHealth = Math.Max(1, maxHealth);
            Health = MaxHealth;
        }

        // The base experience for all calculations
        private const float _baseExperience = 20;

        // The needed experience for this actor to level up
        // XP = BaseXP * Level * (Level +1) / 2
        public int RequiredExperience => (int)Math.Round(_baseExperience * Level * (Level + 1) / 2);

        // How much the actor is worth in experience
        // EXP = BaseEXP * ( 1 + (enemyLevel - playerLevel) * 0.1)
        public int ExperienceWorth => (int)Math.Round(MaxHealth * (1 + (Level - ScreenContainer.Instance.World.Player.Stats.Level) * 0.1));

        public void Set(int? atk = null, int? def = null, int? dodge = null, int? crit = null)
        {
            Attack = Math.Max(1, atk ?? Attack);
            Defense = Math.Max(0, def ?? Defense);
            DodgeChance = Math.Max(0, Math.Min(dodge ?? DodgeChance, 50)); // 50% max dodge chance
            CritChance = Math.Max(0, crit ?? CritChance);

            //update the stats window if parent is player.
            if (Parent is Player)
                ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        public void AddExperience(int experience)
        {
            Experience += experience;

            // Aslong as the current experience is higher than what we need for levelup
            while (Experience >= RequiredExperience)
            {
                // Subtract the required experience for the current level
                Experience -= RequiredExperience;

                // Level up
                Level += 1;

                // Reset health on level up
                Health = MaxHealth;

                // Passively increase attack and defense every 2 levels
                if (Level % 2 == 0)
                    Set(atk: Attack + 1, def: Defense + 1);
            }

            // Update the player stats on the UI
            if (Parent is Player)
                ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        // Additionally we’ll also add a method to set the current level of an actor, by
        // adding experience until the level is reached, this will also make it so that
        // the actor receives the correct stat upgrades.
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
