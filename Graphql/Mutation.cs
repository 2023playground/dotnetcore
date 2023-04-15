using dotnet;

public class Mutation {
    public Film AddFilm([Service] AppDbContext db, string name, string summary) {
        var film = new Film {
            Name = name,
            Summary = summary
        };
        db.Films.Add(film);
        db.SaveChanges();
        return film;
    }
}