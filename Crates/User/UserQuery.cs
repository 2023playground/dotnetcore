using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;

public partial class Query
{
    public Session RestoreSession(
        AppDbContext db,
        [Service] IHttpContextAccessor httpContextAccessor
    )
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

                    var session = db.Sessions
                        .Include(s => s.User)
                        .FirstOrDefault(s => s.Token == token);

                    if (session != null)
                    {
                        if (session.ExpiryDate < DateTime.Now)
                        {
                            throw new QueryException(
                                ErrorBuilder
                                    .New()
                                    .SetMessage("Session expired")
                                    .SetCode(ErrorCode.RequestError.ToString())
                                    .Build()
                            );
                        }

                        return session;
                    }
                    throw new QueryException(
                        ErrorBuilder
                            .New()
                            .SetMessage("Session not found")
                            .SetCode(ErrorCode.RequestError.ToString())
                            .Build()
                    );
                }
                throw new QueryException(
                    ErrorBuilder
                        .New()
                        .SetMessage("Bearer auth header not found")
                        .SetCode(ErrorCode.RequestError.ToString())
                        .Build()
                );
            }
        }
        throw new QueryException(
            ErrorBuilder
                .New()
                .SetMessage("Header not found")
                .SetCode(ErrorCode.RequestError.ToString())
                .Build()
        );
    }
}
