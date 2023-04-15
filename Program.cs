using dotenv.net;
using Microsoft.EntityFrameworkCore;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);
var envVars = DotEnv.Read();

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

app.Run();
