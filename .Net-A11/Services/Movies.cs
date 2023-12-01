namespace MovieRepository.Services;

public partial class MainService
{
    public class Movies : IMovie
    {
        private string[] movieGenre;
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string MovieGenre { get; set; }


        public Movies(int movieID, string? movieTitle, string[] movieGenre)
        {
            MovieID = movieID;
            MovieTitle = movieTitle;
            this.movieGenre = movieGenre;
        }
    }


}
