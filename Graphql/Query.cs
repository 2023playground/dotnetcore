using dotnet;

public class Query
{
    public List<Film> GetFilms([Service] AppDbContext db) =>
        db.Films.ToList<Film>();

}
