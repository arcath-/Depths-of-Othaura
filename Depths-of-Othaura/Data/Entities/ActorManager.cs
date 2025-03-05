using Depths_of_Othaura.Data.Entities.Actors;
using GoRogue.FOV;
using SadConsole.Entities;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;

// TODO: 

namespace Depths_of_Othaura.Data.Entities
{
    /// <summary>
    /// Manages all actors in the game world, including adding, removing, and tracking their positions.
    /// </summary>
    internal sealed class ActorManager
    {
        // ========================= Fields =========================

        /// <summary>
        /// Dictionary that stores actors by their position in the game world.
        /// </summary>
        private readonly Dictionary<Point, Actor> _actors = new Dictionary<Point, Actor>();

        /// <summary>
        /// Manages entities within the game world.
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
                SkipExistsChecks = true // Optimization to skip redundant checks
            };
        }

        // ========================= Actor Management =========================

        /// <summary>
        /// Adds an actor to the manager.
        /// </summary>
        /// <param name="actor">The actor to add.</param>
        /// <returns><c>true</c> if the actor was added successfully; otherwise, <c>false</c>.</returns>
        public bool Add(Actor actor)
        {
            if (ExistsAt(actor.Position)) return false; // Don't add if already exists at that position
            _actors[actor.Position] = actor;

            actor.PositionChanged += UpdateActorPositionWithinManager; // Subscribe to position changes
            EntityComponent.Add(actor); // Add to SadConsole's entity manager

            return true;
        }

        /// <summary>
        /// Removes an actor from the manager.
        /// </summary>
        /// <param name="actor">The actor to remove.</param>
        /// <returns><c>true</c> if the actor was removed successfully; otherwise, <c>false</c>.</returns>
        public bool Remove(Actor actor)
        {
            if (!ExistsAt(actor.Position)) return false; // Don't remove if it doesn't exist
            _actors.Remove(actor.Position);

            actor.PositionChanged -= UpdateActorPositionWithinManager; // Unsubscribe from position changes
            EntityComponent.Remove(actor); // Remove from SadConsole's entity manager

            return true;
        }

        /// <summary>
        /// Removes all actors from the manager.
        /// </summary>
        public void Clear()
        {
            foreach (var actor in _actors.Values)
            {
                _ = Remove(actor); // Remove each actor
            }
        }

        // ========================= Actor Retrieval =========================

        /// <summary>
        /// Retrieves an actor at a specific point in the world.
        /// </summary>
        /// <param name="point">The position to check.</param>
        /// <returns>The actor at the specified point, or <c>null</c> if no actor is found.</returns>
        public Actor Get(Point point)
        {
            if (_actors.TryGetValue(point, out Actor actor))
                return actor;
            return null;
        }

        /// <summary>
        /// Determines whether an actor exists at a specific position.
        /// </summary>
        /// <param name="point">The position to check.</param>
        /// <returns><c>true</c> if an actor exists at the given position; otherwise, <c>false</c>.</returns>
        public bool ExistsAt(Point point)
        {
            return _actors.ContainsKey(point);
        }

        /// <summary>
        /// Checks if a specific actor exists within the manager.
        /// </summary>
        /// <param name="actor">The actor to check.</param>
        /// <returns><c>true</c> if the actor is found within the manager; otherwise, <c>false</c>.</returns>
        public bool Contains(Actor actor)
        {
            return _actors.TryGetValue(actor.Position, out var actorAtPos) && actorAtPos.Equals(actor);
        }

        // ========================= Visibility =========================

        /// <summary>
        /// Updates the visibility of actors based on the field of view.
        /// </summary>
        /// <param name="fieldOfView">The field of view used to determine actor visibility.</param>
        public void UpdateVisibility(IFOV fieldOfView)
        {
            foreach (var actor in _actors)
            {
                // Set the visibility of the actor based on whether they are in the field of view
                actor.Value.IsVisible = fieldOfView.BooleanResultView[actor.Key];
            }
        }

        // ========================= Event Handling =========================

        /// <summary>
        /// Updates the position of an actor within the manager when it moves.
        /// </summary>
        /// <param name="sender">The actor whose position changed.</param>
        /// <param name="e">The old and new position values.</param>
        /// <exception cref="Exception">Thrown if an actor attempts to move to an occupied position.</exception>
        private void UpdateActorPositionWithinManager(object sender, ValueChangedEventArgs<Point> e)
        {
            if (e.OldValue == e.NewValue) return; // No change in position
            var actor = (Actor)sender;

            // Remove from previous position
            _actors.Remove(e.OldValue);

            // Check if the new position is occupied
            if (ExistsAt(e.NewValue))
            {
                throw new Exception($"Cannot move actor to {e.NewValue} another actor already exists there.");
            }

            // Add to new position
            _actors.Add(e.NewValue, actor);
        }
    }
}