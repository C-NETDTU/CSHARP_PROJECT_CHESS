using src.data.model;
using src.data.network;
using src.data.repositories.MongoDB;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore;
using src.services;

namespace MongoDBApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {

                var builder = WebApplication.CreateBuilder(args);
                builder.Services.Configure<PuzzleDBSettings>(builder.Configuration.GetSection("PuzzleDatabase"));
                builder.Services.AddSingleton<PuzzleService>();
                // Add services to the container.
                builder.Services.AddControllers();
                builder.Services.AddDbContext<PuzzleDbContext>(opt => opt.UseInMemoryDatabase("puzzleDB"));
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();

                app.UseAuthorization();
                app.MapControllers();

                app.Run();
                /*
                var connectionString = "mongodb://localhost:27017/";
                    //Environment.GetEnvironmentVariable("MONGODB_URI");
                try
                {
                    MongoClient dbClient = new MongoClient(connectionString);
                    Console.WriteLine($"Connected to MongoDB: {dbClient}");

                    var db = dbClient.GetDatabase("puzzleDB");
                    Console.WriteLine($"Database accessed: {db.DatabaseNamespace}");

                    var puzzles = db.GetCollection<Puzzle>("puzzles");
                    Console.WriteLine($"Collection accessed: {puzzles.CollectionNamespace}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
                */
            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            }
        }
    }
}