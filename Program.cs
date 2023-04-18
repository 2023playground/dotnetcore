using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);
var port = "5000";
try
{
    port = System.Environment.GetEnvironmentVariable("PORT");
}
catch (KeyNotFoundException)
{
    Console.WriteLine("PORT not found in .env file");
}

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

app.Run("http://0.0.0.0:" + port);
