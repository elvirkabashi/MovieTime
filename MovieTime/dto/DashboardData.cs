using MovieTime.Models;

namespace MovieTime.dto
{
    public class DashboardData
    {
        public List<ApplicationUser> Users { get; set; }
        public int UsersCount { get; set; }
        public int MoviesCount { get; set; }
        public int CountComents { get; set; }
        public double TotalRatingsSum { get; set; }
        public List<Movie> TopMovies { get; set; }
        public List<Movie> LatestMovies { get; set; }
        public List<ApplicationUser> LatestUsers { get; set; }
        public List<Rating> LastesReviews { get; set; }
    }
}
