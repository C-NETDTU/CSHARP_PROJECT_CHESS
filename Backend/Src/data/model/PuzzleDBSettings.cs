namespace src.data.model
{
    public class PuzzleDBSettings
    {
        public required string ConnectionString { get; set; } = "mongodb://localhost:27017/";

        public required string DatabaseName { get; set; } = "puzzleDB";

        public required string PuzzleCollectionName { get; set; } = "puzzles";
    }

}
