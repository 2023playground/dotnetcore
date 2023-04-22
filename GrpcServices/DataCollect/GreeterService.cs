using Grpc.Core;

namespace GrpcDataCollect.Services;

public class DataCollectService : Greeteror.GreeterorBase
{
    private readonly ILogger<DataCollectService> _logger;
    public DataCollectService(ILogger<DataCollectService> logger)
    {
        _logger = logger;
    }

    public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
    {
        Console.WriteLine(request);
        Console.WriteLine(request.Name);
        return Task.FromResult(new HelloReply
        {
            Message = "Hello my amigo222" + request.Name
        });
    }
}

public class MovieCollectService : SendMovieDetails.SendMovieDetailsBase
{
    private readonly ILogger<MovieCollectService> _logger;
    public MovieCollectService(ILogger<MovieCollectService> logger)
    {
        _logger = logger;
    }
    public override Task<SendMovieDetailsRes> MovieDetailsReq(MovieDetailList request, ServerCallContext context)
    {
        Console.WriteLine(request);
        return Task.FromResult(new SendMovieDetailsRes
        {
            Res = "Movie details received, This is the first movie's name: " + request.MovieDetails[0]
        });
    }
}
