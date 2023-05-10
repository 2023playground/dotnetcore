using System.Net;

public class ValidateToken
{
    private readonly RequestDelegate _next;
    public ValidateToken(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        // if request is not in whitelist
        Console.WriteLine("in middleware");
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            Console.WriteLine("Authorization header found");
            var auth = context.Request.Headers["Authorization"];
            Console.WriteLine("Authorization header value: " + auth);
            if (auth.ToString().StartsWith("Bearer "))
            {
                var token = auth.ToString().Substring("Bearer ".Length);
                Console.WriteLine("The token: ", token);

                if (HashUtils.IsExpired(token))
                {
                    Console.WriteLine("You are expired");
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Session expired");
                    return;
                }
                // validate token, pass to next
                Console.WriteLine("token not expired");
                await _next(context);
                return;
            }
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync("Bearer auth header not found");
            return;
        }
        Console.WriteLine("header not found MSG");
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        await context.Response.WriteAsync("Header not found");
        return;
    }

}

public static class ValidateTokenExtensions
{
    public static IApplicationBuilder UseValidateToken(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ValidateToken>();
    }
}