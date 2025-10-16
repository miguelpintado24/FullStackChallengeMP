using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TvShowTracker.Api.Data;
using TvShowTracker.Api.Services;
using TvShowTracker.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// SQLite database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "key12345!";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddScoped<AuthService>();
builder.Services.AddHostedService<RecommendationService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.TvShows.Any())
    {
        var actor1 = new Actor { Name = "Actor One" };
        var actor2 = new Actor { Name = "Actor Two" };

        var show1 = new TvShow
        {
            Title = "The Great Adventure",
            Genre = "Action",
            Type = "Series",
            Episodes = new List<Episode>
            {
                new Episode { Title = "Pilot", ReleaseDate = DateTime.Now.AddDays(-100) },
                new Episode { Title = "Episode 2", ReleaseDate = DateTime.Now.AddDays(-90) }
            },
            Actors = new List<Actor> { actor1, actor2 }
        };

        db.TvShows.Add(show1);
        db.SaveChanges();
    }
}

app.Run();
