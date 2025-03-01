using SadConsole;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Depths_of_Othaura.Data.Screens
{
    /// <summary>
    /// Represents the in-game message log screen where messages are displayed.
    /// </summary>
    internal class MessagesScreen : ScreenSurface
    {
        /// <summary>
        /// The internal screen surface used to display messages.
        /// </summary>
        private readonly ScreenSurface _messageSurface;

        /// <summary>
        /// Stores the list of messages displayed in the message log.
        /// </summary>
        private readonly List<string> _messages = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesScreen"/> class.
        /// </summary>
        /// <param name="width">The width of the message screen.</param>
        /// <param name="height">The height of the message screen.</param>
        public MessagesScreen(int width, int height) : base(width, height)
        {
            // We use a surface with a smaller width font to allow more space for text

            // Since our parent uses a 16x16 font and the IBM font is 8x16, we must translate the width properly
            int translatedWidth = (width - 1) * 4; // Adjust width to match original 16x16 spacing (-1 to exclude borders)
            int translatedHeight = height - 2; // Exclude 2 to remove the top/bottom borders

            _messageSurface = new ScreenSurface(translatedWidth, translatedHeight)
            {
                Font = Game.Instance.Fonts["IBM_8x16"], // Use the default SadConsole font
                Position = new Point(1, 1) // Position within the border
            };

            // Debugging aid: Uncomment to visualize the surface area
            // _messageSurface.Fill(background: Color.Blue);

            // Add the message surface as a child so it can be rendered properly
            Children.Add(_messageSurface);

            // Draw the main border around the message screen
            Surface.DrawBorderWithTitle("Messages", Color.Gray, Color.Magenta);
        }

        /// <summary>
        /// Adds a new message to the message log.
        /// </summary>
        /// <param name="message">The message to be added.</param>
        public void AddMessage(string message)
        {
            // Remove the oldest message if we reach our display limit
            if (_messages.Count == _messageSurface.Height - 2)
                _messages.RemoveAt(0);

            _messages.Add(message);

            // Redraw the message log
            DrawMessages();
        }

        /// <summary>
        /// Redraws all messages onto the message log screen.
        /// </summary>
        private void DrawMessages()
        {
            _messageSurface.Surface.Clear();

            // Print messages in order from oldest (top) to newest (bottom)
            var startPos = new Point(2, 1);
            for (int i = 0; i < _messages.Count; i++)
            {
                // Print the message at the given position
                var message = _messages[i];
                _messageSurface.Surface.Print(startPos.X, startPos.Y, message);
                startPos += Direction.Down;
            }
        }

        /// <summary>
        /// Adds a new message to the log using a static shortcut.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public static void WriteLine(string message)
        {
            // Static shortcut to add a message
            ScreenContainer.Instance.Messages.AddMessage(message);
        }
    }
}
