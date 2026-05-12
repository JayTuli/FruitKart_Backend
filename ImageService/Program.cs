using ImageService.Data;
using ImageService.MappingProfile;
using ImageService.Repository;
using JWTAuthentication;
using Microsoft.EntityFrameworkCore;

namespace ImageService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Controllers ───────────────────────────────────────────────────
            builder.Services.AddControllers();

            // ── Database ──────────────────────────────────────────────────────
            builder.Services.AddDbContext<FruitImageDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("ImageServiceDb")));

            // ── Repository ────────────────────────────────────────────────────
            builder.Services.AddScoped<IImageRepository, ImageRepository>();

            // ── Azure Blob Service ────────────────────────────────────────────
            builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();

            // ── AutoMapper ────────────────────────────────────────────────────
            builder.Services.AddAutoMapper(typeof(FruitImageMap).Assembly);

            // ── Swagger ───────────────────────────────────────────────────────
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ── JWT Authentication ────────────────────────────────────────────
            builder.Services.AddJwtAuthentication();

            // ── CORS ──────────────────────────────────────────────────────────
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // ── Auto-migrate ──────────────────────────────────────────────────
            using (var scope = app.Services.CreateScope())
                scope.ServiceProvider.GetRequiredService<FruitImageDbContext>().Database.Migrate();

            // ── Pipeline ──────────────────────────────────────────────────────
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}