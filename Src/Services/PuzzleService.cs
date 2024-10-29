using src.data.model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

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

    public async Task<Puzzle?> GetAsyncId(string id) =>
        await _puzzlesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    public async Task<List<Puzzle>?> GetAsyncThemes(string Themes)
    {
        var filter = Builders<Puzzle>.Filter.Regex("Themes",new MongoDB.Bson.BsonRegularExpression($".*{Themes},? .*","i"));
        return await _puzzlesCollection.Find(filter).ToListAsync();
       
    }

    public async Task CreateAsync(Puzzle newPuzzle) =>
        await _puzzlesCollection.InsertOneAsync(newPuzzle);

    public async Task UpdateAsync(string id, Puzzle updatedPuzzle) =>
        await _puzzlesCollection.ReplaceOneAsync(x => x.Id == id, updatedPuzzle);

    public async Task RemoveAsync(string id) =>
        await _puzzlesCollection.DeleteOneAsync(x => x.Id == id);
}