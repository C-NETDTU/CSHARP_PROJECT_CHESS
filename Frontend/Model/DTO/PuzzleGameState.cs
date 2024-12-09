using Shared.DTO;

namespace Frontend.Model.DTO
{
    public class PuzzleGameState
    {
        public List<PuzzleDTO> Puzzles { get; set; }
        public int Strikes { get; set; }
        public int Streak { get; set; }

        public PuzzleGameState(List<PuzzleDTO> puzzles, int strikes, int streak)
        {
            Puzzles = puzzles;
            Strikes = strikes;
            Streak = streak;
        }
    }
}
