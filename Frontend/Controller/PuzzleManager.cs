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

        public event Action<bool>? OnPuzzleCompleted;

        public event Action? OnPuzzleLoaded;

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
                    foreach (var puzzle in cacheWrapper.Puzzles)
                    {
                        puzzleQueue.Enqueue(puzzle);
                    }
                    strikes = cacheWrapper.Strikes;
                    streak = cacheWrapper.Streak;
                }
                else
                {
                    var fastP = await ApiManager.RetrieveRandomPuzzle();
                    puzzleQueue.Enqueue(fastP);
                    var cache = await FetchPuzzles();
                    var saveData = new PuzzleGameState(cache,strikes, streak);
                    await _storageService.SaveAsync("savedGames.json", saveData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in PuzzleManager.Initialize: {ex.Message}");
            }
        }

        public async Task<List<PuzzleDTO>> GetQueue()
        {
            try
            {
                var tasks = new List<Task<PuzzleDTO?>>();
                puzzleQueue.TryGetNonEnumeratedCount(out var count);
                for (int i = 0; i < count; i++)
                {
                    tasks.Add(ApiManager.RetrieveRandomPuzzle());
                }
                var puzzles = await Task.WhenAll(tasks);
                return [.. puzzles];

            }
            catch (Exception ex) { 
                _logger.LogError($"Err: {ex}");
                return new List<PuzzleDTO>();
            }
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
                PuzzleDTO puzzle = new PuzzleDTO();
                await Task.Run(() => { puzzleQueue.TryDequeue(out var puzzle); });
                if (puzzle != null)
                {
                    _logger.LogInformation($"Sucessfully d-q puzzle: {puzzle}");
                    return;
                }
            }catch(Exception ex) { _logger.LogError($"Err: {ex}"); }
        }

        public async Task FetchAndLoadPuzzle()
        {
            try
            {
                _logger.LogInformation("PuzzleManager: Fetching a puzzle...");
                PuzzleDTO? puzzleDto;
                if (!puzzleQueue.TryPeek(out puzzleDto))
                {
                    puzzleDto = await ApiManager.RetrieveRandomPuzzle();
                    _logger.LogInformation("Fetched new puzzle from the API.");
                }
                else
                {
                    _logger.LogInformation($"Fetched puzzle from the local queue: {puzzleDto.FEN}");
                }
                if (puzzleDto != null)
                {
                    _logger.LogInformation($"PuzzleManager: Fetched puzzle with FEN: {puzzleDto.FEN}");
                    GameManager = new GameManager(puzzleDto.FEN);
                    OnPuzzleLoaded?.Invoke(); 
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

    }
}