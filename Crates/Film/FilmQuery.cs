public partial class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Film> GetFilms(AppDbContext db) => db.Films;
    public IQueryable<Film> GetFilmById(AppDbContext db, int filmId) => db.Films.Where(f => f.FilmId == filmId);
}
