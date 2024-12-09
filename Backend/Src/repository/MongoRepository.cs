using Microsoft.EntityFrameworkCore.Storage;
using MongoDB.Driver;
using MongoDB.Bson;

namespace backend.src.repository;
public class MongoRepository<T> : IMongoRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>("puzzles");
    }

    public async Task AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
    }

    public async Task DeleteAsync(string id)
    {
        await _collection.DeleteOneAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(p => true).ToListAsync();
    }


    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
        await _collection.ReplaceOneAsync(filter, entity);
    }
}
