using System;
using System.Net.Http;
using System.Threading.Tasks;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessMove;
using Frontend.Model.ChessPiece;
using Shared.DTO;

namespace Frontend.Controller
{
    public class PuzzleManager
    {
        public ApiManager ApiManager;
        public GameManager? GameManager;
        private Queue<PuzzleDTO> puzzleQueue;
        public int strikes;
        private int maxStrikes = 3;
        public int score;
        public int streak;
        private int maxStreak;
        
        public event Action<bool>? OnSurvialCompleted; 

        public PuzzleManager(ApiManager apiManager)
        {
            ApiManager = apiManager;
            puzzleQueue = new Queue<PuzzleDTO>();
            score = 0;
            strikes = 0;
            streak = 0;
            maxStreak = 0;
        }

        public void Initialize()
        {
            try
            {
                var puzzle = new PuzzleDTO
                {
                    Id = "1",
                    PuzzleId = "1",
                    FEN = "3r2k1/1p1B1ppp/p7/3p4/P2P4/4P3/5PPP/2R3K1 b - - 0 22",
                    Moves = "d8d7 c1c8 d7d8 c8d8",
                    Rating = 1000,
                    Themes = ""
                };
                
                var solutionMoves = ParseSolutionToList(puzzle.Moves);

                GameManager = new GameManager(puzzle.FEN, solutionMoves); // Pass the FEN
                Console.WriteLine("GameManager initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PuzzleManager.Initialize: {ex.Message}");
                throw;
            }
        }

        private List<string> ParseSolutionToList(string solution)
        {
            if (string.IsNullOrWhiteSpace(solution))
                throw new ArgumentException("The solution string cannot be null or empty.", nameof(solution));
            
            var solutionList = solution.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();

            return solutionList;
        }
        
        public async Task FetchAndLoadPuzzle()
        {
            try
            {
                Console.WriteLine("PuzzleManager: Fetching a random puzzle...");
                PuzzleDTO? puzzleDto = await ApiManager.RetrieveRandomPuzzle();

                if (puzzleDto != null)
                {
                    Console.WriteLine($"PuzzleManager: Fetched puzzle with FEN: {puzzleDto.FEN} and moves: {puzzleDto.Moves}");
                    
                    var solution = ParseSolutionToList(puzzleDto.Moves);
                    GameManager = new GameManager(puzzleDto.FEN, solution );
                    GameManager.OnPuzzleCompleted += HandlePuzzleCompleted;
                }
                else
                {
                    Console.WriteLine("PuzzleManager: No puzzle returned from the API.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PuzzleManager: Error fetching puzzle: {ex.Message}");
            }
        }

        private void HandlePuzzleCompleted(bool success)
        {
            try
            {
                Console.WriteLine("PuzzleManager: Entered HandlePuzzleCompleted with " + success);

                if (success)
                {
                    score++;
                    OnSurvialCompleted?.Invoke(false);
                }
                else
                {
                    Console.WriteLine("PuzzleManager: Updated strike");
                    strikes++;
                    if (strikes == maxStrikes)
                    {
                        Console.WriteLine("PuzzleManager: Max streaks reached.");
                        OnSurvialCompleted?.Invoke(true);
                    }
                    else
                    {
                        Console.WriteLine("PuzzleManager: One strike made");
                        OnSurvialCompleted?.Invoke(false);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PuzzleManager: Exception in HandlePuzzleCompleted - {ex.Message}");
            }
        }

    }
}