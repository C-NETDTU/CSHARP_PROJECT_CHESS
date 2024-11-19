using backend.src.repository;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using src.data.model;
using src.services;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application failed to start: {ex}");
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
     Host.CreateDefaultBuilder(args)
         .ConfigureWebHostDefaults(webBuilder =>
         {
             webBuilder.UseStartup<Startup>();
         })
         .ConfigureServices((context, services) =>
         {
             // Bind the "PuzzleDatabase" section in the configuration
             var puzzleDBName = services.Configure<PuzzleDBSettings>(context.Configuration.GetSection("PuzzleDatabase"));
             services.Configure<PuzzleDBSettings>(context.Configuration.GetSection("PuzzleDatabase"));

             // Register MongoDB Client
             services.AddSingleton<IMongoClient, MongoClient>(sp =>
             {
                 // Resolve PuzzleDBSettings from configuration
                 var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
                 if (settings == null || settings.ConnectionString == null)
                 {
                     throw new Exception("PuzzleDBSettings is null. Please check your configuration.");
                 }

                 return new MongoClient(settings.ConnectionString);
             });

             // Register MongoRepository with collection name
             services.AddScoped<IMongoRepository<Puzzle>>(sp =>
             {
                 var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
                 var database = sp.GetRequiredService<IMongoDatabase>();
                 return new MongoRepository<Puzzle>(database, settings.PuzzleCollectionName);
             });

             // Register other services
             services.AddScoped<IPuzzleService, PuzzleService>();
         });

}
