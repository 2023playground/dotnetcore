public partial class Mutation
{
    public Film CreateFilm(AppDbContext db, int filmId, string filmUrl, string filmName, string mediaFileName, bool hasSessions)
    {
        var film = new Film { FilmId = filmId, FilmUrl = filmUrl, FilmName = filmName, MediaFileName = mediaFileName, HasSessions = hasSessions };
        db.Films.Add(film);
        db.SaveChanges();
        return film;
    }

    public bool DeleteFilmById(AppDbContext db, int filmId)
    {
        var film = db.Films.FirstOrDefault(f => f.FilmId == filmId);
        if (film == null)
        {
            return false;
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
            return false;
        }
        db.Films.RemoveRange(films);
        db.SaveChanges();
        return true;
    }
}
