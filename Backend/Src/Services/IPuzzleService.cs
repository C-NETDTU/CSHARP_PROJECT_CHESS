using src.data.model;
using System.Threading.Tasks;
using System.Collections.Generic;
using Shared.DTO;
namespace src.services
{
    public interface IPuzzleService
    {
        Task<List<PuzzleDTO>> GetAsync(int pageNumber, int pageSize);
        Task<PuzzleDTO?> GetAsyncId(string id);
        Task<List<PuzzleDTO>?> GetAsyncThemes(string themes);
        Task<List<PuzzleDTO>?> GetAsyncRating(int rating);
        Task<PuzzleDTO?> GetAsyncRandom();
        Task<PuzzleDTO?> GetAsyncRandomByCriteria<T>(string criteria, T match);
        Task CreateAsync(Puzzle newPuzzle);
        Task UpdateAsync(string id, Puzzle updatedPuzzle);
        Task RemoveAsync(string id);
    }
}
