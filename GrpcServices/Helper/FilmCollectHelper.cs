using GrpcDataCollect;
using GrpcDataCollect.Services;
using Microsoft.EntityFrameworkCore;

public static class FilmCollectHelper
{
    // 1. Add film if not exist             （DB film not in request）
    // 2. Update film if in DB and in request       (DB film in request)  
    //     a. if !oldFilm.Session &&newFilm.Session, oldFilm.Session=newFilm.Session   
    //     b. if !oldFilm.isActivate, oldFilm.isActivate=true
    // 3. Deactivate film if in DB and not in request   (DB activate film not in request)
    //     a. oldFilm.isActivate=false
    public static bool HandleFilmFromRequest(ILogger<FilmCollectService> _logger, AppDbContext _db, FilmDetailList request)
    {
        var logFlag = false;

        // All activated films
        var activatedFilmList = _db.Films.Where(f => f.IsActivate == true).ToList();
        // New films Map
        var newFilmsMap = request.FilmDetails.ToDictionary(f => f.Id, f => f);

        activatedFilmList.ForEach(film =>
        {
            if (newFilmsMap.ContainsKey(film.FilmCode))
            {
                // Activated Film in DB, Update film
                film.FilmUrl = newFilmsMap[film.FilmCode].FilmUrl;
                film.MediaFileName = newFilmsMap[film.FilmCode].MediaFileName;
                film.FilmName = newFilmsMap[film.FilmCode].FilmName;
                film.IsActivate = true;

                if (!film.HasSessions && newFilmsMap[film.FilmCode].HasSessions)
                {
                    // TODO: Notify subscribers if film has sessions
                }
                film.HasSessions = newFilmsMap[film.FilmCode].HasSessions;


                Console.WriteLine("Update film: " + film.FilmName);
                // Remove touched film from newFlimsMap
                newFilmsMap.Remove(film.FilmCode);
            }
            else
            {
                // Activated film not in request, deactivate film
                if (!logFlag)
                {
                    logFlag = true;
                }
                film.IsActivate = false;
                film.HasSessions = false;
                _logger.LogInformation("Deactivate film: " + film.FilmName);
            }

            _db.Entry(film).State = EntityState.Modified;
        });

        // Add new film
        foreach (GrpcDataCollect.FilmDetail film in newFilmsMap.Values)
        {
            // Add new filme
            // Or Reactivated the deactivated film
            var oldFilm = _db.Films.FirstOrDefault(u => u.FilmCode == film.Id);
            if (oldFilm != null)
            {
                oldFilm.IsActivate = true;
                oldFilm.HasSessions = film.HasSessions;
                _db.Films.Update(oldFilm);
                Console.WriteLine("Reactivated film: " + oldFilm.FilmName);
            }
            else
            {
                var newFilm = new Film
                {
                    FilmCode = film.Id,
                    FilmUrl = film.FilmUrl,
                    FilmName = film.FilmName,
                    MediaFileName = film.MediaFileName,
                    HasSessions = film.HasSessions,
                    IsActivate = true
                };
                _db.Films.Add(newFilm);
                Console.WriteLine("Added new film " + film.FilmName);
            }
        }
        _db.SaveChanges();

        return logFlag;
    }


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
                // Update info
                oldFilm.IsActivate = true;
                oldFilm.MediaFileName = request.FilmDetails[i].MediaFileName;
                oldFilm.FilmUrl = request.FilmDetails[i].FilmUrl;

                // If exist, check HasSessions
                if (oldFilm.HasSessions == false && request.FilmDetails[i].HasSessions == true)
                {
                    // If HasSessions changed from false to true, update HasSessions
                    _logger.LogInformation("This film is up: " + oldFilm.FilmName);
                    oldFilm.HasSessions = true;
                }
                _db.Entry(oldFilm).State = EntityState.Modified;
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

    public static Boolean DeactivateFilmsNotInListAsync(ILogger<FilmCollectService> _logger, AppDbContext _db, List<int> ids)
    {
        // var deactivateFilms = _db.Films
        // .Where(f => !ids.Contains(f.FilmCode) && f.IsActivate).ToList();

        var query1 = _db.Films.Where(f => f.IsActivate);
        var query2 = _db.Films.Where(f => ids.Contains(f.FilmCode));

        var deactivateFilms = query1.Except(query2);

        // bool isDeactivate = false;
        // Dictionary<int, Film> allActivatedFilms = _db.Films.Where(f => f.IsActivate).ToDictionary(f => f.FilmCode, f => f);

        // ids.ForEach(id =>
        // {
        //     if (allActivatedFilms.ContainsKey(id))
        //     {
        //         allActivatedFilms[id].IsActivate = false;
        //         allActivatedFilms[id].HasSessions = false;
        //         if (!isDeactivate)
        //         {
        //             isDeactivate = true;
        //         }
        //         _db.Films.Update(allActivatedFilms[id]);
        //         Console.WriteLine("Deactivate film: " + allActivatedFilms[id].FilmName);
        //     }
        // });


        // TODO: Improve performance by using Batch Update
        if (deactivateFilms.Any())
        {
            foreach (var film in deactivateFilms)
            {
                film.IsActivate = false;
                film.HasSessions = false;
                Console.WriteLine("Deactivate film: " + film.FilmName);
            }
        }
        _db.Films.UpdateRange(deactivateFilms);
        _db.SaveChanges();

        // Return true if there is any film deactivated
        return deactivateFilms.Count() != 0;
        // return isDeactivate;
    }
}
