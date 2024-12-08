namespace src.data.model
{
    public class PuzzleDBSettings
    {
        public string ConnectionString { get; set; } = "mongodb+srv://dbUser:dbUserPassword@puzzledb.fm17g.mongodb.net/?retryWrites=true&w=majority&appName=PuzzleDB\r\n";

        public string DatabaseName { get; set; } = "puzzleDB";

        public string PuzzleCollectionName { get; set; } = "puzzles";
    }

}
