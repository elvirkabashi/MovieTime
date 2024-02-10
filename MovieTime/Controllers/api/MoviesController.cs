using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTime.Data;
using MovieTime.dto;
using MovieTime.Models;

namespace MovieTime.Controllers.apii
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Movies for search api/Movies/?query=
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies([FromQuery] string? query)
        {
            var moviesQuery = _context.Movies
                .Include(x => x.MovieActor)
                .ThenInclude(x => x.Actor)
                .Include(x => x.Genre)
                .OrderByDescending(x => x.Created_at)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                
                int year;
                bool isNumeric = int.TryParse(query, out year);

                moviesQuery = moviesQuery.Where(x =>
                    x.Title.Contains(query) ||
                    x.Description.Contains(query) ||
                    x.Genre.GenreName.Equals(query) ||
                    (isNumeric && x.Year == year)
                );
            }

            var movies = await moviesQuery.ToListAsync();

            if (movies == null || movies.Count == 0)
            {
                return NotFound();
            }

            var movieDtos = movies.Select(x => new MovieDto
            {
                MovieId = x.MovieId,
                Title = x.Title,
                PublishedYear = x.Year,
                Description = x.Description,
                Img = x.IMG,
                Genre = x.Genre.GenreName,
                TrilerURL = x.TrilerURL,
                Created_at = x.Created_at,
                Actors = x.MovieActor.Select(a => new ActorDto { ActorId = a.ActorId, 
                    Name = a.Actor.Name,
                    Img = a.Actor.Img
                }).ToList()
            });

            return Ok(movieDtos);
        }



        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieDto>> GetMovie(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MovieActor)
                .ThenInclude(x => x.Actor)
                .Include(x => x.Genre)
                .FirstOrDefaultAsync(x => x.MovieId == id);

            if (movie == null)
            {
                return NotFound();
            }

            var movieDto = new MovieDto
            {
                MovieId = movie.MovieId,
                Title = movie.Title,
                PublishedYear = movie.Year,
                Description = movie.Description,
                Img = movie.IMG,
                Genre = movie.Genre.GenreName,
                TrilerURL = movie.TrilerURL,

                Actors = movie.MovieActor.Select(a => new ActorDto { ActorId = a.ActorId, 
                    Name = a.Actor.Name,
                    Birthdate = a.Actor.Birthdate, 
                    Img = a.Actor.Img,
                    Description = a.Actor.Description
                }).ToList()
            };

            return Ok(movieDto);
        }


    }
}
