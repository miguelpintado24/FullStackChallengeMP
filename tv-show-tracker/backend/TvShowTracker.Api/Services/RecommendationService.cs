using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TvShowTracker.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace TvShowTracker.Api.Services
{
    public class RecommendationService : BackgroundService
    {
        private readonly ILogger<RecommendationService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public RecommendationService(ILogger<RecommendationService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("📬 Recommendation background service started.");

            // Run every 24 hours (you can change to shorter interval for testing)
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendRecommendationsAsync();
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }

        private async Task SendRecommendationsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var users = await db.Users
                .Include(u => u.Favorites)
                .ThenInclude(f => f.TvShow)
                .ToListAsync();

            foreach (var user in users)
            {
                if (user.Favorites == null || !user.Favorites.Any()) continue;

                var favoriteGenres = user.Favorites.Select(f => f.TvShow!.Genre).Distinct().ToList();
                var recommendations = await db.TvShows
                    .Where(s => favoriteGenres.Contains(s.Genre) &&
                                !user.Favorites.Any(f => f.TvShowId == s.Id))
                    .Take(3)
                    .ToListAsync();

                if (recommendations.Any())
                {
                    // For now, just log it — later you can replace this with email sending logic.
                    _logger.LogInformation($"User '{user.Username}' recommended shows: {string.Join(", ", recommendations.Select(r => r.Title))}");
                }
            }
        }
    }
}
