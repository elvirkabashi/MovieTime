using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTime.Data;
using MovieTime.dto;
using MovieTime.Models;

namespace MovieTime.Controllers.api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WatchlistsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WatchlistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Watchlists
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WatchlistsDto>>> GetWatchlists()
        {
            if (_context.Watchlists == null)
            {
                return NotFound();
            }

            string userId = GetUserIdFromToken();

            var watchlists = await _context.Watchlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Movie)
                .Select(w => new WatchlistsDto
                {
                    Id = w.Id,
                    DateTime = w.DateTime,
                    MovieId = w.MovieId,
                    Title = w.Movie.Title,
                    Year = w.Movie.Year,
                    Description = w.Movie.Description,
                    IMG = w.Movie.IMG,
                })
                .ToListAsync();

            return watchlists;
        }


        // POST: api/Watchlists
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Watchlist>> PostWatchlist(Watchlist watchlist)
        {
            if (_context.Watchlists == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Watchlists' is null.");
            }

            string userId = GetUserIdFromToken();
            watchlist.UserId = userId;

            _context.Watchlists.Add(watchlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWatchlist", new { id = watchlist.Id }, watchlist);
        }

        // DELETE: api/Watchlists/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWatchlist(int id)
        {
            if (_context.Watchlists == null)
            {
                return NotFound();
            }

            string userId = GetUserIdFromToken();

            var watchlist = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);

            if (watchlist == null)
            {
                return NotFound();
            }

            _context.Watchlists.Remove(watchlist);
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
