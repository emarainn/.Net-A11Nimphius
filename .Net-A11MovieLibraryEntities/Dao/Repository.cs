using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public class Repository : IRepository, IDisposable
    {
        private readonly IDbContextFactory<MovieContext> _contextFactory;
        private readonly MovieContext _context;

        public Repository(IDbContextFactory<MovieContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IEnumerable<Movie> GetAll()
        {

            int numItem = 6;
            Console.Write("Enter Number of movies to view: ");
            string sikeNum = Console.ReadLine();

            Console.WriteLine("\n** SIKE THIS BAD BOY HARD CODED **" +
                "\nalso it might take a sec to load...");


            // USER INPUT AND VALIDATION
            /*
            int num;
            Console.Write("Enter amount of movies you want to view: ");

            while (!int.TryParse(Console.ReadLine(), out numItem) || numItem <= 0)
            {
                Console.WriteLine("**Must Enter Number Greater Than 0**");
                Console.WriteLine("Enter amount of movies you want to view: ");
            }
            */

            return _context.Movies.ToList().Take(numItem);

        }

        public IEnumerable<Movie> Search(string searchString)
        {
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            var temp = listOfMovies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            return temp;
        }

        public Movie AddMovie()
        {
            var allMovie = _context.Movies;
            var listOfMovies = allMovie.ToList();
            var maxId = listOfMovies.Count();
            var newMax = maxId + 1;

            Console.Write($"New Movie ID: {newMax}" +
                $"\nEnter New Movie Title: ");
            var movieT = Console.ReadLine();

            var movie = new Movie();
            movie.Title = movieT;

            using (var db = new MovieContext())
            {
                db.Add(movie);
                db.SaveChanges();

            }

            return movie;
        }
        public int maxId()
        {
            var allMovie = _context.Movies;
            var listOfMovies = allMovie.ToList();
            var maxId = listOfMovies.Count();

            return maxId + 1;
        }
    }
}
