using Depths_of_Othaura.Data.Screens;
using SadConsole.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Depths_of_Othaura.Data.Entities
{
    /// <summary>
    /// Represents an entity that can act in the game world.
    /// </summary>
    internal abstract class Actor : Entity
    {
        /// <summary>
        /// The statistics associated with this actor, such as health.
        /// </summary>
        public ActorStats Stats { get; }

        /// <summary>
        /// Determines whether the actor is currently alive.
        /// </summary>
        public bool IsAlive => Stats.Health > 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="foreground">The foreground color of the actor.</param>
        /// <param name="background">The background color of the actor.</param>
        /// <param name="glyph">The glyph representing the actor.</param>
        /// <param name="zIndex">The render layer index of the actor.</param>
        /// <param name="maxHealth">The maximum health of the actor.</param>
        protected Actor(Color foreground, Color background, int glyph, int zIndex, int maxHealth) : base(foreground, background, glyph, zIndex)
        {
            Stats = new ActorStats(this, maxHealth);
        }

        /// <summary>
        /// Moves the actor to a specified (x, y) coordinate if the move is valid.
        /// </summary>
        /// <param name="x">The target X-coordinate.</param>
        /// <param name="y">The target Y-coordinate.</param>
        /// <returns>Returns <c>true</c> if the move was successful; otherwise, <c>false</c>.</returns>
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

        /// <summary>
        /// Moves the actor in a specified direction.
        /// </summary>
        /// <param name="direction">The direction in which to move.</param>
        /// <returns>Returns <c>true</c> if the move was successful; otherwise, <c>false</c>.</returns>
        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }

        /// <summary>
        /// Applies damage to the actor, reducing its health, only if alive.
        /// </summary>
        /// <param name="health">The amount of health to subtract.</param>
        public virtual void ApplyDamage(int health)
        {
            Stats.Health -= health;

            if (!IsAlive && ScreenContainer.Instance.World.ActorManager.Contains(this))
            {
                OnDeath();
            }
        }

        /// <summary>
        /// Handles the actor's death, removing it from the game world and displaying a message.
        /// </summary>
        protected virtual void OnDeath()
        {
            // Remove from manager so its no longer rendered
            ScreenContainer.Instance.World.ActorManager.Remove(this);
            MessagesScreen.WriteLine($"{Name} has died.");
        }

    }
}
