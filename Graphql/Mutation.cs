using dotnet;

public class Mutation {
    public Film AddFilm(AppDbContext db, string name, string summary) {
        var film = new Film(name, summary);
        db.Films.Add(film);
        db.SaveChanges();
        return film;
    }
}