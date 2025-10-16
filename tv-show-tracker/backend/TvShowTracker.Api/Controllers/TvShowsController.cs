using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TvShowTracker.Api.Data;
using TvShowTracker.Api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace TvShowTracker.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TvShowsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public TvShowsController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        // GET: api/tvshows
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TvShow>>> GetTvShows(
            [FromQuery] string? genre,
            [FromQuery] string? type,
            [FromQuery] string? sortBy = "title",
            [FromQuery] string? order = "asc",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = _context.TvShows
                .Include(s => s.Episodes)
                .Include(s => s.Actors)
                .AsQueryable();

            if (!string.IsNullOrEmpty(genre))
                query = query.Where(s => s.Genre!.ToLower() == genre.ToLower());

            if (!string.IsNullOrEmpty(type))
                query = query.Where(s => s.Type!.ToLower() == type.ToLower());

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "genre" => order == "desc" ? query.OrderByDescending(s => s.Genre) : query.OrderBy(s => s.Genre),
                "type" => order == "desc" ? query.OrderByDescending(s => s.Type) : query.OrderBy(s => s.Type),
                _ => order == "desc" ? query.OrderByDescending(s => s.Title) : query.OrderBy(s => s.Title)
            };

            // Pagination
            var totalCount = await query.CountAsync();
            var shows = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                shows
            });
        }

        // GET: api/tvshows/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TvShow>> GetTvShow(int id)
        {
            var show = await _context.TvShows
                .Include(s => s.Episodes)
                .Include(s => s.Actors)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return NotFound();
            return Ok(show);
        }

        // POST: api/tvshows (Admin/Testing)
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<TvShow>> CreateTvShow(TvShow show)
        {
            _context.TvShows.Add(show);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTvShow), new { id = show.Id }, show);
        }

        // DELETE: api/tvshows/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTvShow(int id)
        {
            var show = await _context.TvShows.FindAsync(id);
            if (show == null) return NotFound();

            _context.TvShows.Remove(show);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/tvshows/{id}/actors
        [HttpGet("{id}/actors")]
        public async Task<ActionResult<IEnumerable<Actor>>> GetActorsForShow(int id)
        {
            var show = await _context.TvShows
                .Include(s => s.Actors)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (show == null) return NotFound();
            return Ok(show.Actors);
        }

        // GET: api/tvshows/actor/{actorId}
        [HttpGet("actor/{actorId}")]
        public async Task<ActionResult<IEnumerable<TvShow>>> GetShowsForActor(int actorId)
        {
            var actor = await _context.Actors
                .Include(a => a.TvShows)
                .FirstOrDefaultAsync(a => a.Id == actorId);

            if (actor == null) return NotFound();
            return Ok(actor.TvShows);
        }

        // POST: api/tvshows/{id}/favorite
        [Authorize]
        [HttpPost("{id}/favorite")]
        public async Task<IActionResult> AddFavorite(int id)
        {
            var username = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var exists = await _context.Favorites.AnyAsync(f => f.UserId == user.Id && f.TvShowId == id);
            if (exists) return BadRequest("Already favorited.");

            _context.Favorites.Add(new Favorite { UserId = user.Id, TvShowId = id });
            await _context.SaveChangesAsync();
            return Ok("Added to favorites.");
        }

        // DELETE: api/tvshows/{id}/favorite
        [Authorize]
        [HttpDelete("{id}/favorite")]
        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var username = User.Identity!.Name;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null) return Unauthorized();

            var favorite = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id && f.TvShowId == id);
            if (favorite == null) return NotFound();

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();
            return Ok("Removed from favorites.");
        }

        // GET: api/tvshows/recommendations
        [Authorize]
        [HttpGet("recommendations")]
        public async Task<IActionResult> GetRecommendations()
        {
            var username = User.Identity!.Name;
            var user = await _context.Users
                .Include(u => u.Favorites)
                .ThenInclude(f => f.TvShow)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null) return Unauthorized();

            var favoriteGenres = user.Favorites?.Select(f => f.TvShow!.Genre).Distinct().ToList();
            if (favoriteGenres == null || !favoriteGenres.Any()) return Ok(new List<TvShow>());

            var recommendations = await _context.TvShows
                .Where(s => favoriteGenres.Contains(s.Genre) && !user.Favorites!.Any(f => f.TvShowId == s.Id))
                .Take(5)
                .ToListAsync();

            return Ok(recommendations);
        }
    }
}
