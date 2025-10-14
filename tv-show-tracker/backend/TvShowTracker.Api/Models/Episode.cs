using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Api.Models
{
    public class Episode
    {
        public int Id { get; set; }
        [Required] public string Title { get; set; } = string.Empty;
        public DateTime ReleaseDate { get; set; }
        public int TvShowId { get; set; }
        public TvShow? TvShow { get; set; }
    }
}
