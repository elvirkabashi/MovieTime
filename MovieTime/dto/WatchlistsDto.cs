namespace MovieTime.dto
{
    public class WatchlistsDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }

        // Movie details
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string IMG { get; set; }
    }
}
