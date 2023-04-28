using GrpcDataCollect.Services;
using Microsoft.EntityFrameworkCore;

public static class FilmCollectHelper
{
    public static void AddToDatabase(ILogger<FilmCollectService> _logger, AppDbContext _db, Film film)
    {
        // Void Method to handle incoming Film
        // Add new Film to database if not exist, else update HasSessions
        var oldFilm = _db.Films.FirstOrDefault(u => u.FilmCode == film.FilmCode);
        if (oldFilm != null)
        {
            if (oldFilm.HasSessions != film.HasSessions)
            {
                if (film.HasSessions == true)
                {
                    // TODO: Send notification to user
                    _logger.LogInformation("This film is up: " + film.FilmName);
                }
                oldFilm.HasSessions = film.HasSessions;
                _db.SaveChanges();
            }
            else
            {
                // RemoveInProduction
                _logger.LogInformation("Film not changed");
            };
        }
        else
        {
            // It's a new film, add in DB
            Console.WriteLine("New film added: " + film.FilmName);
            _db.Films.Add(film);
            _db.SaveChanges();
        }
    }
    public static async Task DeactivateFilmsNotInListAsync(ILogger<FilmCollectService> _logger, AppDbContext _db, List<int> ids)
    {
        var deactivateFilms = await _db.Films
        .Where(f => !ids.Contains(f.FilmCode)).ToListAsync();

        // TODO: Improve performance by using Batch Update
        foreach (var film in deactivateFilms)
        {
            film.IsActivate = false;
            film.HasSessions = false;
            Console.WriteLine("Deactivate film: " + film.FilmName);
        }
        await _db.SaveChangesAsync();
    }
}
