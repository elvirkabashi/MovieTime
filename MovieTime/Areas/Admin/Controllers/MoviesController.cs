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
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MoviesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Movies
        public async Task<IActionResult> Index(string genre, string searchKeyWord, string sortOrder, int page = 1)
        {
            const int pageSize = 10;

            IQueryable<Movie> moviesQuery = _context.Movies
                .Include(m => m.MovieActor)
                .ThenInclude(ma => ma.Actor)
                .Include(m => m.Genre);

            // Search filter
            if (!string.IsNullOrEmpty(searchKeyWord))
            {
                moviesQuery = moviesQuery.Where(m =>
                    m.Title.Contains(searchKeyWord) ||
                    m.Description.Contains(searchKeyWord) ||
                    m.Genre.GenreName.Contains(searchKeyWord) ||
                    m.MovieActor.Any(ma => ma.Actor.Name.Contains(searchKeyWord))
                );
            }
            ViewBag.SearchKeyword = searchKeyWord;

            // Genre filter
            if (!string.IsNullOrEmpty(genre))
            {
                moviesQuery = moviesQuery.Where(m => m.Genre.GenreName == genre);
            }

            // Sorting
            switch (sortOrder)
            {
                case "title_desc":
                    moviesQuery = moviesQuery.OrderByDescending(m => m.Title);
                    break;
                case "Year":
                    moviesQuery = moviesQuery.OrderBy(m => m.Year);
                    break;
                case "year_desc":
                    moviesQuery = moviesQuery.OrderByDescending(m => m.Year);
                    break;
                default:
                    moviesQuery = moviesQuery.OrderBy(m => m.Title);
                    break;
            }

            var totalItems = await moviesQuery.CountAsync();
            var pagination = new Pagination(totalItems, page, pageSize);

            var movies = await moviesQuery
                .Skip((pagination.CurrentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Setting up ViewData for sorting links
            ViewData["TitleSortParm"] = sortOrder == "title_desc" ? "title_asc" : "title_desc";
            ViewData["YearSortParm"] = sortOrder == "Year" ? "year_desc" : "Year";

            ViewBag.GenreList = new SelectList(_context.Genres, "GenreId", "GenreName");
            ViewBag.Pagination = pagination;
            ViewBag.GenreFilter = genre;
            ViewBag.SortOrder = sortOrder;

            return View(movies);
        }


        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActor)
                .ThenInclude(ma => ma.Actor)
                .Include(m => m.Genre)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewBag.ActorList = new SelectList(_context.Actors, "ActorId", "Name");
            ViewBag.GenreList = new SelectList(_context.Genres, "GenreId", "GenreName");
            return View();
        }

        // POST: Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Year,Description,GenreId,IMG,SelectedActorIds,TrilerURL,Created_at,ImageFile")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                if (movie.ImageFile != null && movie.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    movie.IMG = uniqueFileName;

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.ImageFile.CopyToAsync(fileStream);
                    }
                }

                if (movie.SelectedActorIds != null)
                {
                    foreach (var actorId in movie.SelectedActorIds)
                    {
                        var movieActor = new MovieActor
                        {
                            MovieId = movie.MovieId,
                            ActorId = actorId,
                            Movie = movie,
                            Actor = _context.Actors.Find(actorId)
                        };

                        _context.MovieActors.Add(movieActor);
                    }
                }

                _context.Add(movie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.GenreList = new SelectList(_context.Genres, "GenreId", "GenreName");
            ViewBag.ActorList = new SelectList(_context.Actors, "ActorId", "Name");

            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.MovieActor)
                .FirstOrDefaultAsync(m => m.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            ViewBag.GenreList = new SelectList(_context.Genres, "GenreId", "GenreName", movie.GenreId);
            ViewBag.ActorList = new SelectList(_context.Actors, "ActorId", "Name", movie.MovieActor.Select(ma => ma.ActorId));
            movie.SelectedActorIds = movie.MovieActor.Select(ma => ma.ActorId).ToList();

            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Year,Description,GenreId,IMG,SelectedActorIds,TrilerURL,Created_at,ImageFile")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                // Repopulate ViewBag with SelectList for genres and actors
                ViewBag.GenreList = new SelectList(_context.Genres, "GenreId", "GenreName", movie.GenreId);
                ViewBag.ActorList = new SelectList(_context.Actors, "ActorId", "Name", movie.SelectedActorIds);

                return View(movie);
            }

            try
            {
                // Handle image file
                if (movie.ImageFile != null && movie.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + movie.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    movie.IMG = uniqueFileName;

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await movie.ImageFile.CopyToAsync(fileStream);
                    }
                }
                else
                {
                    // If ImageFile is null, retain the previous value of IMG
                    var existingMovieEntity = await _context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.MovieId == movie.MovieId);
                    if (existingMovieEntity != null)
                    {
                        movie.IMG = existingMovieEntity.IMG;
                    }
                }

                // Update movie
                _context.Update(movie);

                // Update associated actors
                var existingMovieEntityForActors = await _context.Movies.Include(m => m.MovieActor).FirstOrDefaultAsync(m => m.MovieId == movie.MovieId);
                if (existingMovieEntityForActors != null)
                {
                    existingMovieEntityForActors.MovieActor.Clear();

                    if (movie.SelectedActorIds != null)
                    {
                        foreach (var actorId in movie.SelectedActorIds)
                        {
                            existingMovieEntityForActors.MovieActor.Add(new MovieActor { ActorId = actorId });
                        }
                    }

                    _context.Update(existingMovieEntityForActors);
                }

                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(movie.MovieId))
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



        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return (_context.Movies?.Any(e => e.MovieId == id)).GetValueOrDefault();
        }
    }
}
