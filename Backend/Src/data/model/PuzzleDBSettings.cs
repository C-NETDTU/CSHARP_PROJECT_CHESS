namespace src.data.model
{
    public class PuzzleDBSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017/!";

        public string DatabaseName { get; set; } = "puzzleDB";

        public string PuzzleCollectionName { get; set; } = "puzzles";
    }

}
