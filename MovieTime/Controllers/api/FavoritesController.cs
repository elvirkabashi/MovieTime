using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTime.dto;
using MovieTime.Data;
using MovieTime.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MovieTime.Controllers.api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Favorites
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FavoritesDto>>> GetFavorites()
        {
            if (_context.Favorites == null)
            {
                return NotFound();
            }

            string userId = GetUserIdFromToken();

            var favoriteslists = await _context.Favorites
                .Where(f => f.UserId == userId)
                .Include(f => f.Movie)
                .Select(f => new FavoritesDto
                {
                    FavoriteId = f.FavoriteId,
                    MovieId = f.MovieId,
                    Title = f.Movie.Title,
                    Year = f.Movie.Year,
                    IMG = f.Movie.IMG
                })
                .ToListAsync();

            return favoriteslists;
        }

        // POST: api/Favorites
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Favorite>> PostFavorite(Favorite favorite)
        {
            if (_context.Favorites == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Favorites' is null.");
            }

            string userId = GetUserIdFromToken();
            favorite.UserId = userId;

            _context.Favorites.Add(favorite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFavorite", new { id = favorite.FavoriteId }, favorite);
        }

        // DELETE: api/Favorites/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFavorite(int id)
        {
            if (_context.Favorites == null)
            {
                return NotFound();
            }

            string userId = GetUserIdFromToken();

            var favorite = await _context.Favorites
                .FirstOrDefaultAsync(f => f.FavoriteId == id && f.UserId == userId);

            if (favorite == null)
            {
                return NotFound();
            }

            _context.Favorites.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private string GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim?.Value;
        }
    }

}
