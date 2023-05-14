using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class HashUtils
{
    public static Session GenerateJWT(User user)
    {
        // Set the secret key used to sign the JWT token

        // Set the issuer and audience for the token
        string issuer = Const.LOCALHOSTURL;
        string audience = Const.LOCALHOSTURL;
        DateTime expires = DateTime.UtcNow.AddDays(1);
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Claims
        var claims = new[]
        {
            new Claim("sub", Const.LOCALHOSTURL),
            new Claim("id", user.Id.ToString()),
            new Claim(ClaimTypes.Role, "admin"),
        };

        // Create the token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = credentials
        };

        // Create the token handler
        var tokenHandler = new JwtSecurityTokenHandler();

        // Create the JWT token
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        var theToken = tokenHandler.WriteToken(token);


        var session = new Session
        {
            User = user,
            ExpiryDate = expires,
            Token = theToken
        };

        return session;

    }

    public static Session GetNewSession(User user)
    {
        var expiry = DateTime.Now.AddDays(1).ToUniversalTime();
        var token = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{user.Id}:{expiry.ToString()}")
        );

        var session = new Session
        {
            User = user,
            ExpiryDate = expiry,
            Token = token
        };

        return session;
    }
}
