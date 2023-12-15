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
        string choice;
        do
        {
            _logger.LogInformation("Loading menu options");
            Console.Write("\n1) Search Movie" +
                "\n2) Display Movies" +
                "\n3) Add Movies" +
                "\n4) Edit Movie Title My ID" +
                "\nX) Exit Application" +
                "\nEnter Corresponding Number: ");
            choice = Console.ReadLine().ToUpper();

            if (choice == "1")
            {
                SearchInput();
            }
            else if (choice == "2")
            {
                DisplayMovies();
            }
            else if (choice == "3")
            {
                Console.WriteLine(_repository.AddMovie());
                _logger.LogInformation("Movie Added To Database");
            }
            else if (choice == "4") 
            {
                EditRecordInput();
            }
        }
        while (choice != "X");
        _logger.LogInformation("Closing Application...");
    }

    public void SearchInput()
    {
        var stringSearch = "";

        Console.WriteLine("\nSearch Movie Library By Title\n____________________________________________________");
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
        var movies = _repository.GetAll(numItem).ToList();
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
                Console.WriteLine("Enter Updated Movie Title: ");
                var newTitle = Console.ReadLine();

                using (var db = new MovieContext())
                {
                    var updateMovieTitle = db.Movies.FirstOrDefault(x => x.Id == userInput);
                    Console.WriteLine($"({updateMovieTitle.Id}) {updateMovieTitle.Title}");

                    updateMovieTitle.Title = newTitle;

                    db.Movies.Update(updateMovieTitle);
                    db.SaveChanges();
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
