using GrpcDataCollect;
using GrpcDataCollect.Services;
using Microsoft.EntityFrameworkCore;

public static class FilmCollectHelper
{
    // Take FilmDetail and check each film
    // If not exist, add into db
    // If exist, check HasSessions
    public static void AddOrUpdateFilms(ILogger<FilmCollectService> _logger, AppDbContext _db, FilmDetailList request)
    {
        for (int i = 0; i < request.FilmDetails.Count; i++)
        {
            // Check If film exist in DB
            var oldFilm = _db.Films.FirstOrDefault(u => u.FilmCode == request.FilmDetails[i].Id);
            if (oldFilm != null)
            {
                // Flim exist, set activate if not
                if (oldFilm.IsActivate == false)
                {
                    oldFilm.IsActivate = true;
                }

                // If exist, check HasSessions
                if (oldFilm.HasSessions == false && request.FilmDetails[i].HasSessions == true)
                {
                    // If HasSessions changed from false to true, update HasSessions
                    _logger.LogInformation("This film is up: " + oldFilm.FilmName);
                    oldFilm.HasSessions = true;
                    _db.Entry(oldFilm).State = EntityState.Modified;
                }
            }
            else
            {
                // Film not exist, add into db
                var newFilm = new Film
                {
                    FilmCode = request.FilmDetails[i].Id,
                    FilmUrl = request.FilmDetails[i].FilmUrl,
                    FilmName = request.FilmDetails[i].FilmName,
                    MediaFileName = request.FilmDetails[i].MediaFileName,
                    HasSessions = request.FilmDetails[i].HasSessions,
                    IsActivate = true
                };
                _db.Films.Add(newFilm);
                _logger.LogInformation("New film added: " + newFilm.FilmName);
            }
        }

        // One save for all changes
        _db.SaveChanges();

    }

    public static async Task DeactivateFilmsNotInListAsync(ILogger<FilmCollectService> _logger, AppDbContext _db, List<int> ids)
    {
        var deactivateFilms = await _db.Films
        .Where(f => !ids.Contains(f.FilmCode) && f.IsActivate == true).ToListAsync();

        // TODO: Improve performance by using Batch Update
        foreach (var film in deactivateFilms)
        {
            // TODO: Log when deactivate film
            film.IsActivate = false;
            film.HasSessions = false;
            Console.WriteLine("Deactivate film: " + film.FilmName);
        }
        await _db.SaveChangesAsync();
    }
}
