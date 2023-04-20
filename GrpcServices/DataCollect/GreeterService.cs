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
        return Task.FromResult(new HelloReply
        {
            Message = "Hello my amigo222" + request.Name
        });
    }
}
