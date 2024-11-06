using src.data.model;
using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore;
using src.services;
using Microsoft.OpenApi.Models;

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
                builder.Services.AddDbContext<PuzzleDbContext>(opt => opt.UseInMemoryDatabase("puzzleDB"));
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MyChessApi", Version = "v1" });
                });
                builder.Services.AddControllers();

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
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            }
        }
    }
}