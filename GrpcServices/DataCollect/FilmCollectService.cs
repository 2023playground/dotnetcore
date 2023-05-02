namespace GrpcDataCollect.Services;

public partial class FilmCollectService : SendFilmDetails.SendFilmDetailsBase
{
    private readonly ILogger<FilmCollectService> _logger;
    private readonly AppDbContext _db;

    public FilmCollectService(ILogger<FilmCollectService> logger, AppDbContext db)
    {
        _logger = logger;
        _db = db;
    }
}
