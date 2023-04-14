using dotnet;

public class Query
{
    public Film GetFilm() =>
        new Film
        {
            Date = DateOnly.FromDateTime(DateTime.Now),
            
        };
}
