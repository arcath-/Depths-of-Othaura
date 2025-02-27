﻿using Depths_of_Othaura.Data.Entities;
using Depths_of_Othaura.Data.Screens;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Depths_of_Othaura.Data.Logic
{
    internal static class MeleeCombatLogic
    {

        //Melee Attack Logic
        internal static void Attack(Actor attacker, Actor defender)
        {
            if (!attacker.IsAlive || !defender.IsAlive) return;
            var damage = CalculateDamage(attacker.Stats, defender.Stats, out bool isCriticalHit);

            if (damage > 0)
            {
                MessagesScreen.WriteLine($"{attacker.Name} has attacked {defender.Name} for {damage}{(isCriticalHit ? "critical" : "")} damage.");
                defender.ApplyDamage(damage);

                if (!defender.IsAlive)
                {
                    MessagesScreen.WriteLine($"{attacker.Name} has received {defender.Stats.ExperienceWorth} experience.");
                    var level = attacker.Stats.Level; // store old level before adding experience
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

        //Calculates proportionally scaling damage. 15% variance
        /*
        * The formula is designed to make the damage asymptotically approach the attacker’s Attack value
        * as Defense gets smaller, and approach zero as Defense grows very large. This scaling ensures no
        * extremes in damage, making it smoother and more balanced.
        */
        internal static int CalculateDamage(ActorStats attacker, ActorStats defender, out bool isCriticalHit)
        {
            isCriticalHit = false;
            var random = ScreenContainer.Instance.Random;

            // Dodge chance
            if (random.Next(0, 100) < defender.DodgeChance)
            {
                return 0; // No damage dealt
            }

            // Critical Hit Check
            isCriticalHit = random.Next(0, 100) < attacker.CritChance;
            float critMultiplier = isCriticalHit ? 1.5f : 1.0f;

            // Base Damage (Proportional Scaling with +- 15%randomness)
            int baseDamage = (int)Math.Round((float)attacker.Attack * attacker.Attack / (attacker.Attack + defender.Defense));
            baseDamage = random.Next((int)Math.Floor(baseDamage * 0.85), (int)Math.Ceiling(baseDamage * 1.15)); // Add 15% variance
            baseDamage = (int)Math.Round(baseDamage * critMultiplier); // Apply critical hit multiplier

            return Math.Max(1, baseDamage);
        }

        
        
        


    }
}
