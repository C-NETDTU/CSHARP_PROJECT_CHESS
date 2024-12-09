using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace src.data.model
{
    /// <summary>
    ///  Puzzle class model
    /// </summary>
    public class Puzzle
    {
        /// <summary>
        /// Puzzle object id for the database
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonPropertyName("_id")]
        public string Id { get; set; } = string.Empty!;

        /// <summary>
        /// Puzzle lichess api id
        /// </summary>
        [BsonElement("PuzzleId")]
        public string? PuzzleId { get; set; } = null!;

        /// <summary>
        /// FEN string number 
        /// </summary>
        [BsonElement("FEN")]
        public string? FEN { get; set; } = null!;
        /// <summary>
        /// Order of moves in puzzle
        /// </summary>
        [BsonElement("Moves")]
        public string? Moves { get; set; } = null!;
        /// <summary>
        /// Puzzle rating
        /// </summary>
        [BsonElement("Rating")]
        public int Rating { get; set; }
        /// <summary>
        /// Themes present in puzzle
        /// </summary>
        [BsonElement("Themes")]
        public string Themes { get; set; } = string.Empty;

        /// <summary>
        /// Parameterless constructor for Puzzle class
        /// </summary>
        public Puzzle() { }

        /// <summary>
        /// Parameterized constructor for Puzzle class
        /// </summary>
        public Puzzle(string id, string? puzzleId, string? fen, string? moves, int rating, string themes)
        {
            Id = id;
            PuzzleId = puzzleId;
            FEN = fen;
            Moves = moves;
            Rating = rating;
            Themes = themes;
        }
    }
    
}
