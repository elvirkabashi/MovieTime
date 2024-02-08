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
    public class WatchlistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WatchlistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Watchlists
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 10;


            var totalItems = await _context.Watchlists.CountAsync();


            var pagination = new Pagination(totalItems, page, pageSize);

            // Get watchlists for the current page
            var watchlists = await _context.Watchlists
                .Skip((pagination.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .Include(w => w.Movie)
                .ToListAsync();

            ViewBag.Pagination = pagination;

            var applicationDbContext = _context.Watchlists.Include(w => w.Movie);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Watchlists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Watchlists == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists
                .Include(w => w.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (watchlist == null)
            {
                return NotFound();
            }

            return View(watchlist);
        }

        // GET: Watchlists/Create
        public IActionResult Create()
        {
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId");
            return View();
        }

        // POST: Watchlists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MovieId")] Watchlist watchlist)
        {
            if (ModelState.IsValid)
            {
                _context.Add(watchlist);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId", watchlist.MovieId);
            return View(watchlist);
        }

        // GET: Watchlists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Watchlists == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists.FindAsync(id);
            if (watchlist == null)
            {
                return NotFound();
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId", watchlist.MovieId);
            return View(watchlist);
        }

        // POST: Watchlists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MovieId")] Watchlist watchlist)
        {
            if (id != watchlist.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(watchlist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WatchlistExists(watchlist.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MovieId"] = new SelectList(_context.Movies, "MovieId", "MovieId", watchlist.MovieId);
            return View(watchlist);
        }

        // GET: Watchlists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Watchlists == null)
            {
                return NotFound();
            }

            var watchlist = await _context.Watchlists
                .Include(w => w.Movie)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (watchlist == null)
            {
                return NotFound();
            }

            return View(watchlist);
        }

        // POST: Watchlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Watchlists == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Watchlists'  is null.");
            }
            var watchlist = await _context.Watchlists.FindAsync(id);
            if (watchlist != null)
            {
                _context.Watchlists.Remove(watchlist);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WatchlistExists(int id)
        {
            return (_context.Watchlists?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
