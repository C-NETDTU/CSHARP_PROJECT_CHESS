using src.data.model;
using src.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using DnsClient;
using Shared.DTO;
using System.Reflection.PortableExecutable;

namespace src.controller;

[ApiController]
[Route("api/[controller]")]
public class PuzzleController : ControllerBase
{
    private readonly IPuzzleService _puzzleService;
    private readonly ILogger<PuzzleController> _logger;


    public PuzzleController(ILogger<PuzzleController> logger, IPuzzleService puzzleService)
    {
        _logger = logger;
        _puzzleService = puzzleService;
    }

  
    /// <summary>
    /// Method to return a large list of puzzles using paginiation.
    /// </summary>
    /// <param name="pageNumber"/>
    /// <param name="pageSize"/>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<List<PuzzleDTO>>> Get([FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 100)
    {
        List<PuzzleDTO> puzzles = await _puzzleService.GetAsync(pageNumber,pageSize);
        if(puzzles is null)
        {
            return NotFound();
        }
        return Ok(puzzles);
    }
    /// <summary>
    /// Gets a specific puzzle with matching object id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Puzzle>> Get(string id)
    {
        var Puzzle = await _puzzleService.GetAsyncId(id);

        if (Puzzle is null)
        {
            return NotFound();
        }
        return Ok(Puzzle);
    }


    /// <summary>
    /// Post a new puzzle
    /// </summary>
    /// <param name="newPuzzle"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Post(PuzzleDTO newPuzzle)
    {
        Puzzle p = new Puzzle(newPuzzle.Id, newPuzzle.PuzzleId, newPuzzle.FEN, newPuzzle.Moves, newPuzzle.Rating, newPuzzle.Themes);
        await _puzzleService.CreateAsync(p);

        return CreatedAtAction(nameof(Get), new { id = newPuzzle.Id }, newPuzzle);
    }
    /// <summary>
    /// This updates information in an existing puzzle, that matches the id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="updatedPuzzle"></param>
    /// <returns></returns>
    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, PuzzleDTO updatedPDTO)
    {
        Puzzle updatedPuzzle = new Puzzle(updatedPDTO.Id, updatedPDTO.PuzzleId, updatedPDTO.FEN, updatedPDTO.Moves, updatedPDTO.Rating, updatedPDTO.Themes);
        var puzzle = await _puzzleService.GetAsyncId(id);

        if (puzzle is null)
        {
            return NotFound();
        }

        updatedPuzzle.Id = puzzle.Id;

        await _puzzleService.UpdateAsync(id, updatedPuzzle);

        return NoContent();
    }

    /// <summary>
    /// This deletes an existing puzzle.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
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

    /// <summary>
    /// This gets a list of all puzzles with matching theme.
    /// </summary>
    /// <param name="themes"></param>
    /// <returns></returns>
    [HttpGet("themes")]
    public async Task<ActionResult<List<PuzzleDTO>>> GetThemes([FromQuery] string themes)
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
    /// <summary>
    /// This retrieves a random puzzle with matching criteria and match. Should be able to accept any type.
    /// </summary>
    /// <param name="criteria"></param>
    /// <param name="match"></param>
    /// <returns></returns>
    [HttpGet("random")]
    public async Task<ActionResult<PuzzleDTO>> GetRandomByCriteria([FromQuery] string? criteria, [FromQuery] string? match)
    {
        if (criteria != null && match != null)
        {
            switch (criteria)
            {
                case "Themes":
                    PuzzleDTO p = await _puzzleService.GetAsyncRandomByCriteria(criteria, match);
                    _logger.LogInformation($"\n Returned puzzle:{p.ToString}");
                    return Ok(p);
                case "Rating":
                    try
                    {
                        Int32.TryParse(match, out int matchInt);
                        PuzzleDTO pint = await _puzzleService.GetAsyncRandomByCriteria(criteria, matchInt);
                        return Ok(pint);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical($"\n error: {ex}. Trouble parsin int. Make sure parsed value is int.\0");
                        return BadRequest();
                    }
                case "FEN":
                    match = Uri.UnescapeDataString(match);
                    _logger.LogInformation($"\n Decoded: {match}\0");
                    PuzzleDTO pfen = await _puzzleService.GetAsyncRandomByCriteria(criteria, match);
                    return Ok(pfen);
                default:
                    _logger.LogError("\n Escaped switch-case. Criteria possibly non-existant.\0");
                    return BadRequest();
            }
        }
        return Ok(await _puzzleService.GetAsyncRandom());
    }
    /// <summary>
    /// This retrieves a puzzle with matching rating.
    /// </summary>
    /// <param name="rating"></param>
    /// <returns></returns>
    [HttpGet("rating")]
    public async Task<ActionResult<List<PuzzleDTO>>> GetRating([FromQuery] Int32 rating)
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