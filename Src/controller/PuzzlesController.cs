using src.data.model;
using src.services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;

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
            return NotFound();
        }
        return Ok(puzzles);
    }

}