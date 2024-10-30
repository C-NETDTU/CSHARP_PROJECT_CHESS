using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json;
using ThirdParty.Json.LitJson;

namespace src.data.model
{
    public class Puzzle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty!;

        [BsonElement("PuzzleId")]
        public string? PuzzleId { get; set; } = null!;

        [BsonElement("FEN")]
        public string? FEN { get; set; } = null!;

        [BsonElement("Moves")]
        public string? Moves { get; set; } = null!;

        [BsonElement("Rating")]
        public int? Rating { get; set; } = null!;



        [BsonElement("Themes")]
        public string Themes { get; set; } = string.Empty;

    }
}
