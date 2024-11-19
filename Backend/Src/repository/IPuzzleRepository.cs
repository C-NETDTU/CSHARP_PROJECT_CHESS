using src.data.model;

namespace backend.src.repository;

public interface IPuzzleRepository : IMongoRepository<Puzzle>
{
    Task<IEnumerable<Puzzle>> GetByThemeAsync(string themes);
    Task<IEnumerable<Puzzle>> GetByRatingAsync(int rating);
    Task<Puzzle> GetRandomAsync();
    Task<Puzzle> GetRandomByCriteriaAsync<T>(string criteria, T match);

}
