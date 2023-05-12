using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotChocolate.Authorization;
using HotChocolate.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

public partial class Query
{
    [Authorize]
    public Session RestoreSession(
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal
    )
    {
        var strUserId = claimsPrincipal.FindFirstValue("id");
        int userId;
        int.TryParse(strUserId, out userId);
        var user = db.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            var session = HashUtils.GenerateJWT(user);
            return session;
        }
        throw new QueryException(
            ErrorBuilder
                .New()
                .SetMessage("User not found")
                .SetCode(ErrorCode.RequestError.ToString())
                .Build()
        );
    }
}
