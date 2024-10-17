using src.data.model;
namespace src.data.repositories.MongoDB
{
	public interface IPuzzleService
	{
		Task CreatePuzzle(Puzzle puzzle);
		Task ReadPuzzle(string id);
		Task DeletePuzzle(Puzzle puzzle);
	}
}