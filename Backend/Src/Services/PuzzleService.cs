using src.data.model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
using backend.src.repository;
namespace src.services;

public class PuzzleService : IPuzzleService
{
    private readonly IPuzzleRepository _puzzleRepository;
    public PuzzleService(IPuzzleRepository puzzleRepository)
    {
        _puzzleRepository = puzzleRepository;
    }

    public async Task<List<Puzzle>> GetAsync(int pageNumber, int pageSize)
    {
        var puzzles = await _puzzleRepository.GetAllAsync();
        
        return puzzles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public async Task<Puzzle?> GetAsyncId(string id)
    {
        var puzzle = await _puzzleRepository.GetByIdAsync(id);
        return puzzle;

    }
   
    public async Task<List<Puzzle>?> GetAsyncThemes(string themes)
    {
        return (await _puzzleRepository.GetByThemeAsync(themes)).ToList();
    }
    
    public async Task<List<Puzzle>?> GetAsyncRating(Int32 rating)
    {
        return (await _puzzleRepository.GetByRatingAsync(rating)).ToList();
    }

    public async Task<Puzzle?> GetAsyncRandom()
    {
        /*var pipeline = new BsonDocument[]
    {
        new BsonDocument("$sample", new BsonDocument("size", 1))
    };

        var result = await _puzzlesCollection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
        return result;*/
        return await _puzzleRepository.GetRandomAsync();
    }
    public async Task<Puzzle?> GetAsyncRandomByCriteria<T>(string criteria, T match)
    {
        return await _puzzleRepository.GetRandomByCriteriaAsync<T>(criteria, match);
  
    }
    public async Task CreateAsync(Puzzle newPuzzle) =>
        await _puzzleRepository.AddAsync(newPuzzle);

    public async Task UpdateAsync(string id, Puzzle updatedPuzzle) =>
        await _puzzleRepository.UpdateAsync(id, updatedPuzzle);

    public async Task RemoveAsync(string id) =>
        await _puzzleRepository.DeleteAsync(id);
}