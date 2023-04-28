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
        // Deactivate Films that not in request
        await FilmCollectHelper.DeactivateFilmsNotInListAsync(_logger, _db, request.FilmDetails.Select(f => f.Id).ToList());

        // Add new film to database if not exist, else update HasSessions
        FilmCollectHelper.AddOrUpdateFilms(_logger, _db, request);

        return await Task.FromResult(new SendFilmDetailsRes
        {
            Res = "Request first movie: " + request.FilmDetails[0].FilmName
        });
    }
}
