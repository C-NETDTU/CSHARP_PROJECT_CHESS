using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Numerics;
using src.data.model;
using MongoDB.EntityFrameworkCore.Extensions;

public class PuzzleDbContext : DbContext
{
    public DbSet<Puzzle> Puzzles { get; init; }

    public static PuzzleDbContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<PuzzleDbContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    public PuzzleDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Puzzle>().ToCollection("puzzles");
    }
}