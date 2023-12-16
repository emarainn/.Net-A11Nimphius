using Castle.DynamicProxy.Contributors;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        // Library
        IEnumerable<Movie> GetAllMovies(int numItem);
        void Search(string stringSearch);
        Movie AddMovie();
        void RecordVerification(int userInput);


        // Users
        void AddUser();
        void DisplayUsers();
        void DisplayRecentRating();
        void UserAddRating();
        void SearchUsers(int searchId);
        void DisplayUserSearch(int userId);
        int MostRecentUserId();
        

        // Occupations
        public void DisplayOccupations();

    }
}
