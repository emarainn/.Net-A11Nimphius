using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
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


        // Movie
        public IEnumerable<Movie> GetAllMovies(int numItem)
        {
            if (numItem == 0) 
                return _context.Movies.ToList();
            else 
                return _context.Movies.ToList().Take(numItem);
          
        }

        public void SearchUsers(int searchId)
        {
            var allUsers = _context.Users.ToList();
            var maxUserId = allUsers.Count();
            int userId = 0; 

            //I could use try catch here
            if(searchId > 0 && searchId <= maxUserId)
            {
                userId = searchId;
                DisplayUserSearch(userId);
            }
            else
            {
                Console.WriteLine("\n** Invalid ID **");
            }
        }

        public void Search(string searchString)
        {
            var allMovies = _context.Movies
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
                Console.WriteLine(Line);
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
            Console.WriteLine("\n\nTime To Add Genres To Movie Description" +
                "\n--> hit any key to continue to option menu <--");
            Console.ReadLine();

            var allGenres = _context.Genres.ToList();
            var maxGenreId = allGenres.Count();
            string genreOpt = "";
            do {
                Console.Write("You have the following options: " +
                    "\n\t1) Display ALL Genres" +
                    "\n\t2) Add New Genre" +
                    "\n\t3) Add Genre to Movie Details BY ID" +
                    "\n\tX) To Move On" +
                    "\nEnter Option: ");
                genreOpt = Console.ReadLine().ToUpper();

                if(genreOpt == "1") //All Genres
                {
                    foreach (var genre in allGenres) 
                    { 
                        Console.WriteLine($"{genre.Id}: {genre?.Name}"); 
                    }
                }
                else if(genreOpt == "2") // Add NEW Genre
                { 
                    AddGenre();
                }
                else if (genreOpt == "3") // Add Genre To Movie
                {
                    List<string> addedGenres = new List<string>();
                    var genreIdInput = 0;
                    do {

                        Console.Write("\nEnter 0 To Stop Loop OR" +
                            "\nEnter ID of Genre you would like to add to movie description: ");

                        // Loop Entry Validation
                        while (!int.TryParse(Console.ReadLine(), out genreIdInput) || genreIdInput < -1 || genreIdInput > maxGenreId)
                        {
                            Console.WriteLine("\n**Must Enter Genre ID or 0**");
                            Console.Write("Enter Genre ID OR 0 To Exit Loop: ");
                        }

                        // Displaying Movies
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

        public Genre AddGenre()
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


        // User
        public void AddUser() // I KNOW THAT I SHOULDNT HAVE USER INPUT/TEXT IN HERE
        {
            // User ID
            var allUsers = _context.Users.ToList();
            var maxId = allUsers.Count();
            var newUserId = maxId + 1;
            Console.WriteLine("\n______________________________________________________\nADD NEW USER");
            Console.WriteLine($"\nYour User ID: {newUserId}");

            // User Age 
            int userAge;
            Console.Write("Enter your age: ");
            // Age Validation
            while (!int.TryParse(Console.ReadLine(), out userAge) || userAge <= 0)
            {
                Console.WriteLine("\n**You can't be younger than 0**");
                Console.Write("Enter Your Age: ");
            }

            // User Gender
            string userGender = "";
            do
            {
                Console.Write("\nChoose User Gender " +
                    "\nsorry for not being 2023 friendly" +
                    "\n\tF = Female" +
                    "\n\tM = Male" +
                    "\nEnter Gender: ");
                userGender = Console.ReadLine().ToUpper();

                // User Gender Validation
                if ( userGender == "F" ){ userGender = "F"; }
                else if ( userGender == "M" ){ userGender = "M"; }
                else
                {
                    Console.WriteLine("** Invalid Input **");
                    userGender = "";
                }
            } while (userGender == "");

            // User Zip Code
            var _usZipRegEx = @"^\d{5}(?:[-\s]\d{4})?$"; //For Validation
            string userZipCode = "";
            do
            {
                Console.Write("\nEnter Zip Code: ");
                userZipCode = Console.ReadLine();

                // User Zip Code Validaiton
                if (Regex.Match(userZipCode, _usZipRegEx).Success)
                {
                    Console.WriteLine($"\n{userZipCode} Is A Valid Zip Code Entry");
                }
                else
                {
                    Console.WriteLine($"\n{userZipCode} Is An Invalid Zip Code Entry");
                }

            } while (!Regex.Match(userZipCode, _usZipRegEx).Success);

            //User Occupation
            var allOccs = _context.Occupations.ToList();
            var maxOccId = allOccs.Count;
            string occOption = "";

            do
            {
                Console.Write("\n**********************************************" +
                    "\nNOTICE: YOU MUST ADD AN OCCUPATION TO USER INFO" +
                    "\n**********************************************" +
                    "\nYou have the following options: " +
                    "\n\t1) Display ALL Occupations" +
                    "\n\t2) Add New Occupation" +
                    "\n\t3) Add Occupation To User Info"+
                    "\nEnter Option: ");
                occOption = Console.ReadLine().ToUpper();

                if (occOption == "1") { DisplayOccupations(); occOption = ""; }
                else if (occOption == "2") { AddOccupation(); occOption = ""; }
                else if (occOption == "3")
                {
                    // Adding Occupation To Table
                    int occIdInput = 0;
                    do
                    {
                        // Getting Occupation ID
                        Console.Write("\nEnter ID Of Occupation: ");
                        while (!int.TryParse(Console.ReadLine(), out occIdInput) || occIdInput <= 0 || occIdInput > maxOccId)
                        {
                            Console.WriteLine("\n** Invalid Occupation ID **");
                            Console.Write("Enter ID Of Occupation: ");
                        }

                        var userOccupation = _context.Occupations.FirstOrDefault(x => x.Id == occIdInput);
                        
                        // Adding User To Table
                        var user = new User();
                        user.Age = userAge;
                        user.Gender = userGender;
                        user.ZipCode = userZipCode;
                        user.Occupation = userOccupation;

                        _context.Users.Add(user);
                        _context.SaveChanges();

                        Console.WriteLine($"\nThe Following Has Been Added..." +
                            $"\n_______________________________________" +
                            $"\n\tUserId| {user.Id}" +
                            $"\n\tAge| {user.Age}" +
                            $"\n\tGender| {user.Gender}" +
                            $"\n\tZip Code| {user.ZipCode}" +
                            $"\n\tOccupation ID| {userOccupation.Id}" +
                            $"\n\tOccupation| {userOccupation.Name}" +
                            $"\n_______________________________________\n");

                    } while (occIdInput == null);
                }
                else 
                { 
                    Console.WriteLine("\n** Invalid Input **");
                    occOption = "";
                }

            } while (occOption == "");

        } 

        public void DisplayUsers()
        {
            var users = _context.Users.ToList();
            foreach (var user in users)
            {
                Console.WriteLine($"{user.Id}| Age: {user.Age} | Gender: {user.Gender} | Zip Code: {user.ZipCode}");
            }
        }

        public void DisplayRecentRating()
        {
            var ratings = _context.UserMovies.ToList();
            var maxId = ratings.Count();

            var mostRecentRate = ratings.FirstOrDefault(x => x.Id == maxId);

            Console.WriteLine(mostRecentRate.Id);

        }

        public void UserAddRating()
        {
            // Max User ID
            var allUsers = _context.Users.ToList();
            var maxUserId = allUsers.Count();

            // Getting User ID
            int userInputId = 0;
            string enterIdTxt = "Please Enter Your User ID Bellow...\nIf you don't know it enter 0 to display avalable UserIDs.\nUser Input: ";
            do
            {
                Console.Write(enterIdTxt);
                while (!int.TryParse(Console.ReadLine(), out userInputId) || userInputId < 0 || userInputId > maxUserId)
                {
                    Console.WriteLine("\n** Invalid User ID **");
                    Console.Write(enterIdTxt);
                }

                if( userInputId == 0)
                {
                    Console.WriteLine($"\nThe Following User IDs Are Populated: 1 - {maxUserId}");
                    DisplayUsers();
                }
            } while (userInputId == 0);

            var userId = allUsers.FirstOrDefault(x => x.Id == userInputId);
            Console.WriteLine("Your ID IS: " + userId.Id);

            // Max Movie Id
            var allMovies = _context.Movies.ToList();
            var maxMovieId = allMovies.Count();

            // Getting Movie ID
            int userMovieId = 0;
            string enterMovieTxt = "Enter Movie ID Bellow...\nIf you don't know the ID of the movie enter 0 to search Movie Library." +
                "\nUser Input: ";

            do
            {

                Console.Write(enterMovieTxt);
                while (!int.TryParse(Console.ReadLine(), out userMovieId) || userMovieId < 0 || userMovieId > maxUserId)
                {
                    Console.WriteLine("\n\n** Invalid User ID **");
                    Console.Write(enterIdTxt);
                }

                if (userInputId == 0)
                {
                    Console.WriteLine("\nYou chose to search Movies..." + Line);

                    Console.Write("Enter Movie Title To Search: ");
                    string stringSearch = Console.ReadLine();
                    Search(stringSearch);
                }

            } while (userMovieId == 0);

            // Display Movie
            var ratedMovie = allMovies.FirstOrDefault(x => x.Id == userMovieId);
            Console.WriteLine($"{Line}\nThe Following Movie Was Chosen To Be Rated:" +
                $"\n\tMovie ID| {ratedMovie.Id}" +
                $"\n\tMoie Title| {ratedMovie.Title}");
            
            // Inform User of Next Task (rating movie)
            Console.WriteLine("\n\nIt's time to rate the movie!" +
                "\n--> hit any button to continue <--");
            Console.ReadLine();

            // Get Rating
            var allRatings = _context.UserMovies.ToList();
            int userRatingInput = 0;
            var userRating = 0;
            string enterRatingTxt = $"\nYou have the following options...\n\t1. Horribe\n\t2. Wouldn't watch again" +
                $"\n\t3. Not great but meh\n\t4. I wouldn't complain if I saw it again\n\t5. Nutt\nEnter Movie Rating: ";

            Console.Write(enterRatingTxt);
            while (!int.TryParse(Console.ReadLine(), out userRatingInput) || userRatingInput < 1 || userRatingInput > 5)
            {
                Console.WriteLine("\n\n** Rating Must Be A Number 1 - 5 **");
                Console.Write(enterRatingTxt);
            }

            if(userRatingInput >= 1 && userRatingInput <= 5)
            {
                userRating = userRatingInput;
            }

            Console.WriteLine($"\nYou gave the movie a: {userRating} / 5");

            // Adding User Rating To Table
            var userMovie = new UserMovie();
            userMovie.RatedAt = DateTime.Now;
            userMovie.User = userId;
            userMovie.Movie = ratedMovie;
            userMovie.Rating = userRating;

            _context.UserMovies.Add(userMovie);
            _context.SaveChanges();

            // I KNOW I COULD HAVE USED A ToString() for this
            Console.WriteLine($"{Line}\nThe following has been added to the database under the ID {userMovie.Id}..." +
                $"\nInformation documented at| {userMovie.RatedAt}" +
                $"\n\tUser ID| {userId.Id}" +
                $"\n\tRated Movie Information|" +
                $"\n\t\tID - {ratedMovie.Id}" +
                $"\n\t\tTitle - {ratedMovie.Title}" +
                $"\n\tRating| {userRating}");

        }

        public int MostRecentUserId()
        {
            var allUsers = _context.UserMovies.ToList();
            var maxId = allUsers.Count();
            return maxId;
        }

        public void DisplayUserSearch(int userId)
        {
            // Getting All Users
            var allUsers = _context.Users
                .Include(x => x.Occupation)
                .Include(x => x.UserMovies)
                .ToList();

            var user = allUsers
                .FirstOrDefault(x => x.Id == userId);

            // M = Male & F = Female
            string gender = "";
            if (user.Gender == "M")
            {
                gender = "Male";
            }
            else if (user.Gender == "F")
            {
                gender = "Female";
            }

            // Display Rating Info
            Console.WriteLine($"\nThe following Information was found...{Line}" +
                $"\n\tUser Id| {user.Id}" +
                $"\n\tGender| {gender}" +
                $"\n\tOccupation| {user.Occupation.Name}" +
                $"\n\tUser's Ratings");

            // Display Ratings
            var allRatings = _context.UserMovies
                .Include(x => x.Movie)
                .Include(x => x.User)
                .ToList();
            var ratings = allRatings
                .Where(x => x.Id == userId).ToList();

            Console.WriteLine("User's Ratings");
            int count = 1;
            foreach (var rating in ratings)
            {
                if (rating == null) 
                {
                    var date = DateTime.Now.ToString("MMMM dd, yyyy");
                    Console.WriteLine($"No Ratings As Of {date}");
                }
                else
                {
                    Console.WriteLine($"{Line2}\n\tRating Number| {count}" +
                        $"\n\tRating ID| {rating.Id}" +
                        $"\n\tUser's Rating| {rating.Rating}" +
                        $"\n\tMovie Rated| {rating.Movie.Title}");
                    count++;
                }

            }

        }


        // Occupation
        public void DisplayOccupations()
        {
            var occs = _context.Occupations.ToList();

            foreach(var occ in occs)
            {
                Console.WriteLine($"{occ.Id}| {occ.Name}");
            }
        }

        public void AddOccupation()
        { 
            var allOccupations = _context.Occupations.ToList();
            var maxId = allOccupations.Count();
            var newMax = maxId + 1;
            string newOcc = "";
            Console.WriteLine($"\nNew Occupation ID: {newMax}");

            Console.Write("Enter Occupation Title: ");
            newOcc = Console.ReadLine();

            var occupation = new Occupation();
            occupation.Name = newOcc;

            _context.Occupations.Add(occupation); 
            _context.SaveChanges();

            Console.WriteLine($"{occupation.Id}| {occupation.Name} has been added to database");

        }


        public string Line2 = "__________________________________________________________________";
        public string Line = "\n__________________________________________________________________";
    }
}
