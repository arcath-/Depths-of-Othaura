using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System;
using System.Collections.Generic;
using System.Globalization;
using SadRogue.Primitives;
using Newtonsoft.Json;


namespace Depths_of_Othaura.Data.World.Configuration
{
    /// <summary>
    /// Handles tile configuration, loading tile properties from a JSON file.
    /// </summary>
    internal class TilesConfig
    {
        /// <summary>
        /// The foreground color of the tile, stored as a hex string.
        /// </summary>
        public string Foreground { get; set; }

        /// <summary>
        /// The background color of the tile, stored as a hex string.
        /// </summary>
        public string Background { get; set; }

        /// <summary>
        /// The obstruction type of the tile (e.g., walkable, blocked).
        /// </summary>
        public string Obstruction { get; set; }

        /// <summary>
        /// The type of the tile (e.g., floor, wall, door).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The ASCII ID used to represent the tile.
        /// </summary>
        public int AsciiID { get; set; }

        /// <summary>
        /// The Tiles ID used to represent the tile.
        /// </summary>
        public int TileID { get; set; }

        /// <summary>
        /// The glyph used to represent the tile.
        /// </summary>
        public int Glyph { get; set; }

        /// <summary>
        /// Dictionary storing tile configurations by tile type.
        /// </summary>
        private static Dictionary<TileType, Tile> _configTiles;

        /// <summary>
        /// Converts a hex color string to a <see cref="Color"/> object.
        /// </summary>
        /// <param name="hexColor">The hex color string (e.g., "#RRGGBB" or "#RRGGBBAA").</param>
        /// <returns>The corresponding <see cref="Color"/> object.</returns>
        /// <exception cref="ArgumentException">Thrown if the hex color string is null or empty.</exception>
        /// <exception cref="FormatException">Thrown if the hex color string is not in the correct format.</exception>
        private static Color HexToColor(string hexColor)
        {
            if (string.IsNullOrWhiteSpace(hexColor))
                throw new ArgumentException("Hex color cannot be null or empty.");

            hexColor = hexColor.TrimStart('#');

            if (hexColor.Length != 6 && hexColor.Length != 8)
                throw new FormatException("Hex color must be 6 or 8 characters long.");

            // Parse RGB components
            byte r = byte.Parse(hexColor[..2], NumberStyles.HexNumber);
            byte g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);

            // Parse alpha if provided, otherwise default to 255 (fully opaque)
            byte a = hexColor.Length == 8
                ? byte.Parse(hexColor.Substring(6, 2), NumberStyles.HexNumber)
                : (byte)255;

            return new Color(r, g, b, a);
        }

        /// <summary>
        /// Converts a <see cref="TilesConfig"/> object to a <see cref="Tile"/> object.
        /// </summary>
        /// <param name="tileConfig">The <see cref="TilesConfig"/> object to convert.</param>
        /// <returns>The corresponding <see cref="Tile"/> object.</returns>
        private static Tile ConvertFromConfigurationTile(TilesConfig tileConfig)
        {
            return new Tile(Enum.Parse<TileType>(tileConfig.Type, true))
            {
                Foreground = HexToColor(tileConfig.Foreground),
                Background = HexToColor(tileConfig.Background),
                Glyph = (Constants.AsciiRenderMode ? tileConfig.AsciiID : tileConfig.TileID),
                Obstruction = Enum.Parse<ObstructionType>(tileConfig.Obstruction, true),
                AsciiID = tileConfig.AsciiID,
                TileID = tileConfig.TileID
            };
        }

        /// <summary>
        /// Loads the tile configuration from the JSON file specified in <see cref="Constants.TileConfiguration"/>.
        /// </summary>
        private static void LoadConfiguration()
        {
            var tilesJson = File.ReadAllText(Constants.TileConfiguration);
            var tiles = JsonConvert.DeserializeObject<List<TilesConfig>>(tilesJson);

            // Debug: Check if any tiles were loaded
            if (tiles == null || tiles.Count == 0)
            {
                //System.Console.WriteLine("ERROR: No tiles loaded from TileConfiguration file!");
                throw new InvalidOperationException("Tile configuration file is empty or invalid.");
            }
            else
            {
                //System.Console.WriteLine($"Successfully loaded {tiles.Count} tiles from TileConfiguration.");
            }

            _configTiles = tiles.ToDictionary(a => Enum.Parse<TileType>(a.Type, true), ConvertFromConfigurationTile);
        }

        /// <summary>
        /// Gets the <see cref="Tile"/> configuration for the specified <see cref="TileType"/>.
        /// </summary>
        /// <param name="tileType">The <see cref="TileType"/> to get the configuration for.</param>
        /// <returns>The corresponding <see cref="Tile"/> object.</returns>
        /// <exception cref="Exception">Thrown if no configuration is found for the specified <see cref="TileType"/>.</exception>
        public static Tile Get(TileType tileType)
        {
            if (_configTiles == null) LoadConfiguration();
            if (!_configTiles.TryGetValue(tileType, out var tile))
                throw new Exception($"Missing tile configuration for tile type \"{tileType}\".");
            return tile;
        }

        /// <summary>
        /// Provides a custom JSON converter for handling character values.
        /// </summary>
        class CharacterConverter : JsonConverter
        {
            /// <inheritdoc />
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(int);
            }

            /// <inheritdoc />
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string strValue = (string)reader.Value;
                    if (!string.IsNullOrEmpty(strValue) && strValue.Length == 1)
                    {
                        // Convert single character string to its ASCII integer value
                        return (int)strValue[0];
                    }
                    throw new JsonSerializationException("String must contain exactly one character.");
                }
                else if (reader.TokenType == JsonToken.Integer)
                {
                    // Return the integer value as is
                    return Convert.ToInt32(reader.Value);
                }
                throw new JsonSerializationException("Unsupported JSON token for integer or single-character string.");
            }

            /// <inheritdoc />
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Serialize the integer as a number
                writer.WriteValue(value);
            }
        }
    }
}