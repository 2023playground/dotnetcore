using HotChocolate.Execution;

public partial class Mutation
{
    public Film CreateFilm(AppDbContext db, int filmId, string filmUrl, string filmName, string mediaFileName, bool hasSessions)
    {
        var film = new Film { FilmCode = filmId, FilmUrl = filmUrl, FilmName = filmName, MediaFileName = mediaFileName, HasSessions = hasSessions };
        db.Films.Add(film);
        db.SaveChanges();
        return film;
    }

    public bool DeleteFilmById(AppDbContext db, int Id)
    {
        var film = db.Films.FirstOrDefault(f => f.Id == Id);
        if (film == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("Failed to delete film. Film not found").Build());
        }
        db.Films.Remove(film);
        db.SaveChanges();
        return true;
    }

    public bool DeleteAllFilms(AppDbContext db)
    {
        var films = db.Films.ToList();
        if (films == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("Failed to delete film. Film not found").Build());
        }
        db.Films.RemoveRange(films);
        db.SaveChanges();
        return true;
    }
}
