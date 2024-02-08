namespace MovieTime.dto
{
    public class FavoritesDto
    {
        public int FavoriteId { get; set; }
        // Movie details
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string IMG { get; set; }
    }
}
