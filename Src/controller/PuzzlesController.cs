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


    [HttpGet("themes")]
    public async Task<ActionResult<List<Puzzle>>> GetThemes([FromQuery] string themes)
    {
        var puzzles = await _puzzleService.GetAsyncThemes(themes);
        if (puzzles == null || !puzzles.Any())
        {
            _logger.LogCritical($"\n Error in finding puzzles by theme {themes}. Either, no such theme exists, or something went wrong!\0");

            return NotFound();
        }
        _logger.LogInformation($"\nFound: {puzzles.Count()} puzzles with themes {themes}\0");

        return Ok(puzzles);
    }
    /*
     I think this is the goto way to retrieve puzzles. We can retrieve a random puzzle through the use of query paramters, as such:
        localhost:<port>/api/puzzle/random?criteria={criteria}&match={match}. An example could be querying for themes, as such:
        .../random?criteria=Themes&match=discoveredAttack endGame 
     Please note that if you query by themes, they need to be in alpabetic order.
     */
    [HttpGet("random")]
    public async Task<ActionResult<Puzzle>> GetRandomByCriteria([FromQuery] string? criteria, [FromQuery] string? match)
    {
        if (criteria != null && match != null)
        {
            switch (criteria)
            {
                case "Themes":
                    return Ok(await _puzzleService.GetAsyncRandomByCriteria(criteria, match));
                case "Rating":
                    try
                    {
                        Int32.TryParse(match, out int matchInt);
                        return Ok(await _puzzleService.GetAsyncRandomByCriteria(criteria, matchInt));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical($"\n error: {ex}. Trouble parsin int. Make sure parsed value is int.\0");
                        return BadRequest();
                    }
                case "FEN":
                    match = Uri.UnescapeDataString(match);
                    _logger.LogInformation($"\n Decoded: {match}\0"); 
                    return Ok(await _puzzleService.GetAsyncRandomByCriteria(criteria, match));
                default:
                    _logger.LogError("\n Escaped switch-case. Criteria possibly non-existant.\0");
                    return BadRequest();
            }
        }
        return Ok(await _puzzleService.GetAsyncRandom());
    }

    [HttpGet("rating")]
    public async Task<ActionResult<List<Puzzle>>> GetRating([FromQuery] Int32 rating)
    {
        var puzzles = await _puzzleService.GetAsyncRating(rating);
        if (puzzles == null || !puzzles.Any()) {
            _logger.LogCritical($"\nError in finding puzzles with {rating}. Either no such puzzles exists, or something went wrong!\0");
            return NotFound();
        }
        _logger.LogInformation($"\nFound: {puzzles.Count()} puzzles with rating {rating}\0");
        return Ok(puzzles);
    }
}