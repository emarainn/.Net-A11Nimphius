using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Dao;

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
            Console.Write("\n1) Search Movie" +
                "\n2) Display Movies" +
                "\n3) Add Movies" +
                "\nX) Exit Application" +
                "\nEnter Corisponding Number: ");
            choice = Console.ReadLine().ToUpper();

            if (choice == "1")
            {
                var stringSearch = "Week";

                Console.WriteLine($"Looking For Movies With {stringSearch} In Title" +
                    $"\n___________________________________________");

                // USER INPUT AND VALIDATION
                /*
                // I'd also wrap this in a do while

                Console.Write("Enter Movie Title: ");
                stringSearch = Console.ReadLine();

                if (stringSearch == "")
                {
                    Console.WriteLine("\n**Must Enter Title**");
                }
                */

                _repository.Search(stringSearch).ToList().ForEach(movie => Console.WriteLine($"{movie.Id}| {movie.Title}"));


            }
            else if (choice == "2")
            {
                var movies = _repository.GetAll().ToList();
                movies.ForEach(movie => Console.WriteLine($"{movie.Id}| {movie.Title}"));
            }
            else if (choice == "3")
            {

                Console.WriteLine(_repository.AddMovie());
            }
        }
        while (choice != "X");
        Console.WriteLine("Closing Application...");
    }

}
