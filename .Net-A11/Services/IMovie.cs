namespace MovieRepository.Services;

public partial class MainService
{
    public interface IMovie
    {
        public int MovieID { get; set; }
        public string MovieTitle { get; set; }
        public string MovieGenre { get; set; }
    }
}