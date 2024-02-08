namespace MovieTime.Models
{
    public class Genre
    {
        public int GenreId { get; set; }
        public string GenreName {  get; set; }
        public virtual ICollection<Movie>? Movie { get; set; }
    }
}
