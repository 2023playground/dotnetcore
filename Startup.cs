


// Path: Startup.cs

public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<ValidateToken>();
        app.UseAuthorization();
    }
}