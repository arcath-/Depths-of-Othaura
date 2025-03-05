using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.Screens;
using Depths_of_Othaura.Data.World;
using GoRogue.FOV;
using SadRogue.Primitives;
using SadRogue.Primitives.GridViews;
using Color = SadRogue.Primitives.Color;

namespace Depths_of_Othaura.Data.World
{
    /// <summary>
    /// Manages the field of view (FOV) for the game.
    /// </summary>
    internal class FOVManager
    {
        private readonly WorldScreen _world;
        private readonly IFOV _fieldOfView; //This should not be instantiated every movement.
        private readonly LambdaGridView<bool> _fovGrid;
        private readonly bool[,] _currentFOV;
        private readonly int _width;
        private readonly int _height;

        //Cache color
        private readonly Color _unseenColor;
        private readonly Tile _baseTileForValues;
        private bool _disableFov = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="FOVManager"/> class.
        /// </summary>
        /// <param name="world">The world screen.</param>
        public FOVManager(WorldScreen world)
        {
            _world = world ?? throw new ArgumentNullException(nameof(world));
            _width = world.Tilemap.Width;
            _height = world.Tilemap.Height;

            //Initialize the FOV
            _fovGrid = new LambdaGridView<bool>(
                _width,
                _height,
                (point) => !BlocksFov(point, world));

            _fieldOfView = new RecursiveShadowcastingFOV(_fovGrid); // Setup FOV map, should not be set inside function, should be set ONCE, so move here.
            _currentFOV = new bool[_width, _height];

            //calculate color
            _baseTileForValues = world.Tilemap[0, 0]; //Cache, it will never change.
            _unseenColor = CalculateUnseenColor(_baseTileForValues);

            //Initialize the map to not visible
            InitilizeNotVisible();

        }

        //To initialize everything to a default, so all will be black.
        private void InitilizeNotVisible()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Point point = new Point(x, y);
                    _world.SetTileVisibility(point, false); // Ensures all unexplored tiles are blacked out
                }
            }
        }


        /// <summary>
        /// Calculates the field of view for the specified player.
        /// </summary>
        /// <param name="player">The player to calculate the field of view for.</param>
        public void CalculateFOV(Player player)
        {
            // This logic was taken from the player class and slightly modified
            _fieldOfView.Calculate(player.Position, player.FovRadius);
            ExploreTilemap(player);
        }

        /// <summary>
        /// Toggles the visibility of the entire map
        /// </summary>
        /// <param name="isEnabled"></param>
        public void ToggleVisibility(bool isEnabled)
        {
            _disableFov = isEnabled;
            _world.Surface.IsDirty = true; // Force a redraw

            var tilemap = _world.Tilemap;

            //This is to turn off the value,
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Point point = new Point(x, y);
                    Color baseColor = isEnabled ? _unseenColor : _baseTileForValues.Foreground;

                    tilemap[point.X, point.Y].Foreground = baseColor;
                    //tilemap[point.X, point.Y].IsVisible = !isEnabled; //Now it turns it all off.

                    if (isEnabled)
                    {
                        //Set the tiles that were previouslyHasBeenLit to a explored state if visibility is going off.
                        if (tilemap[point.X, point.Y].HasBeenLit)
                        {
                            //Its the explored style
                            tilemap[point.X, point.Y].Foreground = _unseenColor;
                            tilemap[point.X, point.Y].Background = Color.Transparent;
                        }
                    }

                    _world.SetTileVisibility(point, !isEnabled);
                }
            }
        }

        /// <summary>
        /// Explores the tilemap based on the player's field of view.
        /// </summary>
        /// <param name="player">The player to explore the tilemap for.</param>
        private void ExploreTilemap(Player player)
        {
            //If you are in debug, do not run the FOV
            if (_disableFov) return;

            var tilemap = _world.Tilemap;

            //Recalculate Field of View - necessary because the map is always going to be dirty
            _fieldOfView.Calculate(player.Position, player.FovRadius);

            // Iterate through the ENTIRE map, and apply FOV, this will be a lot of code.
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Point point = new Point(x, y);
                    double fovValue = _fieldOfView.DoubleResultView[point];
                    bool inFov = fovValue > 0;

                    //Check if there is a change from before.
                    if (_currentFOV[x, y] != inFov)
                    {
                        //Its a change!
                        _currentFOV[x, y] = inFov; //Update so there isnt a change next time.
                        ApplyFov(point, tilemap, inFov);
                    }
                }
            }
        }

        //Set colors function
        private void ApplyFov(SadRogue.Primitives.Point point, Tilemap tilemap, bool inFov)
        {
            //Update visibility even if colors are not changed.
            tilemap[point.X, point.Y].IsVisible = inFov;

            if (inFov)
            {
                // Tile is currently in FOV, show its actual color
                tilemap[point.X, point.Y].Foreground = _baseTileForValues.Foreground;
                tilemap[point.X, point.Y].Background = _baseTileForValues.Background;
                tilemap[point.X, point.Y].HasBeenLit = true;

                _world.SetTileVisibility(point, tilemap[point.X, point.Y].IsVisible);

            }
            else
            {
                //If we have been in the level before, use those instead.
                if (tilemap[point.X, point.Y].HasBeenLit)
                {
                    // Tile has been seen before, so make it dark gray
                    tilemap[point.X, point.Y].Foreground = Color.Lerp(_baseTileForValues.Foreground, Color.Black, 0.7f);

                    _world.SetTileVisibility(point, tilemap[point.X, point.Y].IsVisible);
                }
                else
                {
                    //set the unseen values
                    // Completely unseen tile remains black
                    tilemap[point.X, point.Y].Foreground = Color.Black;
                    tilemap[point.X, point.Y].Background = Color.Transparent;
                    _world.SetTileVisibility(point, tilemap[point.X, point.Y].IsVisible);
                }


            }


        }

        /// <summary>
        /// Calculates the unseen color based on base value with Lerp
        /// </summary>
        /// <param name="tile">a sample tile.</param>
        /// <returns>new Color</returns>
        private Color CalculateUnseenColor(Tile tile)
        {
            Color baseColor = tile.Foreground;

            //We lerp it to 50% of black.
            return Color.Lerp(baseColor, Color.Black, 0.5f);
        }

        //This was a anonymous function, that means garbage allocation. Move outside the function.
        private bool BlocksFov(SadRogue.Primitives.Point point, WorldScreen world)
        {
            return BlocksFov(world.Tilemap[point.X, point.Y].Obstruction);
        }

        private static bool BlocksFov(ObstructionType obstructionType)
        {
            return obstructionType switch
            {
                ObstructionType.VisionBlocked or ObstructionType.FullyBlocked => true,
                _ => false,
            };
        }
    }
}