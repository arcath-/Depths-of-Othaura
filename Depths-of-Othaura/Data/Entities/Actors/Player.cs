using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Input;
using SadConsole.Input;
using SadRogue.Primitives;
using System;
using Color = SadRogue.Primitives.Color;

// TODO: Implement a race and class system with managers.

namespace Depths_of_Othaura.Data.Entities.Actors
{
    /// <summary>
    /// Represents the player character in the game.
    /// </summary>
    internal class Player : Actor
    {
        // ========================= Fields =========================

        private readonly InputHandler _inputHandler = new InputHandler();
        private readonly Tilemap _tilemap;

        // ========================= Properties =========================

        private int _fovRadius = Constants.PlayerFieldOfViewRadius;

        /// <summary>
        /// Gets or sets the field-of-view radius for the player.
        /// </summary>
        public int FovRadius
        {
            get => _fovRadius;
            set
            {
                _fovRadius = value;
            }
        }

        // ========================= Constructor =========================

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        /// <param name="position">The starting position of the player.</param>
        /// <param name="tilemap">The tilemap to use for FOV calculations and movement.</param>
        public Player(Point position, Tilemap tilemap) : base(Color.White, Color.Transparent, '@', zIndex: int.MaxValue, maxHealth: 100)
        {
            _tilemap = tilemap ?? throw new ArgumentNullException(nameof(tilemap));

            IsFocused = true;
            PositionChanged += Player_PositionChanged;

            if (!Move(position.X, position.Y))
                throw new Exception($"Unable to move player to spawn position: {position}");
        }

        // ========================= Input Handling =========================

        /// <summary>
        /// Processes keyboard input for player actions, such as movement and toggling render/debug modes.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        /// <returns><c>true</c> if keyboard input was processed; otherwise, <c>false</c>.</returns>
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            return _inputHandler.ProcessKeyboard(keyboard, this) || base.ProcessKeyboard(keyboard);
        }

        // ========================= Movement =========================

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

        /// <summary>
        /// Attempts to move the actor to the specified coordinates.
        /// </summary>
        /// <param name="x">The X coordinate to move to.</param>
        /// <param name="y">The Y coordinate to move to.</param>
        /// <returns><c>true</c> if the actor was moved; otherwise, <c>false</c>.</returns>
        public bool Move(int x, int y)
        {
            var actorManager = ScreenContainer.Instance.World.ActorManager;

            if (!IsAlive) return false;

            // If the position is out of bounds, don't allow movement
            if (!_tilemap.InBounds(x, y)) return false;

            // If another actor already exists at the location, don't allow movement
            if (actorManager.ExistsAt((x, y))) return false;

            // Don't allow movement for these cases
            var obstruction = _tilemap[x, y].Obstruction;
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

        // ========================= Event Handling =========================

        /// <summary>
        /// Handles the player's position changing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments containing the old and new positions.</param>
        private void Player_PositionChanged(object sender, ValueChangedEventArgs<Point> e)
        {
            var world = ScreenContainer.Instance.World;
            // Calculate the field of view for the player's position
            world.FovManager.CalculateFOV(this);
        }
    }
}