using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MovieTime.Models
{
    public class Movie
    {
        
        public int MovieId { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public int Year { get; set; }
        public string? Description { get; set; }
        public int GenreId { get; set; }
        [Required]
        public virtual Genre? Genre { get; set; }

        public string? IMG { get; set; }
        public string? TrilerURL {  get; set; }
        public virtual DateTime Created_at { get; set; } = DateTime.Now;
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public List<MovieActor> MovieActor {  get; set; } = new List<MovieActor>();
        [Required]
        [NotMapped]
        public List<int> SelectedActorIds { get; set; }
        public virtual ICollection<Watchlist>? Watchlist { get; set; }
        public virtual ICollection<Rating>? Rating { get; set; }
        public virtual ICollection<Favorite>? Favorite { get; set; }
    }
}
