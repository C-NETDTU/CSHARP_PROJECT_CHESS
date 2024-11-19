using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using src.data.model;
using backend.src.repository;
using src.services;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // ConfigureServices is required to set up DI services
    public void ConfigureServices(IServiceCollection services)
    {
        // Bind MongoDB configuration
        services.Configure<PuzzleDBSettings>(_configuration.GetSection("PuzzleDatabase"));

        // Register MongoDB Client and Database
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });

        // Register Repositories and Services
        services.AddScoped<IMongoRepository<Puzzle>, MongoRepository<Puzzle>>();
        services.AddScoped<IPuzzleRepository, PuzzleRepository>();
        services.AddScoped<IPuzzleService, PuzzleService>();

        // Add controllers and configure JSON options
        services.AddControllers()
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

        // Swagger setup for API documentation
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Puzzle API",
                Version = "v1"
            });
        });
    }

    // Configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        if (env.IsDevelopment())
        {
            // Use Developer Exception Page in Development mode
            app.UseDeveloperExceptionPage();

            // Enable Swagger UI for API documentation
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Puzzle API v1");
            });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();

        // Map controller endpoints
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        logger.LogInformation("Application started successfully.");
    }
}
