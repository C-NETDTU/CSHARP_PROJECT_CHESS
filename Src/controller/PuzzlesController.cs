using src.data.model;
using src.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using DnsClient;

namespace src.controller;

[ApiController]
[Route("api/[controller]")]
public class PuzzleController : ControllerBase
{
    private readonly PuzzleService _puzzleService;
    private readonly ILogger<PuzzleController> _logger;

    public PuzzleController(ILogger<PuzzleController> logger, PuzzleService puzzleService)
    {
        _logger = logger;
        _puzzleService = puzzleService;
    }

    /*
     * this method isn't optimised at all. You should never use this, but it is here for future possible usages. 
     * Since we have over 4mio. elements, this will be extremely slow. Instead, query by id, theme, or random with criteria.
     */
    [HttpGet]
    public async Task<List<Puzzle>> Get() =>
        await _puzzleService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Puzzle>> Get(string id)
    {
        var Puzzle = await _puzzleService.GetAsyncId(id);

        if (Puzzle is null)
        {
            return NotFound();
        }

        return Puzzle;
    }
    


    [HttpPost]
    public async Task<IActionResult> Post(Puzzle newPuzzle)
    {
        await _puzzleService.CreateAsync(newPuzzle);

        return CreatedAtAction(nameof(Get), new { id = newPuzzle.Id }, newPuzzle);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, Puzzle updatedPuzzle)
    {
        var puzzle = await _puzzleService.GetAsyncId(id);

       if (puzzle is null)
        {
            return NotFound();
        }

        updatedPuzzle.Id = puzzle.Id;

        await _puzzleService.UpdateAsync(id, updatedPuzzle);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var Puzzle = await _puzzleService.GetAsyncId(id);

        if (Puzzle is null)
        {
            return NotFound();
        }

        await _puzzleService.RemoveAsync(id);

        return NoContent();
    }


    [HttpGet("themes/{themes}")]
    public async Task<ActionResult<List<Puzzle>>> GetThemes(string themes)
    {
        var puzzles = await _puzzleService.GetAsyncThemes(themes);
        if (puzzles == null || !puzzles.Any())
        {
            _logger.LogCritical($"\n Error in finding puzzles by theme {themes}. Either, no such theme exists, or something went wrong!\n");

            return NotFound();
        }
        _logger.LogInformation($"\nFound: {puzzles.Count()} puzzles with themes {themes}\n");

        return Ok(puzzles);
    }

    [HttpGet("random")]
    public async Task<ActionResult<Puzzle>> GetRandom()
    {
        var randomPuzzle = await _puzzleService.GetAsyncRandom();
        if(randomPuzzle == null)
        {
            _logger.LogCritical($"\n Cannot retrieve random puzzle. Check connection to database!");
            return NotFound();
        }
        return Ok(randomPuzzle);
    }
    [HttpGet("random/themes/{theme}")]
    public async Task<ActionResult<Puzzle>?> GetRandomByTheme(string theme)
    {
        var randomPuzzle = await _puzzleService.GetAsyncRandomByTheme(theme);
        if(randomPuzzle == null)
        {
            _logger.LogCritical($"\n Cannot retrieve random puzzle by themes {theme}! Possibly, no such theme exists.");
            return NotFound();
        }
        return Ok(randomPuzzle);
    }

    [HttpGet("rating/{rating}")]
    public async Task<ActionResult<List<Puzzle>>> GetRating(Int32 rating)
    {
        var puzzles = await _puzzleService.GetAsyncRating(rating);
        if (puzzles == null || !puzzles.Any()) {
            _logger.LogCritical($"\nError in finding puzzles with {rating}. Either no such puzzles exists, or something went wrong!\n");
            return NotFound();
        }
        _logger.LogInformation($"\nFound: {puzzles.Count()} puzzles with rating {rating}\n");
        return Ok(puzzles);
    }
}