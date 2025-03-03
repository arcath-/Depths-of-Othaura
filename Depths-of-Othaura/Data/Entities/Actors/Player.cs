using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Input;
using SadConsole.Input;
using SadRogue.Primitives;
using Color = SadRogue.Primitives.Color;

namespace Depths_of_Othaura.Data.Entities.Actors
{
    /// <summary>
    /// Represents the player character in the game.
    /// </summary>
    internal class Player : Actor
    {
        private readonly InputHandler _inputHandler = new();
        private readonly Tilemap _tilemap; // Add this

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

        /// <summary>
        /// Processes keyboard input for player actions, such as movement and toggling render/debug modes.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        /// <returns><c>true</c> if keyboard input was processed; otherwise, <c>false</c>.</returns>
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            return _inputHandler.ProcessKeyboard(keyboard, this) || base.ProcessKeyboard(keyboard);
        }

        

        /// <summary>
        /// Handles the player's position changing.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event arguments containing the old and new positions.</param>
        private void Player_PositionChanged(object sender, ValueChangedEventArgs<Point> e)
        {
            var world = ScreenContainer.Instance.World;
            
        }

        public bool Move(Direction direction)
        {
            var position = Position + direction;
            return Move(position.X, position.Y);
        }

        public bool Move(int x, int y)
        {
            var actorManager = Depths_of_Othaura.Data.Screens.ScreenContainer.Instance.World.ActorManager;

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
    }
}