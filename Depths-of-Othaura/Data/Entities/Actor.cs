﻿using Depths_of_Othaura.Data.Screens;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Depths_of_Othaura.Data.Entities
{
    internal abstract class Actor : Entity
    {
        public ActorStats Stats { get; }
        public bool IsAlive => Stats.Health > 0;

        //constructor
        protected Actor(Color foreground, Color background, int glyph, int zIndex, int maxHealth) : base(foreground, background, glyph, zIndex)
        {
            Stats = new ActorStats(this, maxHealth);
        }

        public virtual bool Move(int x, int y)
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            var actorManager = ScreenContainer.Instance.World.ActorManager;

            if (!IsAlive || (Position.X == x && Position.Y == y)) return false;

            // If the position is out of bounds, don't allow movement
            if (!tilemap.InBounds(x, y)) return false;

            // If another actor already exists at the location, don't allow movement
            if (actorManager.ExistsAt((x, y))) return false;

            // Don't allow movement for these cases
            var obstruction = tilemap[x, y].Obstruction;
            switch (obstruction)
            {
                case World.ObstructionType.FullyBlocked:
                case World.ObstructionType.MovementBlocked:
                    return false;
            }

            // Set new position
            Position = new Point(x, y);
            return true;
        }

        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }

        //Do the thing
        public virtual void ApplyDamage(int health)
        {
            Stats.Health -= health;

            if (!IsAlive && ScreenContainer.Instance.World.ActorManager.Contains(this))
            {
                OnDeath();
            }
        }

        protected virtual void OnDeath()
        {
            // Remove from manager so its no longer rendered
            ScreenContainer.Instance.World.ActorManager.Remove(this);
            MessagesScreen.WriteLine($"{Name} has died.");
        }

    }
}
