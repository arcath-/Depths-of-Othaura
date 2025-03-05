﻿using SadConsole;
using SadRogue.Primitives;
using System.Collections.Generic;
using System.Linq;

// TODO:

namespace Depths_of_Othaura.Data
{
    /// <summary>
    /// Provides extension methods for various data types.
    /// </summary>
    internal static class Extensions
    {
        // ========================= Constants =========================

        /// <summary>
        /// Array of cardinal direction points (North, South, East, West).
        /// </summary>
        private static readonly Point[] _directionalDirections =
        [
            new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1)
        ];

        /// <summary>
        /// Array of diagonal and cardinal direction points.
        /// </summary>
        private static readonly Point[] _diagonalDirections =
        [
            new Point(-1, 0), new Point(1, 0), new Point(0, -1), new Point(0, 1),
            new Point(-1, -1), new Point(-1, 1), new Point(1, -1), new Point(1, 1)
        ];

        // ========================= Math Extensions =========================

        /// <summary>
        /// Calculates the specified percentage of the given integer value.
        /// </summary>
        /// <param name="value">The base value from which the percentage will be calculated.</param>
        /// <param name="percentage">The percentage to calculate from the base value.</param>
        /// <returns>The calculated percentage of the base value as an integer.</returns>
        internal static int PercentageOf(this int value, int percentage)
        {
            return (int)Math.Round(value * percentage / 100.0);
        }

        // ========================= Point Extensions =========================

        /// <summary>
        /// Gets the neighboring points based on the given point.
        /// </summary>
        /// <param name="point">The central point for which neighbors will be retrieved.</param>
        /// <param name="includeDiagonals">Indicates whether diagonal neighbors should be included.</param>
        /// <returns>A collection of neighboring points.</returns>
        internal static IEnumerable<Point> GetNeighborPoints(this Point point, bool includeDiagonals)
        {
            return (includeDiagonals ? _diagonalDirections : _directionalDirections).Select(d => new Point(point.X + d.X, point.Y + d.Y));
        }

        // ========================= CellSurface Extensions =========================

        /// <summary>
        /// Draws a border around the given surface with a title at the top.
        /// </summary>
        /// <param name="surface">The surface on which the border and title will be drawn.</param>
        /// <param name="title">The text to be displayed at the top of the border.</param>
        /// <param name="borderColor">The color of the border.</param>
        /// <param name="titleColor">The color of the title text.</param>
        internal static void DrawBorderWithTitle(this ICellSurface surface, string title, Color borderColor, Color titleColor)
        {
            // Draw borders
            var shapeParams = ShapeParameters.CreateStyledBox(ICellSurface.ConnectedLineThick, new ColoredGlyph(borderColor), ignoreBorderBackground: true);
            surface.DrawBox(new Rectangle(0, 0, surface.Width, surface.Height), shapeParams);

            // Print title
            surface.Print(surface.Width / 2 - title.Length / 2, 0, new ColoredString(title, titleColor, Color.Transparent));
        }
    }
}