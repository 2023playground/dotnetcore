public partial class Query
{
    // Get all Film Subscriptions for a User
    public IQueryable<FilmSubscription> GetSubscribedFilms(AppDbContext db, int userId) => db.FilmSubscription.Where(f => f.UserId == userId);

    // Get all Users for a Film
    public IQueryable<FilmSubscription> GetFilmSubscriber(AppDbContext db, int filmId) => db.FilmSubscription.Where(f => f.FilmId == filmId);
}