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
        public string Id { get; set; } = null!;

        [BsonElement("PuzzleId")]
        public string? PuzzleId { get; set; } = null!;

        [BsonElement("FEN")]
        public string? FEN { get; set; } = null!;

        [BsonElement("Moves")]
        public string? Moves { get; set; } = null!;

        [BsonElement("Rating")]
        public int? Rating { get; set; } = null!;

        [BsonElement("RatingDeviation")]
        public int? RatingDeviation { get; set; } = null!;

        [BsonElement("Popularity")]
        public int? Popularity { get; set; } = null!;

        [BsonElement("NbPlays")]
        public int? NbPlays { get; set; } = null!;

        [BsonElement("Themes")]
        public string? Themes { get; set; } = null!;

        [BsonElement("GameUrl")]
        public string? GameUrl { get; set; } = null!;

        [BsonElement("OpeningTags")]
        public string? OpeningTags { get; set; } = null!;
        /*
        [BsonConstructor]
        public Puzzle(string Id, string PuzzleId, string FEN, string Moves, int Rating, int RatingDeviation, int Popularity, int NbPlays, string Themes, string GameUrl)
        {
            this.Id = Id;
            this.PuzzleId = PuzzleId;
            this.FEN = FEN;
            this.Moves = Moves;
            this.Rating = Rating;
            this.RatingDeviation = RatingDeviation;
            this.Popularity = Popularity;
            this.NbPlays = NbPlays;
            this.Themes = Themes;
            this.GameUrl = GameUrl;
        }
        */
    }
}
