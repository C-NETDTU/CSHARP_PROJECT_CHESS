using MongoDB.Driver;
using src.data.model;
using System;

namespace src.data.repositories.MongoDB
{
    public class PuzzleRepository : IPuzzleService
    {
        public Task CreatePuzzle(Puzzle puzzle)
        {
            throw new NotImplementedException();
        }

        public Task DeletePuzzle(Puzzle puzzle)
        {
            throw new NotImplementedException();
        }

        public Task ReadPuzzle(string id)
        {
            throw new NotImplementedException();
        }
    }
}