using src.data.model;
using src.services;
using Microsoft.AspNetCore.Mvc;

namespace src.controller;

[ApiController]
[Route("api/[controller]")]
public class PuzzleController : ControllerBase
{
    private readonly PuzzleService _puzzleService;

    public PuzzleController(PuzzleService puzzleService) =>
        _puzzleService = puzzleService;

    [HttpGet]
    public async Task<List<Puzzle>> Get() =>
        await _puzzleService.GetAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<Puzzle>> Get(string id)
    {
        var Puzzle = await _puzzleService.GetAsync(id);

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
        var puzzle = await _puzzleService.GetAsync(id);

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
        var Puzzle = await _puzzleService.GetAsync(id);

        if (Puzzle is null)
        {
            return NotFound();
        }

        await _puzzleService.RemoveAsync(id);

        return NoContent();
    }
}