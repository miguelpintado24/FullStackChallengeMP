using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Api.Models
{
    public class TvShow
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public string? Genre { get; set; }
        public string? Type { get; set; }
        public ICollection<Episode>? Episodes { get; set; }
        public ICollection<Actor>? Actors { get; set; }
        public ICollection<Favorite>? Favorites { get; set; }
    }
}
