using src.data.model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq;
using MongoDB.Bson;
using backend.src.repository;
using Shared.DTO;
namespace src.services;

public class PuzzleService : IPuzzleService
{
    private readonly IPuzzleRepository _puzzleRepository;
    public PuzzleService(IPuzzleRepository puzzleRepository)
    {
        _puzzleRepository = puzzleRepository;
    }

    public async Task<List<PuzzleDTO>> GetAsync(int pageNumber, int pageSize)
    {
        var puzzles = await _puzzleRepository.GetAllAsync();

        List<PuzzleDTO> PuzzleDTOs = puzzles.Select(p => new PuzzleDTO(p.Id, p.PuzzleId, p.FEN, p.Moves, p.Rating, p.Themes)).ToList();

        return PuzzleDTOs
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

    }

    public async Task<PuzzleDTO> GetAsyncId(string id)
    {
        var result = await _puzzleRepository.GetByIdAsync(id);
        PuzzleDTO pDTO = new PuzzleDTO(result.Id,result.PuzzleId,result.FEN,result.Moves,result.Rating,result.Themes);
        return pDTO;

    }
   
    public async Task<List<PuzzleDTO>?> GetAsyncThemes(string themes)
    {
        var result = (await _puzzleRepository.GetByThemeAsync(themes)).ToList();
        List<PuzzleDTO> pDTO = new List<PuzzleDTO>();
        result.ForEach(p => pDTO.Add(new PuzzleDTO(p.Id, p.PuzzleId, p.FEN, p.Moves, p.Rating, p.Themes)));
        return pDTO;
    }
    
    public async Task<List<PuzzleDTO>?> GetAsyncRating(Int32 rating)
    {
        var result = (await _puzzleRepository.GetByRatingAsync(rating)).ToList();
        List<PuzzleDTO> pDTO = new List<PuzzleDTO>();
        result.ForEach(p => pDTO.Add(new PuzzleDTO(p.Id,p.PuzzleId,p.FEN,p.Moves,p.Rating,p.Themes)));
        return pDTO;
    }

    public async Task<PuzzleDTO?> GetAsyncRandom()
    {
        /*var pipeline = new BsonDocument[]
    {
        new BsonDocument("$sample", new BsonDocument("size", 1))
    };

        var result = await _puzzlesCollection.Aggregate<Puzzle>(pipeline).FirstOrDefaultAsync();
        return result;*/
        var result = await _puzzleRepository.GetRandomAsync();
        return new PuzzleDTO(result.Id, result.PuzzleId, result.FEN, result.Moves, result.Rating, result.Themes);
    }
    public async Task<PuzzleDTO?> GetAsyncRandomByCriteria<T>(string criteria, T match)
    {
        var result = await _puzzleRepository.GetRandomByCriteriaAsync<T>(criteria, match);
        PuzzleDTO pDTO = new PuzzleDTO(result.Id, result.PuzzleId, result.FEN, result.Moves, result.Rating, result.Themes);
        return pDTO;
  
    }
    public async Task CreateAsync(Puzzle newPuzzle) =>
        await _puzzleRepository.AddAsync(newPuzzle);

    public async Task UpdateAsync(string id, Puzzle updatedPuzzle) =>
        await _puzzleRepository.UpdateAsync(id, updatedPuzzle);

    public async Task RemoveAsync(string id) =>
        await _puzzleRepository.DeleteAsync(id);
}