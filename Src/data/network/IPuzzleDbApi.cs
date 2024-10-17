using src.data.model;
using System.Threading.Tasks;
namespace src.data.network
{
	public interface IPuzzleDbApi
	{
		Task<Puzzle> createPuzzle(Puzzle puzzle);
		Task<Puzzle> ReadPuzzle(Puzzle puzzle);
		Task deletePuzzle(Puzzle puzzle);
	}
}