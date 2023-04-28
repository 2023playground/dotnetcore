using Grpc.Core;

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
    // Add new Film to database if not exist, else update HasSessions
    // Change Film to inactive if film in DB but not in request
    public async override Task<SendFilmDetailsRes> FilmDetailsReq(FilmDetailList request, ServerCallContext context)
    {
        // Deactivate Films not in request
        await FilmCollectHelper.DeactivateFilmsNotInListAsync(_logger, _db, request.FilmDetails.Select(f => f.Id).ToList());

        // Add new film to database if not exist, else update HasSessions
        // TODO: Any faster way to do? Try to reduce DB call
        for (int i = 0; i < request.FilmDetails.Count; i++)
        {
            var newFilm = new Film
            {
                FilmId = request.FilmDetails[i].Id,
                FilmUrl = request.FilmDetails[i].FilmUrl,
                FilmName = request.FilmDetails[i].FilmName,
                MediaFileName = request.FilmDetails[i].MediaFileName,
                HasSessions = request.FilmDetails[i].HasSessions,
                IsActivate = true
            };
            FilmCollectHelper.AddToDatabase(_logger, _db, newFilm);
        }

        return await Task.FromResult(new SendFilmDetailsRes
        {
            Res = "Request first movie: " + request.FilmDetails[0].FilmName
        });
    }
}
