using System.Collections.Concurrent;
using Shared.DTO;
using Frontend.Services;
using Microsoft.Extensions.Logging;
using Frontend.Model.DTO;

namespace Frontend.Controller
{
    public class PuzzleManager
    {
        public ApiManager ApiManager;
        public GameManager? GameManager;
        private ConcurrentQueue<PuzzleDTO> puzzleQueue;
        private readonly IFileStorageService _storageService;
        private readonly ILogger _logger;
        public int strikes;
        private int maxStrikes = 3;
        public int score;
        public int streak;
        private int maxStreak;
        
        public event Action<bool>? OnSurvialCompleted; 

        public PuzzleManager(ApiManager apiManager, IFileStorageService fileStorageService, ILogger<PuzzleManager> logger)
        {
            ApiManager = apiManager;
            _storageService = fileStorageService;
            _logger = logger;
            puzzleQueue = new ConcurrentQueue<PuzzleDTO>();
            score = 0;
            strikes = 0;
            streak = 0;
            maxStreak = 0;
        }

        public async Task Initialize()
        {
            try
            {
                var cacheWrapper = await _storageService.LoadAsync<PuzzleGameState>("savedGames.json");

                if (cacheWrapper != null && cacheWrapper.Puzzles != null)
                {
                    foreach (var puzzle in cacheWrapper.Puzzles.Where(p => p != null))
                    {
                        puzzleQueue.Enqueue(puzzle);
                    }
                    strikes = cacheWrapper.Strikes;
                    streak = cacheWrapper.Streak;
                }
                else
                {
                    var fastP = await ApiManager.RetrieveRandomPuzzle();
                    if (fastP == null) {
                        _logger.LogCritical("Error in retrieving puzzle. Check connection.");
                        return;
                    }
                    puzzleQueue.Enqueue(fastP);
                    var cache = await FetchPuzzles();
                    cache.Insert(0,fastP);
                    var saveData = new PuzzleGameState(cache,strikes, streak);
                    await _storageService.SaveAsync("savedGames.json", saveData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PuzzleManager.Initialize: {ex.Message}");
            }
        }

        public List<PuzzleDTO> GetQueue()
        {
            var puzzles = new List<PuzzleDTO>();
            while (puzzleQueue.TryDequeue(out var puzzle))
            {
                if (puzzle != null)
                {
                    puzzles.Add(puzzle);
                }
                else
                {
                    _logger.LogWarning("Encountered a null puzzle in the queue.");
                }
            }
            return puzzles;
        }

        public async Task<List<PuzzleDTO>> FetchPuzzles(int amount = 10)
        {
            try
            {
                _logger.LogInformation("PuzzleManager: Fetching 10 puzzles...");
                var tasks = new List<Task<PuzzleDTO?>>();
                for (int i = 0; i < amount; i++)
                {
                    tasks.Add(ApiManager.RetrieveRandomPuzzle());
                }
                var puzzles = await Task.WhenAll(tasks);
                _logger.LogInformation($"Puzzles fetched successfully. Total puzzles in queue: {puzzleQueue.Count}");
                return [.. puzzles];
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching puzzles: {ex.Message}");
                return [];
            }
        }

        //Idea is to call this method and the DequeuePuzzle when a puzzle has been completed.
        public async Task FetchAndAddPuzzle()
        {
            try {
                var puzzle = await ApiManager.RetrieveRandomPuzzle();
                puzzleQueue.Enqueue(puzzle);
            } catch (Exception ex) { _logger.LogError($"Err: {ex}"); }
        }
        public async Task DequeuePuzzle()
        {
            try
            {

                await Task.Run(() =>
                {
                    PuzzleDTO pDTO = new PuzzleDTO();
                    puzzleQueue.TryDequeue(out var puzzle);
                    if (puzzle != null)
                    {
                        _logger.LogInformation($"Sucessfully d-q puzzle: {puzzle}");
                    }
                });
            }
            catch(Exception ex) { _logger.LogError($"Err: {ex}"); }
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
                _logger.LogInformation("PuzzleManager: Fetching a puzzle...");
                if (!puzzleQueue.TryPeek(out var puzzleDto))
                {
                    puzzleDto = await ApiManager.RetrieveRandomPuzzle();
                    if(puzzleDto == null)
                    {
                        throw new Exception("Invalid puzzle! Check connection.");
                    }
                    _logger.LogInformation("Fetched new puzzle from the API.");
                }
                else
                {
                    _logger.LogInformation($"Fetched puzzle from the local queue: {puzzleDto.FEN}");
                }
                if (puzzleDto != null)
                {
                    _logger.LogInformation($"PuzzleManager: Fetched puzzle with FEN: {puzzleDto.FEN} and moves: {puzzleDto.Moves}");

                    var solution = ParseSolutionToList(puzzleDto.Moves);
                    GameManager = new GameManager(puzzleDto.FEN, solution);
                    GameManager.OnPuzzleCompleted += HandlePuzzleCompleted;
                }
                else
                {
                    _logger.LogInformation("PuzzleManager: No puzzle returned from the API.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"PuzzleManager: Error fetching puzzle: {ex.Message}");
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
                    _logger.LogInformation("PuzzleManager: Updated strike");
                    strikes++;
                    if (strikes == maxStrikes)
                    {
                        _logger.LogInformation("PuzzleManager: Max strikes reached.");
                        OnSurvialCompleted?.Invoke(true);
                    }
                    else
                    {
                        _logger.LogInformation("PuzzleManager: One strike made");
                        OnSurvialCompleted?.Invoke(false);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"PuzzleManager: Exception in HandlePuzzleCompleted - {ex.Message}");
            }
        }

    }
}