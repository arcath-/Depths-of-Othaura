using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using Depths_of_Othaura.Data.Logic;

using SadConsole.Input;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using GoRogue.FOV;

using System.Collections.Generic;

namespace Depths_of_Othaura.Data.Entities.Actors
{
    /// <summary>
    /// Represents the player-controlled character in the game.
    /// </summary>
    internal class Player : Actor
    {
        /// <summary>
        /// The player's field of view.
        /// </summary>
        public IFOV FieldOfView { get; }

        private int _fovRadius = Constants.PlayerFieldOfViewRadius;

        /// <summary>
        /// The radius of the player's field of view.
        /// Adjusting this property recalculates the field of view.
        /// </summary>
        public int FovRadius
        {
            get => _fovRadius;
            set
            {
                _fovRadius = value;

                // Recalculate field of view on radius change
                FieldOfView.Calculate(Position, _fovRadius);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class at the given position.
        /// </summary>
        /// <param name="position">The starting position of the player.</param>
        public Player(Point position)
            : base(Color.White, Color.Transparent, '@', zIndex: int.MaxValue, maxHealth: 100)
        {
            Name = "Rogue";

            // Setup field of view map
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            FieldOfView = new RecursiveShadowcastingFOV(new LambdaGridView<bool>(tilemap.Width, tilemap.Height,
                (point) => !BlocksFov(tilemap[point.X, point.Y].Obstruction)));

            IsFocused = true;
            PositionChanged += Player_PositionChanged;
            Position = position;
        }

        /// <summary>
        /// Marks all tiles within the current field of view as explored.
        /// </summary>
        public void ExploreCurrentFov()
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            foreach (var point in FieldOfView.CurrentFOV)
            {
                tilemap[point.X, point.Y].IsVisible = true;
                tilemap[point.X, point.Y].InFov = true;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }
        }

        /// <summary>
        /// Updates the visibility status of tiles within the player's field of view.
        /// </summary>
        private void ExploreTilemap()
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;

            // Mark newly seen tiles
            foreach (var point in FieldOfView.NewlySeen)
            {
                tilemap[point.X, point.Y].IsVisible = true;
                tilemap[point.X, point.Y].InFov = true;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }

            // Mark tiles that are no longer in view
            foreach (var point in FieldOfView.NewlyUnseen)
            {
                tilemap[point.X, point.Y].InFov = false;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }
        }

        /// <summary>
        /// Handles updates when the player's position changes.
        /// </summary>
        /// <param name="sender">The object triggering the event.</param>
        /// <param name="e">The event arguments containing the old and new position.</param>
        private void Player_PositionChanged(object sender, ValueChangedEventArgs<Point> e)
        {
            // Recalculate the player's field of view
            FieldOfView.Calculate(e.NewValue, FovRadius);

            // Update visibility of all actors based on the new FOV
            ScreenContainer.Instance.World.ActorManager.UpdateVisibility(FieldOfView);

            // Explore newly visible areas
            ExploreTilemap();
        }

        /// <summary>
        /// Determines whether a given obstruction type blocks field of view.
        /// </summary>
        /// <param name="obstructionType">The obstruction type to check.</param>
        /// <returns>Returns <c>true</c> if the obstruction blocks vision; otherwise, <c>false</c>.</returns>
        private static bool BlocksFov(ObstructionType obstructionType)
        {
            return obstructionType switch
            {
                ObstructionType.VisionBlocked or ObstructionType.FullyBlocked => true,
                _ => false,
            };
        }

        /// <summary>
        /// A dictionary mapping player movement keys to movement directions.
        /// </summary>
        private readonly Dictionary<Keys, Direction> _playerMovements = new()
        {
            {Keys.W, Direction.Up},
            {Keys.A, Direction.Left},
            {Keys.S, Direction.Down},
            {Keys.D, Direction.Right}
        };

        /// <summary>
        /// Processes player input for movement.
        /// </summary>
        /// <param name="keyboard">The keyboard input object.</param>
        /// <returns>Returns <c>true</c> if an action was performed; otherwise, <c>false</c>.</returns>
        public override bool ProcessKeyboard(Keyboard keyboard)
        {
            if (!UseKeyboard) return false;
            var moved = false;
            foreach (var kvp in _playerMovements)
            {
                if (keyboard.IsKeyPressed(kvp.Key))
                {
                    var moveDirection = kvp.Value;
                    moved = Move(moveDirection);
                    break;
                }
            }
            return base.ProcessKeyboard(keyboard) || moved;
        }

        /// <summary>
        /// Moves the player to the specified position and triggers game logic.
        /// </summary>
        /// <param name="x">The target X-coordinate.</param>
        /// <param name="y">The target Y-coordinate.</param>
        /// <returns>Returns <c>true</c> if the move was successful; otherwise, <c>false</c>.</returns>
        public override bool Move(int x, int y)
        {
            var moved = base.Move(x, y);

            // Execute game logic tick on movement, even if movement fails, only if alive.
            if (IsAlive)
                GameLogic.Tick(new Point(x, y));

            return moved;
        }

        /// <summary>
        /// Applies damage to the player and updates the stats screen.
        /// </summary>
        /// <param name="health">The amount of health to subtract.</param>
        public override void ApplyDamage(int health)
        {
            base.ApplyDamage(health);
            ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }
    }
}
