namespace MovieTime.dto
{
    public class MovieDto
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public int PublishedYear { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public string Img { get; set; }
        public string? TrilerURL { get; set; }
        public DateTime Created_at { get; set; }
        public List<ActorDto> Actors { get; set; }
    }
}
