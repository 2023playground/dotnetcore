using HotChocolate.Execution;

public partial class Mutation
{
    // Create Film Subscription
    public bool CreateFilmSubscription(AppDbContext db, int userId, int filmId)
    {
        var filmSubscription = new FilmSubscription { UserId = userId, FilmId = filmId };
        // Create FilmSubscription if user and film exists
        if (db.Users.Any(u => u.Id == userId) && db.Films.Any(f => f.Id == filmId))
        {
            try
            {
                db.FilmSubscription.Add(filmSubscription);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                throw new QueryException(ErrorBuilder.New().SetMessage("Error during creating Film Subscription. ").Build());
            }
        }
        throw new QueryException(ErrorBuilder.New().SetMessage("User or Film not found").Build());
    }

    // Delete Film Subscription
    public bool DeleteFilmSubscription(AppDbContext db, int userId, int filmId)
    {
        var filmSubscription = db.FilmSubscription.FirstOrDefault(f => f.UserId == userId && f.Id == filmId);
        if (filmSubscription == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("User or Film not found").Build());
        }
        db.FilmSubscription.Remove(filmSubscription);
        db.SaveChanges();
        return true;
    }
}