public partial class Mutation
{
    public Film CreateFilm(AppDbContext db, int filmId, string filmUrl, string filmName, string mediaFileName, bool hasSessions)
    {
        var film = new Film { FilmId = filmId, FilmUrl = filmUrl, FilmName = filmName, MediaFileName = mediaFileName, HasSessions = hasSessions };
        db.Films.Add(film);
        db.SaveChanges();
        return film;
    }
}
