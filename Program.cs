using dotenv.net;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
var envVars = DotEnv.Read();
var port = "5000";
try{
    port = envVars["PORT"];
} catch (KeyNotFoundException)
{
    Console.WriteLine("PORT not found in .env file");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(envVars["DB_HOST"]));

builder.Services
    .AddGraphQLServer()
    .RegisterDbContext<AppDbContext>()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddProjections().AddFiltering();

var app = builder.Build();

app.MapGraphQL();

app.Run("http://0.0.0.0:"+port);
