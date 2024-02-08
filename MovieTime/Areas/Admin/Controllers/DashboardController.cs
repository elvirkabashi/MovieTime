using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTime.Data;
using MovieTime.dto;

namespace MovieTime.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            var users = _context.Users.ToList();
            var moviesCount = _context.Movies.Count();
            var countComents = _context.Ratings.Count();
            var totalRatingsSum = _context.Ratings.Sum(r => r.Rate);

            // Get top 5 movies by rating
            var topMovies = _context.Movies
                .Include(x => x.Genre)
                .Include(x => x.Rating)
                .OrderByDescending(m => m.Rating.Average(r => r.Rate))
                .Take(5)
                .ToList();

            // Get top 5 lates movies
            var latestMovies = _context.Movies
                .Include(x => x.Genre)
                .OrderByDescending(m => m.Created_at)
                .Take(5)
                .ToList();

            var latestUsers = _context.Users
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .ToList();

            var lastesReviews = _context.Ratings
                .Include(m => m.Movie)
                .OrderByDescending(m => m.Created)
                .Take(5)
                .ToList();

            var dashboardData = new DashboardData
            {
                Users = users,
                UsersCount = users.Count,
                MoviesCount = moviesCount,
                CountComents = countComents,
                TotalRatingsSum = totalRatingsSum,
                TopMovies = topMovies,
                LatestMovies = latestMovies,
                LatestUsers = latestUsers,
                LastesReviews = lastesReviews
            };

            return View(dashboardData);
        }
    }
}
