using System.Net;
using System.Security.Claims;
using HotChocolate.Language;
using Newtonsoft.Json.Linq;

public class ValidateToken
{
    private readonly RequestDelegate _next;
    public ValidateToken(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var allowedQueriesAndMutations = new List<string> { "login", "register", "films" };

        // GRPC in whitelist
        if (context.Request.Path.StartsWithSegments("/dataCollect.SendFilmDetails"))
        {
            Console.WriteLine("in GRPC");
            await _next(context);
            return;
        }


        // if (context.Request.Path.StartsWithSegments("/graphql"))
        // {
        //     Console.WriteLine("in GQL");
        //     Console.WriteLine("GQL Path: " + context.Request.Path);
        //     using var streamReader = new StreamReader(context.Request.Body);
        //     var body = await streamReader.ReadToEndAsync();
        //     var jObject = JObject.Parse(body);
        //     var query = jObject["query"]?.ToString()?.ToLowerInvariant();

        //     Console.WriteLine("GQL Body: " + body);
        //     Console.WriteLine("jObject: " + jObject);
        //     Console.WriteLine("GQL query: " + query);


        //     await _next(context);
        //     return;
        // }

        // if request is not in whitelist
        Console.WriteLine("in middleware");
        if (context.Request.Headers.ContainsKey("Authorization") && context.GetType() == typeof(GraphQLRequest))
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

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "John Doe"),
                    new Claim(ClaimTypes.Role, "Admin")
                };

                var identity = new ClaimsIdentity(claims, "MyAuthType");
                var principal = new ClaimsPrincipal(identity);
                context.Items["claims"] = principal;
                context.User.AddIdentity(identity);

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