using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Api.Models
{
    public class Actor
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        public ICollection<TvShow>? TvShows { get; set; }
    }
}
