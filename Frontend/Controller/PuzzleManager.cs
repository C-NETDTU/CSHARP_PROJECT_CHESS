using System;
using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading.Tasks;
using Frontend.Model.ChessBoard;
using Frontend.Model.ChessMove;
using Frontend.Model.ChessPiece;
using Shared.DTO;
using Frontend.Services;
using Microsoft.Extensions.Logging;

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
                var cache = await _storageService.LoadAsync<List<PuzzleDTO>>("savedGames.json");
                if (cache == null)
                {
                    var fastP = await ApiManager.RetrieveRandomPuzzle();
                    puzzleQueue.Enqueue(fastP);
                    cache = await FetchPuzzles();
                    await _storageService.SaveAsync("savedGames.json", cache);
                }
                foreach (var puzzle in cache)
                {
                    puzzleQueue.Enqueue(puzzle);
                }
                return;
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
                await _storageService.SaveAsync("savedGames.json",puzzles);
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