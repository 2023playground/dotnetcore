using Grpc.Core;

namespace GrpcDataCollect.Services;

public class FilmCollectService : SendFilmDetails.SendFilmDetailsBase
{
    private readonly ILogger<FilmCollectService> _logger;
    public FilmCollectService(ILogger<FilmCollectService> logger)
    {
        _logger = logger;
    }
    public override Task<SendFilmDetailsRes> FilmDetailsReq(FilmDetailList request, ServerCallContext context)
    {
        // Console.WriteLine(request);
        return Task.FromResult(new SendFilmDetailsRes
        {
            Res = "Film details received, This is the first Film's name: " + request.FilmDetails[0].FilmName
        });
    }
}
