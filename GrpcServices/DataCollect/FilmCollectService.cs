using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace GrpcDataCollect.Services;

public class FilmCollectService : SendFilmDetails.SendFilmDetailsBase
{
    private readonly ILogger<FilmCollectService> _logger;
    private readonly AppDbContext _db;
    public FilmCollectService(ILogger<FilmCollectService> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    // Void Method to handle incoming Film
    // Add new Film to database if not exist, else update HasSessions
    public void AddToDatabase(Film film)
    {
        var oldFilm = _db.Films.FirstOrDefault(u => u.FilmId == film.FilmId);
        if (oldFilm != null)
        {
            if (oldFilm != null && oldFilm.HasSessions != film.HasSessions)
            {
                oldFilm.HasSessions = film.HasSessions;
                _db.SaveChanges();
            }
            else
            {
                _logger.LogInformation("Film not changed");
            };
        }
        else
        {
            _db.Films.Add(film);
            _db.SaveChanges();
        }
    }

    public override Task<SendFilmDetailsRes> FilmDetailsReq(FilmDetailList request, ServerCallContext context)
    {
        // var newFilm = new Film
        // {
        //     FilmId = 55555,
        //     FilmUrl = "55555",
        //     FilmName = "55555",
        //     MediaFileName = "55555",
        //     HasSessions = false
        // };

        // Handle incoming data to Film object
        for (int i = 0; i < request.FilmDetails.Count; i++)
        {
            var newFilm = new Film
            {
                FilmId = request.FilmDetails[i].Id,
                FilmUrl = request.FilmDetails[i].FilmUrl,
                FilmName = request.FilmDetails[i].FilmName,
                MediaFileName = request.FilmDetails[i].MediaFileName,
                HasSessions = request.FilmDetails[i].HasSessions
            };
            AddToDatabase(newFilm);
        }

        // request.FilmDetails.Select<FilmDetail, Boolean>(film =>
        // {
        //     var newFilm = new Film
        //     {
        //         FilmId = film.Id,
        //         FilmUrl = film.FilmUrl,
        //         FilmName = film.FilmName,
        //         MediaFileName = film.MediaFileName,
        //         HasSessions = film.HasSessions
        //     };
        //     AddToDatabase(newFilm);
        //     return true;
        // });

        return Task.FromResult(new SendFilmDetailsRes
        {
            Res = "Type: " + request.FilmDetails[0]
        });
    }
}
