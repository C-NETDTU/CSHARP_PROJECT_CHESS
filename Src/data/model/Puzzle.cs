using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace src.data.model
{
    public class Puzzle
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("PuzzleId")]
        public string PuzzleId { get; set; }

        [BsonElement("FEN")]
        public string FEN { get; set; }

        [BsonElement("Moves")]
        public string Moves { get; set; }

        [BsonElement("Rating")]
        public int Rating { get; set; }

        [BsonElement("RatingDeviation")]
        public int RatingDeviation { get; set; }

        [BsonElement("Popularity")]
        public int Popularity { get; set; }

        [BsonElement("NbPlays")]
        public int NbPlays { get; set; }

        [BsonElement("Themes")]
        public string Themes { get; set; }

        [BsonElement("GameUrl")]
        public string GameUrl { get; set; }

        public Puzzle(ObjectId id, string puzzleId, string fen, string moves, int rating, int ratingDeviation, int popularity, int nbPlays, string themes, string gameUrl)
        {
            Id = id;
            PuzzleId = puzzleId;
            FEN = fen;
            Moves = moves;
            Rating = rating;
            RatingDeviation = ratingDeviation;
            Popularity = popularity;
            NbPlays = nbPlays;
            Themes = themes;
            GameUrl = gameUrl;
        }
    }
}
