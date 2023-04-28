using HotChocolate.Execution;

public partial class Mutation
{
    // Create Film Subscription if user and film exists
    // And if film doesn't have sessions and is activate
    // And if user did not subscribe to this film
    public FilmSubscription CreateFilmSubscription(AppDbContext db, int userId, int filmId)
    {
        var filmSubscription = new FilmSubscription { UserId = userId, FilmId = filmId };
        // Check if user and film exists
        if (
            db.Users.Any(u => u.Id == userId)
            && db.Films.Any(
            f => f.Id == filmId
            && f.HasSessions == false
            && f.IsActivate == true
            ))
        {
            // Add FilmSubscription if user did not subscribe to this film
            if (db.FilmSubscription.Any(fs => fs.UserId == userId && fs.FilmId == filmId))
            {
                throw new QueryException(ErrorBuilder.New().SetMessage("User already subscribed to this film").Build());
            }
            else
            {
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
        }
        else
        {
            // TODO: Provide more information about error
            throw new QueryException(ErrorBuilder.New().SetMessage("User or Film not available").Build());
        }

    }

    // Delete Film Subscription
    public bool DeleteFilmSubscription(AppDbContext db, int userId, int filmId)
    {
        var filmSubscription = db.FilmSubscription.FirstOrDefault(f => f.UserId == userId && f.FilmId == filmId);
        if (filmSubscription == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("User or Film not found").Build());
        }
        db.FilmSubscription.Remove(filmSubscription);
        db.SaveChanges();
        return true;
    }
}