namespace MovieTime.Models
{
    public class Watchlist
        
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public virtual Movie? Movie { get; set; }
        public string? UserId { get; set; }
        public DateTime DateTime { get; set; } = DateTime.Now;

    }
}
