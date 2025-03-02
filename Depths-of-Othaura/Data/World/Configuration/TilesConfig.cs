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
        /// The ASCII character used to represent the tile.
        /// </summary>
        [JsonConverter(typeof(CharacterConverter))]
        public int Ascii { get; set; }

        /// <summary>
        /// Dictionary storing tile configurations by tile type.
        /// </summary>
        private static Dictionary<TileType, Tile> _configTiles;

        /// <summary>
        /// Retrieves the tile configuration for a given tile type.
        /// </summary>
        /// <param name="tileType">The type of tile to retrieve.</param>
        /// <returns>A tile instance configured with the specified tile type.</returns>
        /// <exception cref="Exception">Thrown if the tile type is not found in the configuration.</exception>
        public static Tile Get(TileType tileType)
        {
            if (_configTiles == null) LoadConfiguration();
            if (!_configTiles.TryGetValue(tileType, out var tile))
                throw new Exception($"Missing tile configuration for tile type \"{tileType}\".");
            return tile;
        }

        /// <summary>
        /// Loads tile configurations from the JSON configuration file.
        /// </summary>
        private static void LoadConfiguration()
        {
            var tilesJson = File.ReadAllText(Constants.TileConfiguration);
            var tiles = JsonConvert.DeserializeObject<List<TilesConfig>>(tilesJson);
            _configTiles = tiles.ToDictionary(a => Enum.Parse<TileType>(a.Type, true), ConvertFromConfigurationTile);
        }

        /// <summary>
        /// Converts a tile configuration entry into a tile instance.
        /// </summary>
        /// <param name="tileConfig">The tile configuration data.</param>
        /// <returns>A new <see cref="Tile"/> instance with the properties from the configuration.</returns>
        private static Tile ConvertFromConfigurationTile(TilesConfig tileConfig)
        {
            return new Tile(Enum.Parse<TileType>(tileConfig.Type, true))
            {
                Foreground = HexToColor(tileConfig.Foreground),
                Background = HexToColor(tileConfig.Background),
                Glyph = tileConfig.Ascii,
                Obstruction = Enum.Parse<ObstructionType>(tileConfig.Obstruction, true)
            };
        }

        /// <summary>
        /// Converts a hexadecimal color string to a <see cref="Color"/> object.
        /// </summary>
        /// <param name="hexColor">The hex color string.</param>
        /// <returns>A <see cref="Color"/> object representing the color.</returns>
        /// <exception cref="ArgumentException">Thrown if the input string is null or empty.</exception>
        /// <exception cref="FormatException">Thrown if the hex color format is invalid.</exception>
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
        /// Custom JSON converter for handling character glyphs stored as integers.
        /// </summary>
        class CharacterConverter : JsonConverter
        {
            /// <inheritdoc/>
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(int);
            }

            /// <inheritdoc/>
            /// <summary>
            /// Reads a JSON value and converts it into an integer glyph.
            /// </summary>
            /// <exception cref="JsonSerializationException">Thrown if the JSON value is invalid.</exception>
            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.String)
                {
                    string strValue = (string)reader.Value;
                    if (!string.IsNullOrEmpty(strValue) && strValue.Length == 1)
                    {
                        return (int)strValue[0]; // Convert character to ASCII integer value
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

            /// <inheritdoc/>
            /// <summary>
            /// Writes an integer glyph as a JSON value.
            /// </summary>
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Serialize the integer as a number
                writer.WriteValue(value);
            }
        }
    }
}
