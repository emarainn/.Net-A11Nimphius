using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using MovieRepository.Models;

namespace MovieRepository.Services;

public partial class MainService : IMainService
{
    private readonly ILogger<MainService> _logger;
    private readonly IRepository _repository;

    public MainService(ILogger<MainService> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public void Invoke()
    {
        MainMenu();
    }
    public void MainMenu()
    {
        _logger.LogInformation("Loading Main Menu...");
        Console.WriteLine("\n\nWelcome To The Main Menu\n__________________________________________________________________");
        string userInput = "";
        do
        {
            Console.Write("You have the following options:" +
                "\n\t1. Add New User" +
                "\n\t2. Add Movie Review (by ID)" +
                "\n\t3. Search Users (by ID)" +
                "\n\tM. Movie Library Menu (part 1 of final)" +
                "\n\tX. Exit Application" +
                "\nEnter Your Option: ");
            userInput = Console.ReadLine().ToUpper();

            if(userInput == "1")
            {
                _repository.AddUser();
                _logger.LogInformation("User Added");
            }
            else if( userInput == "2") 
            {
                AddMovieRating();
            }
            else if( userInput == "3") { SearchUser(); }
            else if (userInput == "M") { MovieLibraryMenu(); }
            else if (userInput == "X") { Console.WriteLine("\nExiting Application..."); break; }
            else { Console.WriteLine("** Invalid Input **"); }

        } while (userInput != "X");
    }

    public void MovieLibraryMenu()
    {
        _logger.LogInformation("Loading Move Library Menu...");
        string choice;
        do
        {
            Console.Write("\n\t1. Search Movie" +
                "\n\t2. Display Movies" +
                "\n\t3. Add Movies" +
                "\n\t4. Edit Movie Title My ID" +
                "\n\tR. Return to Main Menu" +
                "\n\tX. Exit Application" +
                "\nEnter Corresponding Number: ");
            choice = Console.ReadLine().ToUpper();

            if (choice == "1") { SearchInput(); }
            else if (choice == "2") { DisplayMovies(); }
            else if (choice == "3")
            {
                Console.WriteLine(_repository.AddMovie());
                _logger.LogInformation("Movie Added To Database");
            }
            else if (choice == "4")
            {
                EditRecordInput();
                _logger.LogInformation("\nReccord Eddited");
            }
            else if(choice == "R") { MainMenu(); }

        }
        while (choice != "X");
        _logger.LogInformation("Closing Application...");
    }

    public void SearchUser()
    {
        Console.WriteLine("\nSearch User By User ID\n___________________________________________________");

        int searchId = 0;

        do
        {
            Console.Write("\nEnter 0 to see most recent User ID\nOr" +
                "\nEnter ID To Get Corresponding Information: ");
            while (!int.TryParse(Console.ReadLine(), out searchId) || searchId < 0)
            {
                Console.Write("Enter ID To Get Corresponding Information: ");
            }

            if (searchId == 0)
            {
                Console.WriteLine($"Most Recent User ID: {_repository.MostRecentUserId()}");
            }

            _repository.SearchUsers(searchId);

        } while (searchId == 0);
    }

    public void AddMovieRating()
    {
        Console.WriteLine("\n\nRate A Movie\n__________________________________________________________________");
        _repository.UserAddRating();

    }

    public void SearchInput()
    {
        var stringSearch = "";

        Console.WriteLine("\nSearch Movie Library By Title\n__________________________________________________________________");
        Console.Write("Enter Movie Title: ");
        stringSearch = Console.ReadLine();

        _logger.LogInformation("Searching Database...");
        _repository.Search(stringSearch);
    }

    public void DisplayMovies() 
    {
        int numItem = 0;
        Console.Write("Enter number of movies you woild like to view\nOREnter 0 to view ALL movies" +
            "\nEnter Number: ");

        while (!int.TryParse(Console.ReadLine(), out numItem) || numItem < -1)
        {
            _logger.LogInformation("Invalid User Entry");
            Console.WriteLine("\n**Must Enter Num >= 0**");
            Console.Write("Enter amount of movies you want to view: ");
        }

        _logger.LogInformation("Loading movies");
        var movies = _repository.GetAllMovies(numItem).ToList();
        movies.ForEach(movie => Console.WriteLine($"{movie.Id}| {movie.Title}"));
    }

    public void EditRecordInput()
    {
        int userInput;
        Console.WriteLine("Edit Record By ID\n____________________________________________________");

        Console.Write("Enter Movie ID to Update: ");
        
        while (!int.TryParse(Console.ReadLine(), out userInput) || userInput <= 0)
        {
            _logger.LogInformation("Invalid User Entry");
            Console.WriteLine("\n**Must Enter Num > 0**");
            Console.Write("Enter Movie ID to Update: ");
        }

        Console.WriteLine("\nYou Have Chosen The Following Record To Edit: ");
        _repository.RecordVerification(userInput);

        string yn = "";
        do {
            Console.Write("\nIs This The Correct Record? (Y / N): ");
            yn = Console.ReadLine().ToUpper();

            if(yn == "N")
            {
                EditRecordInput();
            }
            else if(yn == "Y")
            {
                Console.Write("Enter Updated Movie Title: ");
                var newTitle = Console.ReadLine();

                using (var db = new MovieContext())
                {
                    var updateMovieTitle = db.Movies.FirstOrDefault(x => x.Id == userInput);

                    Console.WriteLine($"({updateMovieTitle.Id}) {updateMovieTitle.Title}\nHas Been Changed To...");
                    updateMovieTitle.Title = newTitle;

                    db.Movies.Update(updateMovieTitle);
                    db.SaveChanges();

                    Console.WriteLine($"({updateMovieTitle.Id}) {updateMovieTitle.Title}");
                }
                _logger.LogInformation("Movie Updated");

                break;
            }
            else
            {
                Console.WriteLine("\n**Must Enter Y or N **");
            }

        } while (yn != "Y");
    }

}
