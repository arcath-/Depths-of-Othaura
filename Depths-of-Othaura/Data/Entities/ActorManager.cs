using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

// TODO: 

namespace Depths_of_Othaura.Data.Entities
{
    /// <summary>
    /// Manages the actors in the game world, providing methods for adding, removing, and retrieving actors.
    /// </summary>
    internal sealed class ActorManager
    {
        // ========================= Fields =========================

        /// <summary>
        /// Dictionary storing actors by their position.
        /// </summary>
        private readonly Dictionary<Point, Actor> _actors = new Dictionary<Point, Actor>();

        /// <summary>
        /// Manages the actors' visual representation on the screen.
        /// </summary>
        public readonly EntityManager EntityComponent;

        // ========================= Constructor =========================

        /// <summary>
        /// Initializes a new instance of the <see cref="ActorManager"/> class.
        /// </summary>
        public ActorManager()
        {
            EntityComponent = new EntityManager()
            {
                SkipExistsChecks = true
            };
        }

        // ========================= Actor Management =========================

        /// <summary>
        /// Adds an actor to the manager.
        /// </summary>
        /// <param name="actor">The actor to add.</param>
        /// <returns><c>true</c> if the actor was added; otherwise, <c>false</c>.</returns>
        public bool Add(Actor actor)
        {
            if (ExistsAt(actor.Position)) return false;
            _actors[actor.Position] = actor;

            actor.PositionChanged += UpdateActorPositionWithinManager;
            EntityComponent.Add(actor);

            return true;
        }

        /// <summary>
        /// Removes an actor from the manager.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        /// <returns><c>true</c> if the actor was removed; otherwise, <c>false</c>.</returns>
        public bool Remove(Actor actor)
        {
            if (!ExistsAt(actor.Position)) return false;
            _actors.Remove(actor.Position);

            actor.PositionChanged -= UpdateActorPositionWithinManager;
            EntityComponent.Remove(actor);

            return true;
        }

        /// <summary>
        /// Clears all actors from the manager.
        /// </summary>
        public void Clear()
        {
            foreach (var actor in _actors.Values)
            {
                _ = Remove(actor);
            }
        }

        // ========================= Actor Retrieval =========================

        /// <summary>
        /// Gets the actor at the specified point.
        /// </summary>
        /// <param name="point">The point to get the actor at.</param>
        /// <returns>The actor at the specified point, or <c>null</c> if no actor exists at the point.</returns>
        public Actor Get(Point point)
        {
            if (_actors.TryGetValue(point, out Actor actor))
                return actor;
            return null;
        }

        /// <summary>
        /// Checks if an actor exists at the specified point.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns><c>true</c> if an actor exists at the point; otherwise, <c>false</c>.</returns>
        public bool ExistsAt(Point point)
        {
            return _actors.ContainsKey(point);
        }

        // ========================= Event Handling =========================

        /// <summary>
        /// Updates the actor's position within the manager when its position changes.
        /// </summary>
        /// <param name="sender">The actor whose position changed.</param>
        /// <param name="e">The event arguments containing the old and new positions.</param>
        /// <exception cref="Exception">Thrown if the actor is moved to a position that is already occupied.</exception>
        private void UpdateActorPositionWithinManager(object sender, ValueChangedEventArgs<Point> e)
        {
            if (e.OldValue == e.NewValue) return;
            var actor = (Actor)sender;

            // Remove from previous
            _actors.Remove(e.OldValue);

            // Check if the new position is occupied
            if (ExistsAt(e.NewValue))
            {
                throw new Exception($"Cannot move actor to {e.NewValue} another actor already exists there.");
            }

            _actors.Add(e.NewValue, actor);
        }
    }
}