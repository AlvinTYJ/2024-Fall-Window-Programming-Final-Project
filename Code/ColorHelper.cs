using System;
using System.Drawing;
using System.Drawing.Printing;

namespace Final_Project.Extensions
{
    public static class ColorHelper
    {
        /// <summary>
        /// Converts a hexadecimal color code to a System.Drawing.Color.
        /// </summary>
        /// <param name="hex">The hexadecimal color code, e.g., "#RRGGBB" or "RRGGBB".</param>
        /// <returns>A System.Drawing.Color representing the given hex code.</returns>
        public static Color FromHEX(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Hex color cannot be null or empty.", nameof(hex));

            // Remove '#' if present
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            if (hex.Length != 6)
                throw new ArgumentException("Hex color must be 6 characters long.", nameof(hex));

            // Parse RGB values from the hex string
            int r = int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            int g = int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            int b = int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            Console.WriteLine("rgb:{0}, {1}, {2}",r,g,b);

            return Color.FromArgb(r, g, b);
        }
    }
}
