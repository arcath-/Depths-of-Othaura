using Depths_of_Othaura.Data.Entities;
using Depths_of_Othaura.Data.Screens;

using System;

namespace Depths_of_Othaura.Data.Logic
{
    /// <summary>
    /// Handles melee combat logic, including damage calculation, attacks, and experience gain.
    /// </summary>
    internal static class MeleeCombatLogic
    {
        /// <summary>
        /// Processes a melee attack from one actor to another.
        /// </summary>
        /// <param name="attacker">The actor performing the attack.</param>
        /// <param name="defender">The actor being attacked.</param>
        internal static void Attack(Actor attacker, Actor defender)
        {
            if (!attacker.IsAlive || !defender.IsAlive) return;
            var damage = CalculateDamage(attacker.Stats, defender.Stats, out bool isCriticalHit);

            if (damage > 0)
            {
                MessagesScreen.WriteLine($"{attacker.Name} has attacked {defender.Name} for {damage}{(isCriticalHit ? " critical" : "")} damage.");
                defender.ApplyDamage(damage);

                if (!defender.IsAlive)
                {
                    MessagesScreen.WriteLine($"{attacker.Name} has received {defender.Stats.ExperienceWorth} experience.");
                    var level = attacker.Stats.Level; // Store old level before adding experience
                    attacker.Stats.AddExperience(defender.Stats.ExperienceWorth);
                    if (level < attacker.Stats.Level)
                        MessagesScreen.WriteLine($"{attacker.Name} has leveled up!");
                }
            }
            else
            {
                MessagesScreen.WriteLine($"{defender.Name} dodged the attack by {attacker.Name}!");
            }
        }

        /// <summary>
        /// Calculates the damage dealt in a melee attack, factoring in attack, defense, dodge, and critical hits.
        /// </summary>
        /// <param name="attacker">The attacking actor's stats.</param>
        /// <param name="defender">The defending actor's stats.</param>
        /// <param name="isCriticalHit">Outputs whether the attack was a critical hit.</param>
        /// <returns>The amount of damage dealt.</returns>
        internal static int CalculateDamage(ActorStats attacker, ActorStats defender, out bool isCriticalHit)
        {
            isCriticalHit = false;
            var random = ScreenContainer.Instance.Random;

            // Check if the defender dodges the attack
            if (random.Next(0, 100) < defender.DodgeChance)
            {
                return 0; // No damage dealt
            }

            // Check for a critical hit
            isCriticalHit = random.Next(0, 100) < attacker.CritChance;
            float critMultiplier = isCriticalHit ? 1.5f : 1.0f;

            // Calculate base damage using a proportional scaling formula
            int baseDamage = (int)Math.Round((float)attacker.Attack * attacker.Attack / (attacker.Attack + defender.Defense));

            // Apply random variance of ±15%
            baseDamage = random.Next((int)Math.Floor(baseDamage * 0.85), (int)Math.Ceiling(baseDamage * 1.15));

            // Apply critical hit multiplier
            baseDamage = (int)Math.Round(baseDamage * critMultiplier);

            return Math.Max(1, baseDamage);
        }
    }
}
