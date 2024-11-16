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

        private GameManager? gameManager;
        private ApiManager? apiManager;
        public Queue<Puzzle> puzzleQueue;
        public int strikes;
        private int maxStrikes = 3;
        public int score;
        public int streak;
        private int maxStreak;

        public event Action<bool>? OnPuzzleCompleted;

        public event Action? OnNewPuzzleLoaded;

        public PuzzleManager()
        {
            apiManager = new ApiManager(_httpClient);
            puzzleQueue = new Queue<Puzzle>();
            score = 0;
            strikes = 0;
            streak = 0;
            maxStreak = 0;
        }

        public async Task Initialize()
        {
            if (puzzleQueue.Count == 0)
            {
                await FetchPuzzleAsync();
            }
            else
            {
                await LoadNextPuzzle();
            }
        }

        public async Task FetchPuzzleAsync()
        {

            var puzzles = await FetchPuzzlesFromAPI();
            foreach (var puzzle in puzzles)
            {
                puzzleQueue.Enqueue(puzzle);
            }
            await LoadNextPuzzle();
        }
        private Puzzle ParseToPuzzleFromDTO(PuzzleDTO puzzleDTO)
        { 
            return new Puzzle(puzzleDTO.FEN, puzzleDTO.Moves);
        }


        private async Task<List<Puzzle>> FetchPuzzlesFromAPI()
        {
            var tasks = Enumerable.Range(0, 10).Select(_ => apiManager.RetrieveRandomPuzzle());
            var puzzleDTOs = await Task.WhenAll(tasks);
            return puzzleDTOs.Select(ParseToPuzzleFromDTO).ToList();
        }


        private async Task LoadNextPuzzle()
        {
            if (puzzleQueue.Count <= 3)
            {
                await FetchPuzzleAsync();
                return;
            }

            var nextPuzzle = puzzleQueue.Dequeue();
            StartPuzzle(nextPuzzle);
        }

        private void StartPuzzle(Puzzle puzzle)
        {
            if (gameManager != null)
            {
                gameManager.OnPuzzleCompleted -= OnPuzzleCompletedHandler;
            }
            var solutionMoves = ParseSolutionMoves(puzzle.Solution);

            gameManager = new GameManager(puzzle.FEN, solutionMoves);

            gameManager.OnPuzzleCompleted += OnPuzzleCompletedHandler;
            OnNewPuzzleLoaded?.Invoke(); // This is an event that the UI listens to, so that it can update the board with the new puzzle
        }

        private List<string> ParseSolutionMoves(string solution)
        {
            return solution.Split(' ').ToList();
        }

        public async void OnPuzzleCompletedHandler(bool success)
        {
            if (success)
            {
                score++;
                streak++;
                maxStreak = Math.Max(streak, maxStreak);
            }
            else
            {
                strikes++;
                streak = 0;

                if (strikes >= maxStrikes)
                {
                    EndSession();
                    return;
                }
            }

            OnPuzzleCompleted?.Invoke(success);
            await LoadNextPuzzle();
        }

        public void EndSession()
        {
            // We need to notify the UI that the session has ended
            // Reset the score, streak, and strikes
            score = 0;
            strikes = 0;
            streak = 0;
        }

        public async Task ResetSession()
        {
            // We call this when we want to start a new session
            score = 0;
            strikes = 0;
            streak = 0;
            await LoadNextPuzzle();
        }

        public void SetTestPuzzleData(List<Puzzle> puzzles)
        {
            puzzleQueue = new Queue<Puzzle>(puzzles);
        }

    }

    public class Puzzle
    {
        public string FEN { get; set; }
        public string Solution { get; set; }

        public Puzzle(string fen, string solution)
        {
            FEN = fen;
            Solution = solution;
        }
    }
}