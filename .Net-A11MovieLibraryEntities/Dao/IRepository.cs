using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        IEnumerable<Movie> GetAll(int numItem);
        void Search(string stringSearch);
        Movie AddMovie();
        void RecordVerification(int userInput);

    }
}
