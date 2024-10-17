using src.data.model;
using src.data.network;
using src.data.repositories.MongoDB;
using MongoDB.Driver;

namespace MongoDBApi
{
    public class Program
    {
        static void Main(string[] args)
        {

            var connectionString = "mongodb://localhost:27017/";
                //Environment.GetEnvironmentVariable("MONGODB_URI");
            try
            {
                MongoClient dbClient = new MongoClient(connectionString);
                Console.WriteLine($"Connected to MongoDB: {dbClient}");

                var db = dbClient.GetDatabase("puzzleDB");
                Console.WriteLine($"Database accessed: {db.DatabaseNamespace}");

                var puzzles = db.GetCollection<Puzzle>("puzzles");
                Console.WriteLine($"Collection accessed: {puzzles.CollectionNamespace}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}