using src.data.model;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace src.services
{
    public interface IPuzzleService
    {
        Task<List<Puzzle>> GetAsync(int pageNumber, int pageSize);
        Task<Puzzle?> GetAsyncId(string id);
        Task<List<Puzzle>?> GetAsyncThemes(string themes);
        Task<List<Puzzle>?> GetAsyncRating(int rating);
        Task<Puzzle?> GetAsyncRandom();
        Task<Puzzle?> GetAsyncRandomByCriteria<T>(string criteria, T match);
        Task CreateAsync(Puzzle newPuzzle);
        Task UpdateAsync(string id, Puzzle updatedPuzzle);
        Task RemoveAsync(string id);
    }
}
