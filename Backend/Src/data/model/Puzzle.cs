using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;
using ThirdParty.Json.LitJson;

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
        public int? Rating { get; set; } = null!;
        /// <summary>
        /// Themes present in puzzle
        /// </summary>
        [BsonElement("Themes")]
        public string Themes { get; set; } = string.Empty;

    }
}
