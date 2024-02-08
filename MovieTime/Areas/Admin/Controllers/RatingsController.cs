using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieTime.Data;
using MovieTime.Models;

namespace MovieTime.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RatingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Ratings
        public async Task<IActionResult> Index(int page = 1, string sortOrder = "")
        {
            const int pageSize = 10;

            var totalItems = await _context.Ratings.CountAsync();

            IQueryable<Rating> ratingsQuery = _context.Ratings.Include(r => r.Movie);

            switch (sortOrder)
            {
                case "rate_desc":
                    ratingsQuery = ratingsQuery.OrderByDescending(r => r.Rate);
                    break;
                case "rate_asc":
                    ratingsQuery = ratingsQuery.OrderBy(r => r.Rate);
                    break;
                default:
                    // Default sorting, e.g., by date
                    ratingsQuery = ratingsQuery.OrderByDescending(r => r.Created);
                    break;
            }

            var ratings = await ratingsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagination = new Pagination(totalItems, page, pageSize, sortOrder);

            ViewBag.Pagination = pagination;
            ViewBag.CurrentSort = sortOrder;

            return View(ratings);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var rating = await _context.Ratings.FindAsync(id);
            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }


        private bool RatingExists(int id)
        {
          return (_context.Ratings?.Any(e => e.RatingId == id)).GetValueOrDefault();
        }
    }
}
