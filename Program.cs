using Microsoft.EntityFrameworkCore;
using GrpcDataCollect.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;
using HotChocolate.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

DotNetEnv.Env.Load();
var port = "5000";
try
{
    port = System.Environment.GetEnvironmentVariable("PORT")!;
}
catch (KeyNotFoundException)
{
    Console.WriteLine("PORT not found in .env file");
}

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(
        IPAddress.Parse("0.0.0.0"),
        int.Parse(port!),
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http1;
        }
    );

    options.Listen(
        IPAddress.Parse("0.0.0.0"),
        int.Parse(port!) + 1,
        listenOptions =>
        {
            listenOptions.Protocols = HttpProtocols.Http2;
        }
    );
});


builder.Services.AddGrpc();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseNpgsql(System.Environment.GetEnvironmentVariable("DB_HOST"))
);

// Authentication

var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecretKey));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidIssuer = Const.LOCALHOSTURL,
                ValidAudience = Const.LOCALHOSTURL,
                ValidateIssuerSigningKey = true,
                RequireExpirationTime = true,
                IssuerSigningKey = securityKey
            };
    });

builder.Services.AddAuthorization();

builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .RegisterDbContext<AppDbContext>()
    .AddMutationType<Mutation>()
    .AddProjections()
    .AddFiltering()
;

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




app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL();

app.MapGrpcService<FilmCollectService>();




app.Run();
