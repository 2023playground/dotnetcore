using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace GrpcDataCollect.Services;

public partial class FilmCollectService : SendFilmDetails.SendFilmDetailsBase
{
    public override Task<SendFilmDetailsRes> FilmDetailsReq(
        FilmDetailList request,
        ServerCallContext context
    )
    {
        var containsDeactivated = false;

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
                containsDeactivated = true;
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

        return Task.FromResult(
            new SendFilmDetailsRes
            {
                Res = "Request first movie: " + request.FilmDetails[0].FilmName,
                IsDeactivated = containsDeactivated
            }
        );
    }
}
