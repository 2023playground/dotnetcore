public class FilmQuery:Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Film> GetFilms(AppDbContext db) =>
        db.Films;

}
