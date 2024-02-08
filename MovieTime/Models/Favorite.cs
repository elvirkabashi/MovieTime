namespace MovieTime.Models
{
    public class Favorite
    {
        public int FavoriteId { get; set; }
        public int MovieId { get; set; }
        public string? UserId { get; set; }
        public virtual Movie? Movie { get; set; }
    }
}
