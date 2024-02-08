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
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ActorsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Actors
        public async Task<IActionResult> Index()
        {
            return _context.Actors != null ?
                        View(await _context.Actors.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Actors'  is null.");
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.ActorId == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActorId,Name,Birthdate,Img,Description,ImageFile")] Actor actor)
        {
            if (ModelState.IsValid)
            {

                if (actor.ImageFile != null && actor.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "actors");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + actor.ImageFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    actor.Img = uniqueFileName;

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await actor.ImageFile.CopyToAsync(fileStream);
                    }
                }

                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActorId,Name,Birthdate,Img,Description,ImageFile")] Actor actor)
        {
            if (id != actor.ActorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (actor.ImageFile != null && actor.ImageFile.Length > 0)
                    {
                        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "actors");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + actor.ImageFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        actor.Img = uniqueFileName;

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await actor.ImageFile.CopyToAsync(fileStream);
                        }
                    }
                    else
                    {
                        // If ImageFile is null, retain the previous value of IMG
                        var existingEntity = await _context.Actors.AsNoTracking().FirstOrDefaultAsync(m => m.ActorId == actor.ActorId);
                        if (existingEntity != null)
                        {
                            actor.Img = existingEntity.Img;
                        }
                    }

                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.ActorId))
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
            return View(actor);
        }


        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Actors == null)
            {
                return NotFound();
            }

            var actor = await _context.Actors
                .FirstOrDefaultAsync(m => m.ActorId == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Actors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Actors'  is null.");
            }
            var actor = await _context.Actors.FindAsync(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
            return (_context.Actors?.Any(e => e.ActorId == id)).GetValueOrDefault();
        }
    }
}
