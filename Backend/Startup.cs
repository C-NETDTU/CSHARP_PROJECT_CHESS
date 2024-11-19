using backend.src.repository;
using MongoDB.Driver;
using src.data.model;
using src.services;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddLogging();
        services.AddTransient<IMongoRepository<Puzzle>, MongoRepository<Puzzle>>();
        services.AddTransient<IPuzzleRepository, PuzzleRepository>();
        services.AddTransient<IPuzzleService, PuzzleService>();

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        logger.LogInformation("Application started.");
    }
}