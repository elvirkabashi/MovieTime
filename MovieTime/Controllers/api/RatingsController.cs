using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTime.Data;
using MovieTime.Models;

namespace MovieTime.Controllers.api
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context; 
        }

        // GET: api/Ratings/ByMovieId/5
        [HttpGet("ByMovieId/{movieId}")]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatingsByMovieId(int movieId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.MovieId == movieId)
                .ToListAsync();

            //if (ratings == null || !ratings.Any())
            //{
            //    return NotFound(); // No ratings found for the specified movie
            //}

            return ratings;
        }

        // GET: api/Ratings/AverageRating/5
        [HttpGet("AverageRating/{movieId}")]
        public async Task<ActionResult<double?>> GetAverageRating(int movieId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.MovieId == movieId)
                .ToListAsync();

            if (!ratings.Any())
            {
                return null; // No ratings found for the specified movie
            }

            var averageRating = ratings.Average(r => r.Rate);

            return averageRating;
        }

        [HttpGet("TopRatedMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetTopRatedMovies()
        {
            if (_context.Movies == null || _context.Ratings == null)
            {
                return NotFound();
            }

            var topRatedMovies = await _context.Movies
                .Join(
                    _context.Ratings,
                    movie => movie.MovieId,
                    rating => rating.MovieId,
                    (movie, rating) => new { Movie = movie, Rating = rating }
                )
                .GroupBy(joined => new { joined.Movie.MovieId, joined.Movie.Title })
                .Select(grouped => new
                {
                    MovieId = grouped.Key.MovieId,
                    Title = grouped.Key.Title,
                    RatingCount = grouped.Count()
                })
                .OrderByDescending(result => result.RatingCount)
                .Take(10) 
                .ToListAsync();

            if (!topRatedMovies.Any())
            {
                return NotFound();
            }

            return topRatedMovies.Select(result => new Movie
            {
                MovieId = result.MovieId,
                Title = result.Title,
            }).ToList();
        }

        // GET: api/Ratings/CountByMovieId/5
        [HttpGet("CountByMovieId/{movieId}")]
        public async Task<ActionResult<int>> GetRatingCountByMovieId(int movieId)
        {
            if (_context.Ratings == null)
            {
                return NotFound();
            }

            var ratingCount = await _context.Ratings
                .Where(r => r.MovieId == movieId)
                .CountAsync();

            return ratingCount;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
          if (_context.Ratings == null)
          {
              return NotFound();
          }
            return await _context.Ratings.ToListAsync();
        }

        // GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rating>> GetRating(int id)
        {
          if (_context.Ratings == null)
          {
              return NotFound();
          }
            var rating = await _context.Ratings.FindAsync(id);

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

        // PUT: api/Ratings/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, Rating rating)
        {
            if (id != rating.RatingId)
            {
                return BadRequest();
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Ratings
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
          if (_context.Ratings == null)
          {
              return Problem("Entity set 'ApplicationDbContext.Ratings'  is null.");
          }
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRating", new { id = rating.RatingId }, rating);
        }

        // DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            if (_context.Ratings == null)
            {
                return NotFound();
            }
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RatingExists(int id)
        {
            return (_context.Ratings?.Any(e => e.RatingId == id)).GetValueOrDefault();
        }
    }
}
