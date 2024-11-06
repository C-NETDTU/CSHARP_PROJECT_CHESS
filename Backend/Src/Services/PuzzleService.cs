using src.data.model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
namespace src.services;

public class PuzzleService
{
    private readonly IMongoCollection<Puzzle>? _puzzlesCollection;
    public PuzzleService(
        IOptions<PuzzleDBSettings> PuzzleDatabaseSettings
        )
    {
        var mongoClient = new MongoClient(
            PuzzleDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            PuzzleDatabaseSettings.Value.DatabaseName);

        _puzzlesCollection = mongoDatabase.GetCollection<Puzzle>(
            PuzzleDatabaseSettings.Value.PuzzleCollectionName);
    }

    public async Task<List<Puzzle>> GetAsync() =>
        await _puzzlesCollection.Find(_ => true).ToListAsync();

    public async Task<Puzzle?> GetAsyncId(string id)
    {
        var puzzle = await _puzzlesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        return puzzle;

    }
   
    public async Task<List<Puzzle>?> GetAsyncThemes(string themes)
    {
        var filter = Builders<Puzzle>.Filter.Where(p => p.Themes.Contains(themes));
        return await _puzzlesCollection.Find(filter).ToListAsync();
    }
    
    public async Task<List<Puzzle>?> GetAsyncRating(Int32 rating)
    {
        var filter = Builders<Puzzle>.Filter.Where(p => p.Rating == rating);
        return await _puzzlesCollection.Find(filter).ToListAsync();
    }

    public async Task<Puzzle?> GetAsyncRandom()
    {
        var pipeline = new BsonDocument[]
    {
        new BsonDocument("$sample", new BsonDocument("size", 1))
    };

        var result = await _puzzlesCollection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
        return result;
    }
    public async Task<Puzzle?> GetAsyncRandomByCriteria<T>(string criteria, T match)
    {
        Puzzle? result = null;
        if(criteria == "Rating" && match is int matchInt)
        {
            var pipelineint = new BsonDocument[]
            {
            new BsonDocument("$match", new BsonDocument(criteria, matchInt)),
            new BsonDocument("$sample", new BsonDocument("size", 1))
            };
            result = await _puzzlesCollection.Aggregate<Puzzle>(pipelineint).FirstOrDefaultAsync();
            if (result == null)
            {
                Console.WriteLine($"No puzzle found for criteria: {criteria} with match: {match}");
            }
            return result;
        }
        else if(match is string matchString){
        var pipeline = new BsonDocument[]
        {
            new BsonDocument("$match", new BsonDocument(criteria, new BsonDocument("$regex", matchString).Add("$options", "i"))),
            new BsonDocument("$sample", new BsonDocument("size", 1))
        };

        result = await _puzzlesCollection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
        if (result == null){
                Console.WriteLine($"No puzzle found for criteria: {criteria} with match: {match}");
            }
            return result;
        } else {
            Console.WriteLine($"No puzzle found.");
            return result;
        }
    }
    public async Task CreateAsync(Puzzle newPuzzle) =>
        await _puzzlesCollection.InsertOneAsync(newPuzzle);

    public async Task UpdateAsync(string id, Puzzle updatedPuzzle) =>
        await _puzzlesCollection.ReplaceOneAsync(x => x.Id == id, updatedPuzzle);

    public async Task RemoveAsync(string id) =>
        await _puzzlesCollection.DeleteOneAsync(x => x.Id == id);
}