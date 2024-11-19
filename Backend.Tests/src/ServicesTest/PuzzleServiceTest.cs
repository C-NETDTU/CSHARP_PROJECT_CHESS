using Xunit;
using Moq;
using src.data.model;
using src.services;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace Backend.Services.Tests
{

    public class PuzzleConsumer
    {
        private readonly IPuzzleService _puzzleService;

        public PuzzleConsumer(IPuzzleService puzzleService)
        {
            _puzzleService = puzzleService;
        }

        public Task<List<Puzzle>> GetPuzzles(int pageNumber, int pageSize)
        {
            return _puzzleService.GetAsync(pageNumber, pageSize);
        }
        public Task<List<Puzzle>> GetRating(int rating)
        {
            return _puzzleService.GetAsyncRating(rating);
        }
        public Task<Puzzle> GetMatchingCriteria(string Criteria, string Match)
        {
            return _puzzleService.GetAsyncRandomByCriteria(Criteria, Match); 
        }
    }
    public class PuzzleConsumerTests
    {
        [Fact]
        public async Task GetPuzzles_ReturnsExpectedPuzzles()
        {
            var mockPuzzleService = new Mock<IPuzzleService>();
            var expectedPuzzles = new List<Puzzle> { new Puzzle { Id = "1" }, new Puzzle { Id = "2" } };

            mockPuzzleService.Setup(service => service.GetAsync(1, 2)).ReturnsAsync(expectedPuzzles);

            var consumer = new PuzzleConsumer(mockPuzzleService.Object);

            var result = await consumer.GetPuzzles(1, 2);

            Assert.Equal(expectedPuzzles, result);
        }
        [Fact]
        public async Task GetPuzzleByRating_ReturnsExpectedPuzzle()
        {
            var mockPuzzleService = new Mock<IPuzzleService>();
            var possiblePuzzles = new List<Puzzle> { new Puzzle { Id = "1", Rating = 1200 }, new Puzzle { Id = "2", Rating = 1100 }, new Puzzle { Id = "3", Rating = 2000 } };

            mockPuzzleService.Setup(service => service.GetAsyncRating(1100)).ReturnsAsync(possiblePuzzles);

            var consumer = new PuzzleConsumer(mockPuzzleService.Object);

            var result = await consumer.GetRating(1100);
            Assert.Equal(possiblePuzzles[0], result[0]);
        }

        [Fact]
        public async Task GetRandomPuzzleByCriteria_ReturnsExpectedPuzzle()
        {
            var mockPuzzleService = new Mock<IPuzzleService>(); ;
            var possiblePuzzles = new List<Puzzle> { new Puzzle { Id = "1", Themes = "discoveredAttack, endGame" }, new Puzzle { Id = "2", Themes = "advantage, endGame" }, new Puzzle { Id = "3", Themes = "advancedPawn, skewer" } };

            mockPuzzleService.Setup(service => service.GetAsyncRandomByCriteria("Themes", "discoveredAttack")).ReturnsAsync((string criteria, string match) => possiblePuzzles.Find(p => p.Themes.Contains(match)));

            var consumer = new PuzzleConsumer(mockPuzzleService.Object);
            
            var result = await consumer.GetMatchingCriteria("Themes", "discoveredAttack");
            Assert.Contains(result,possiblePuzzles);
            Assert.Equal(possiblePuzzles[0], result);
            mockPuzzleService.Setup(service => service.GetAsyncRandomByCriteria("Themes", "advantage")).ReturnsAsync((string criteria, string match) => possiblePuzzles.Find(p => p.Themes.Contains(match)));
            var result2 = await consumer.GetMatchingCriteria("Themes", "advantage");
            Assert.Contains(result2, possiblePuzzles);
            Assert.Equal(possiblePuzzles[1], result2);
        }
    }
}

