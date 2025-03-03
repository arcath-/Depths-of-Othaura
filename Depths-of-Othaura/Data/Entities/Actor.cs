using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Input;
using SadConsole.Entities;
using SadConsole.Input;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;

namespace Depths_of_Othaura.Data.Entities
{
    /// <summary>
    /// Base class for all actors in the game.
    /// </summary>
    internal abstract class Actor : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Actor"/> class.
        /// </summary>
        /// <param name="foreground">The foreground color of the actor.</param>
        /// <param name="background">The background color of the actor.</param>
        /// <param name="glyph">The glyph representing the actor.</param>
        /// <param name="zIndex">The Z index of the actor.</param>
        /// <param name="maxHealth">The maximum health of the actor.</param>
        protected Actor(Color foreground, Color background, int glyph, int zIndex, int maxHealth) : base(foreground, background, glyph, zIndex)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
        }

        /// <summary>
        /// Gets or sets the maximum health of the actor.
        /// </summary>
        public int MaxHealth { get; set; }

        /// <summary>
        /// Gets or sets the current health of the actor.
        /// </summary>
        public int Health { get; set; }

        /// <summary>
        /// Gets a value indicating whether the actor is alive.
        /// </summary>
        public bool IsAlive => Health > 0;

        /// <summary>
        /// Attempts to move the actor to the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate to move to.</param>
        /// <param name="y">The Y coordinate to move to.</param>
        /// <returns><c>true</c> if the actor was moved; otherwise, <c>false</c>.</returns>
        public bool Move(int x, int y)
        {
            var tilemap = Depths_of_Othaura.Data.Screens.ScreenContainer.Instance.World.Tilemap;
            var actorManager = Depths_of_Othaura.Data.Screens.ScreenContainer.Instance.World.ActorManager;

            if (!IsAlive) return false;

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
            Position = new SadRogue.Primitives.Point(x, y);
            return true;
        }

        /// <summary>
        /// Attempts to move the actor in the specified direction.
        /// </summary>
        /// <param name="direction">The direction to move in.</param>
        /// <returns><c>true</c> if the actor was moved; otherwise, <c>false</c>.</returns>
        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }
    }
}