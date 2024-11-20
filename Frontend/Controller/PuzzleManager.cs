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
        private readonly HttpClient _httpClient = new HttpClient(new HttpClientHandler())
        {
            BaseAddress = new Uri("http://localhost:52474")
        };

        public GameManager? GameManager;
        
        private Queue<PuzzleDTO> puzzleQueue;
        public int strikes;
        private int maxStrikes = 3;
        public int score;
        public int streak;
        private int maxStreak;

        public event Action<bool>? OnPuzzleCompleted;

        public event Action? OnNewPuzzleLoaded;

        public PuzzleManager()
        {
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
                    FEN = "rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1",
                    Moves = "",
                    Rating = 1000,
                    Themes = ""
                };

                GameManager = new GameManager(puzzle.FEN); // Pass the FEN
                Console.WriteLine("GameManager initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PuzzleManager.Initialize: {ex.Message}");
                throw; // Rethrow to let Blazor show the error (or handle it gracefully)
            }
        }
    }
}