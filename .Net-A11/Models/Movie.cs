using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace MovieRepository.Models;

public class Movie
{
    public List<string> Genres { get; set; }
    public long Id { get; set; }
    public string Title { get; set; }
}

public class MovieMap : ClassMap<Movie>
{
    public MovieMap()
    {
        Map(m => m.Id).Index(0).Name("movieId");
        Map(m => m.Title).Index(1).Name("title");
        Map(m => m.Genres).Index(2).Name("genres").TypeConverter<GenresConverter<List<string>>>();
    }
}

public class GenresConverter<T> : DefaultTypeConverter
{
    public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        var array = text.Split("|");

        return array.ToList();
    }

    // instructions found at https://joshclose.github.io/CsvHelper/examples/configuration/class-maps/type-conversion/
    public override string? ConvertToString(object? o, IWriterRow row, MemberMapData memberMapData)
    {
        var genres = (List<string>) o;

        return string.Join(",", genres);
    }
}
