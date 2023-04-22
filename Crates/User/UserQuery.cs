using HotChocolate.Execution;

public partial class Query
{
    public User RestoreSession(AppDbContext db, [Service] IHttpContextAccessor httpContextAccessor)
    {
        if (httpContextAccessor.HttpContext != null)
        {
            if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var auth = httpContextAccessor.HttpContext.Request.Headers["Authorization"];
                if (auth.ToString().StartsWith("Bearer "))
                {
                    var token = auth.ToString().Substring("Bearer ".Length);
                    // find session using token

                    var session = db.Sessions.FirstOrDefault(s => s.Token == token);

                    if (session != null)
                    {
                        if (session.ExpiryDate < DateTime.Now)
                        {
                            throw new QueryException(
                                ErrorBuilder.New().SetMessage("Session expired").Build()
                            );
                        }

                        // find user using session
                        var user = db.Users.FirstOrDefault(u => u.Id == session.UserId);
                        if (user != null)
                        {
                            return user;
                        }
                        throw new QueryException(
                            ErrorBuilder.New().SetMessage("User not found").Build()
                        );
                    }
                    throw new QueryException(
                        ErrorBuilder.New().SetMessage("Session not found").Build()
                    );
                }
                throw new QueryException(
                    ErrorBuilder.New().SetMessage("Bearer auth header not found").Build()
                );
            }
        }
        throw new QueryException(ErrorBuilder.New().SetMessage("Header not found").Build());
    }
}
