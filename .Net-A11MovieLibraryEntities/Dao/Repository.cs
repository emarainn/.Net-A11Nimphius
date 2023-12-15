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

        public IEnumerable<Movie> GetAll(int numItem)
        {
            if (numItem == 0) 
                return _context.Movies.ToList();
            else 
                return _context.Movies.ToList().Take(numItem);
          
        }

        public void Search(string searchString)
        {
            using (var db = new MovieContext())
            {
                var allMovies = db.Movies
                    .Include(x => x.MovieGenres)
                    .ThenInclude(x => x.Genre)
                    .ToList();

                var movies = allMovies
                    .Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));
            
                foreach (var movie in movies)
                {
                    Console.WriteLine($"Movie {movie.Id}: {movie?.Title}" +
                        $"\nGenres: ");

                    foreach (var genre in movie?.MovieGenres ?? new List<MovieGenre>())
                    {
                        Console.WriteLine($"\t{genre.Genre.Name}");
                    }
                    Console.WriteLine("____________________________________________________");
                }
            }
        }

        public Movie AddMovie()
        {
            // New ID and Movie Title
            var allMovies = _context.Movies.ToList();
            var maxId = allMovies.Count();
            var newMovieId = maxId + 1;

            Console.Write($"New Movie ID: {newMovieId}\nEnter New Movie Title: ");
            var movieT = Console.ReadLine();

            // Adding Movie To Table
            var movie = new Movie();
            movie.Title = movieT;

            _context.Movies.Add(movie);
            _context.SaveChanges();

            // User Input Genres
            var allGenres = _context.Genres.ToList();
            var maxGenreId = allGenres.Count();
            string genreOpt = "";
            do {
                Console.Write("\nYou have the following options: " +
                    "\n\t1) Display ALL Genres" +
                    "\n\t2) Add New Genre" +
                    "\n\t3) Add Genre to Movie Details BY ID" +
                    "\n\tX) To Move On" +
                    "\nEnter Option: ");
                genreOpt = Console.ReadLine().ToUpper();

                if(genreOpt == "1")
                {
                    foreach (var genre in allGenres)
                    {
                        Console.WriteLine($"{genre.Id}: {genre?.Name}");
                    }
                }
                else if(genreOpt == "2") 
                {
                    addGenre();
                }
                else if (genreOpt == "3")
                {
                    List<string> addedGenres = new List<string>();
                    var genreIdInput = 0;
                    do {

                        Console.Write("\nEnter 0 To Stop Loop OR" +
                            "\nEnter ID of Genre you would like to add to movie description: ");

                        while (!int.TryParse(Console.ReadLine(), out genreIdInput) || genreIdInput < -1 || genreIdInput > maxGenreId)
                        {
                            Console.WriteLine("\n**Must Enter Genre ID or 0**");
                            Console.Write("Enter Genre ID OR 0 To Exit Loop: ");
                        }

                        if (genreIdInput == 0)
                        {
                            Console.WriteLine("Exiting LOOP...");
                        }
                        else
                        {
                            // Adding Info To MovieGenres Table Thing
                            var genres = allGenres.FirstOrDefault(x => x.Id == genreIdInput);

                            var movGen = new MovieGenre();
                            movGen.Movie = movie;
                            movGen.Genre = genres;

                            _context.MovieGenres.Add(movGen);
                            _context.SaveChanges();

                        }

                    } while (genreIdInput != 0);
                }
                else if (genreOpt == "X") { break; }
                else
                {
                    Console.WriteLine("** Invalid Input **");
                    genreOpt = "";
                }

            } while (genreOpt != "X");

            return movie;
        }

        public Genre addGenre()
        {
            // New ID and Movie Title
            var allGenres = _context.Genres.ToList();
            var maxId = allGenres.Count();
            var newMax = maxId + 1;

            Console.Write($"New Genre ID: {newMax}\nEnter New Genre: ");
            var newGenre = Console.ReadLine();

            var genre = new Genre();
            genre.Name = newGenre;

            _context.Genres.Add(genre);
            _context.SaveChanges();
            return genre;
        }

        void IRepository.RecordVerification(int userInput)
        {
            var allMovies = _context.Movies
                .Include(x => x.MovieGenres)
                .ThenInclude(x => x.Genre)
                .ToList();

            var record = allMovies.Where(movie => movie.Id == userInput);

            foreach (var movie in record)
            {
                Console.WriteLine($"_______________________________________________" +
                    $"\n\tID: {movie.Id}" +
                    $"\n\tTitle: {movie?.Title}" +
                    $"\n\tRelease Date: {movie?.ReleaseDate:MM-dd-yyyy}" +
                    $"\n\tGenres: ");

                foreach (var genre in movie?.MovieGenres ?? new List<MovieGenre>())
                {
                    Console.WriteLine($"\t\t{genre.Genre.Name}");
                }
            }


        }
    }
}



/*      SEARCH METHOD STUFF

        // ORIGINAL CODE
        public IEnumerable<Movie> Search(string searchString)
        {
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            var temp = listOfMovies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            return temp;
        }

        
        // MY CODE
        // Should Search and pull genres shouldnt have any text in it

        public IEnumerable<Movie> Search(string searchString)
        {
    var stringSearch = "";

    Console.WriteLine("Search Movie Library By Title\n__");
    Console.Write("Enter Movie Title: ");
    stringSearch = Console.ReadLine();

    using (var db = new MovieContext())
    {
        var movie = db.Movies
            .Include(x => x.MovieGenres)
            .ThenInclude(x => x.Genre)
            .FirstOrDefault(mov => mov.Title.Contains(searchString));

        Console.WriteLine($"Movie: {movie?.Title} {movie?.ReleaseDate:MM-dd-yyyy}");

        Console.WriteLine("Genres:");

        foreach (var genre in movie?.MovieGenres ?? new List<MovieGenre>())
        {
            Console.WriteLine($"\t{genre.Genre.Name}");
        }
    }
}

*/
