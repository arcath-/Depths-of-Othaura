using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using SadConsole.Input;
using SadRogue.Primitives.GridViews;
using Direction = SadRogue.Primitives.Direction;
using GoRogue.FOV;
using GoRogue;
using Depths_of_Othaura.Data.Logic;

namespace Depths_of_Othaura.Data.Entities.Actors
{
    internal class Player : Actor
    {
        public IFOV FieldOfView { get; }

        private int _fovRadius = Constants.PlayerFieldOfViewRadius;
        public int FovRadius
        {
            get => _fovRadius;
            set
            {
                _fovRadius = value;

                // Recalculate fov on radius change
                FieldOfView.Calculate(Position, _fovRadius);
            }
        }

        public Player(Point position) : base(Color.White, Color.Transparent, '@', zIndex: int.MaxValue, maxHealth: 100)
        {
            Name = "Rogue";

            // Setup FOV map
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            FieldOfView = new RecursiveShadowcastingFOV(new LambdaGridView<bool>(tilemap.Width, tilemap.Height,
                (point) => !BlocksFov(tilemap[point.X, point.Y].Obstruction)));

            IsFocused = true;
            PositionChanged += Player_PositionChanged;
            Position = position;

            
        }

        private readonly Dictionary<Keys, Direction> _playerMovements = new()
        {
            {Keys.W, Direction.Up},
            {Keys.A, Direction.Left},
            {Keys.S, Direction.Down},
            {Keys.D, Direction.Right}
        };

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

        private static bool BlocksFov(ObstructionType obstructionType)
        {
            return obstructionType switch
            {
                ObstructionType.VisionBlocked or ObstructionType.FullyBlocked => true,
                _ => false,
            };
        }

        private void ExploreTilemap()
        {
            var tilemap = ScreenContainer.Instance.World.Tilemap;

            // Seen tiles entering the FOV
            foreach (var point in FieldOfView.NewlySeen)
            {
                tilemap[point.X, point.Y].IsVisible = true;
                tilemap[point.X, point.Y].InFov = true;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }

            // Unseen tiles leaving the FOV
            foreach (var point in FieldOfView.NewlyUnseen)
            {
                tilemap[point.X, point.Y].InFov = false;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }
        }

        private void Player_PositionChanged(object sender, ValueChangedEventArgs<Point> e)
        {
            // Calculate the field of view for the player's position
            FieldOfView.Calculate(e.NewValue, FovRadius);

            // Update the visibility of actors
            ScreenContainer.Instance.World.ActorManager.UpdateVisibility(FieldOfView);

            // Explore the dungeon tiles
            ExploreTilemap();
        }

        //overriding to call tick logic
        public override bool Move(int x, int y)
        {
            var moved = base.Move(x, y);

            // Execute a game logic tick on movement, even if movement failed. Only if alive.
            if (IsAlive) 
                GameLogic.Tick(new Point(x, y));

            return moved;
        }

        //overridden to update stats screen when losing health.
        public override void ApplyDamage(int health)
        {
            base.ApplyDamage(health);
            ScreenContainer.Instance.PlayerStats.UpdatePlayerStats();
        }

        public void ExploreCurrentFov()
        {
            // Used when we teleport
            var tilemap = ScreenContainer.Instance.World.Tilemap;
            foreach (var point in FieldOfView.CurrentFOV)
            {
                tilemap[point.X, point.Y].IsVisible = true;
                tilemap[point.X, point.Y].InFov = true;
                ScreenContainer.Instance.World.Surface.IsDirty = true;
            }
        }
    }
}
