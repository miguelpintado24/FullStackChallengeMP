using System.ComponentModel.DataAnnotations;

namespace TvShowTracker.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] PasswordSalt { get; set; }

        public ICollection<Favorite>? Favorites { get; set; }
    }
}
