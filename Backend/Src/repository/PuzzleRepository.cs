using backend.src.repository;
using MongoDB.Bson;
using MongoDB.Driver;
using src.data.model;
using System.Reflection.PortableExecutable;

namespace backend.src.repository
{
    public class PuzzleRepository : MongoRepository<Puzzle>, IPuzzleRepository
    {
        private readonly IMongoCollection<Puzzle> _collection;

        public PuzzleRepository(IMongoDatabase database) : base(database, "puzzles")
        {
            _collection = database.GetCollection<Puzzle>("puzzles");
        }

        public async Task<IEnumerable<Puzzle>> GetByThemeAsync(string theme)
        {
            var filter = Builders<Puzzle>.Filter.Where(p => p.Themes.Contains(theme));
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Puzzle>> GetByRatingAsync(int rating)
        {
            var filter = Builders<Puzzle>.Filter.Eq(p => p.Rating, rating);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<Puzzle> GetRandomAsync()
        {
            var pipeline = new BsonDocument[]
            {
            new BsonDocument("$sample", new BsonDocument("size", 1))
            };
            var result = await _collection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
            if(result == null)
            {
                Console.WriteLine("No puzzle found.");
                Console.WriteLine($"{_collection.Database.DatabaseNamespace.DatabaseName}");
            }
            return result;
        }

        public async Task<Puzzle> GetRandomByCriteriaAsync<T>(string criteria, T match)
        {
            Puzzle? result = null;
            if (criteria == "Rating" && match is int matchInt)
            {
                var pipelineint = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument(criteria, matchInt)),
                    new BsonDocument("$sample", new BsonDocument("size", 1))
                };
                result = await _collection.Aggregate<Puzzle>(pipelineint).FirstOrDefaultAsync();
                if (result == null)
                {
                    Console.WriteLine($"No puzzle found for criteria: {criteria} with match: {match}");
                }
                return result;
            }
            else if (match is string matchString)
            {
                var pipeline = new BsonDocument[]
                {
                    new BsonDocument("$match", new BsonDocument(criteria, new BsonDocument("$regex", matchString).Add("$options", "i"))),
                    new BsonDocument("$sample", new BsonDocument("size", 1))
                };
                result = await _collection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
                if (result == null)
                {
                    Console.WriteLine($"No puzzle found for criteria: {criteria} with match: {match}");
                }
                return result;
            }
            else
            {
                Console.WriteLine($"No puzzle found.");
                return result;
            }
        }
    }

}
