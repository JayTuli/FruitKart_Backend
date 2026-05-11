using MenuServiceAPI.Data;
using MenuServiceAPI.MappingProfile;
using MenuServiceAPI.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MenuDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("MenuServiceDb")));

builder.Services.AddAutoMapper(typeof(MenuItemProfile).Assembly);

builder.Services.AddScoped<IMenuRepository, MenuRepository>();

builder.Services.AddHttpClient("ImageService", client =>
{
    var url = builder.Configuration["ServiceUrls:ImageService"]
        ?? throw new InvalidOperationException("ServiceUrls:ImageService not configured.");
    client.BaseAddress = new Uri(url);
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<MenuDbContext>().Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.MapControllers();
app.Run();