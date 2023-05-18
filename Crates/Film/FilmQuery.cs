using System.Text.Json;

public partial class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    public IQueryable<Film> GetFilms(AppDbContext db) => db.Films;

    public IQueryable<Film> GetFilmById(AppDbContext db, int filmId) =>
        db.Films.Where(f => f.Id == filmId);

    public async Task<Boolean> TriggerNotificationAsync([Service] EmailNotificationHandler handler)
    {
        var userList = new List<User>();
        userList.Add(new User
        {
            Email = "leebang31698@gmail.com",
            FirstName = "Bang",
            LastName = "Lee"
        });

        userList.Add(new User
        {
            Email = "derekli1995@outlook.com",
            FirstName = "derek",
            LastName = "Lee"
        });

        var film = new Film
        {
            FilmName = "Test Film",
            FilmUrl = "https://google.com",
            MediaFileName = "test.mp4"
        };


        await handler.SendEmailAsync(film, userList);
        return true;
    }
}
