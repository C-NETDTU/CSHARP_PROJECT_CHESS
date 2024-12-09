using src.data.model;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using src.services;
using Microsoft.OpenApi.Models;
using backend.src.repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.EntityFrameworkCore;

namespace MongoDBApi
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure PuzzleDatabaseSettings
                builder.Services.Configure<PuzzleDBSettings>(builder.Configuration.GetSection("PuzzleDatabase"));

                // Register MongoDB client and database
                builder.Services.AddSingleton<IMongoClient>(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
                    return new MongoClient(settings.ConnectionString);
                });
                builder.Services.AddSingleton(sp =>
                {
                    var client = sp.GetRequiredService<IMongoClient>();
                    var settings = sp.GetRequiredService<IOptions<PuzzleDBSettings>>().Value;
                    return client.GetDatabase(settings.DatabaseName);
                });

                // Register the repositories
                builder.Services.AddTransient(typeof(IMongoRepository<>), typeof(MongoRepository<>));
                builder.Services.AddTransient<IPuzzleRepository, PuzzleRepository>();

                // Register the services
                builder.Services.AddTransient<IPuzzleService, PuzzleService>();

                // Add services to the container.
                builder.Services.AddDbContext<PuzzleDbContext>(opt => opt.UseInMemoryDatabase("puzzleDB"));
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyChessApi", Version = "v1" });
                });
                builder.Services.AddControllers()
                    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(options =>
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MyChessApi v1");
                    });
                }

                app.UseHttpsRedirection();
                app.UseAuthorization();
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}