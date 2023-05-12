using System.Security.Claims;

public partial class Query
{
    // Get all Film Subscriptions for a User
    public IQueryable<FilmSubscription> GetSubscribedFilms(AppDbContext db, ClaimsPrincipal claimsPrincipal)
    {
        var strUserId = claimsPrincipal.FindFirstValue("id");
        int userId;
        int.TryParse(strUserId, out userId);
        return db.FilmSubscription.Where(f => f.UserId == userId);
    }

    // Get all Users for a Film
    public IQueryable<FilmSubscription> GetFilmSubscriber(AppDbContext db, int filmId) => db.FilmSubscription.Where(f => f.FilmId == filmId);
}