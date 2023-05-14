using System.Security.Claims;
using HotChocolate.Execution;

public partial class Mutation
{
    // Create Film Subscription if user and film exists
    // And if film doesn't have sessions and is activate
    // And if user did not subscribe to this film
    public FilmSubscription CreateFilmSubscription(AppDbContext db, ClaimsPrincipal claimsPrincipal, int filmId)
    {
        var strUserId = claimsPrincipal.FindFirstValue("id");
        int userId;
        int.TryParse(strUserId, out userId);
        var filmSubscription = new FilmSubscription { UserId = userId, FilmId = filmId };

        // Add FilmSubscription if user did not subscribe to this film
        try
        {
            db.FilmSubscription.Add(filmSubscription);
            db.SaveChanges();
            return filmSubscription;
        }
        catch (Exception)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("Error during creating Film Subscription.").Build());
        }
    }

    // Delete Film Subscription
    public bool DeleteFilmSubscription(AppDbContext db, ClaimsPrincipal claimsPrincipal, int subscriptionId)
    {
        var strUserId = claimsPrincipal.FindFirstValue("id");
        int userId;
        int.TryParse(strUserId, out userId);
        var filmSubscription = db.FilmSubscription.FirstOrDefault(f => f.Id == subscriptionId && f.UserId == userId);
        if (filmSubscription == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("User or Film not found").Build());
        }
        db.FilmSubscription.Remove(filmSubscription);
        db.SaveChanges();
        return true;
    }
}