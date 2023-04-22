using Microsoft.EntityFrameworkCore;
using GrpcDataCollect.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

DotNetEnv.Env.Load();
var port = "5000";
try
{
    port = System.Environment.GetEnvironmentVariable("PORT");
}
catch (KeyNotFoundException)
{
    Console.WriteLine("PORT not found in .env file");
}

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(
        IPAddress.Loopback,
        int.Parse(port!),
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        }
    );

    options.Listen(
        IPAddress.Loopback,
        int.Parse(port!) + 1,
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        }
    );
});

builder.WebHost.UseUrls($"http://localhost:{port}", $"http://localhost:{int.Parse(port!) + 1}");

builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(System.Environment.GetEnvironmentVariable("DB_HOST"))
);

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<AppDbContext>()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
        Console.WriteLine("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error applying migrations: {ex.Message}");
    }
}

app.MapGraphQL();

app.MapGrpcService<FilmCollectService>();

app.Run();
