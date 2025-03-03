using Depths_of_Othaura.Data.Entities.Actors;
using Depths_of_Othaura.Data.Screens;
using SadConsole.Input;


namespace Depths_of_Othaura.Input
{
    /// <summary>
    /// Handles keyboard input for the player.
    /// </summary>
    internal class InputHandler
    {
        /// <summary>
        /// Processes keyboard input and updates the player's state accordingly.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        /// <param name="player">The player to update.</param>
        /// <returns><c>true</c> if keyboard input was processed; otherwise, <c>false</c>.</returns>
        public bool ProcessKeyboard(Keyboard keyboard, Player player)
        {
            if (!player.UseKeyboard) return false;

            bool moved = HandleMovement(keyboard, player);
            HandleToggles(keyboard);

            return moved;
        }

        /// <summary>
        /// Handles player movement based on keyboard input.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        /// <param name="player">The player to update.</param>
        /// <returns><c>true</c> if the player moved; otherwise, <c>false</c>.</returns>
        private bool HandleMovement(Keyboard keyboard, Player player)
        {
            var moveDirection = GetMovementDirection(keyboard);
            if (moveDirection != null)
            {
                return player.Move((Direction)moveDirection);
            }
            return false;
        }

        /// <summary>
        /// Gets the movement direction based on keyboard input.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        /// <returns>The movement direction, or <c>null</c> if no movement keys are pressed.</returns>
        private static Direction? GetMovementDirection(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.W)) return Direction.Up;
            if (keyboard.IsKeyPressed(Keys.A)) return Direction.Left;
            if (keyboard.IsKeyPressed(Keys.S)) return Direction.Down;
            if (keyboard.IsKeyPressed(Keys.D)) return Direction.Right;
            return null;
        }

        /// <summary>
        /// Handles toggling render and debug modes based on keyboard input.
        /// </summary>
        /// <param name="keyboard">The keyboard state to process.</param>
        private static void HandleToggles(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.T))
            {
                ScreenContainer.Instance.World.ToggleRenderMode();
            }

            if (keyboard.IsKeyPressed(Keys.F1))
            {
                ScreenContainer.Instance.World.ToggleDebugMode();
            }
        }
    }
}