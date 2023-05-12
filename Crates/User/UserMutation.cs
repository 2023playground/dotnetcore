using HotChocolate.Execution;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.IdentityModel.Tokens.Jwt;
using System.Net;

public partial class Mutation
{
    public User Register(
        AppDbContext db,
        string email,
        string firstName,
        string lastName,
        string password
    )
    {
        // hash password
        var hash = PasswordUtils.HashPassword(password);

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Password = hash
        };
        db.Users.Add(user);
        db.SaveChanges();
        return user;
    }

    public User UpdateUserInfo(AppDbContext db, int id, string firstName, string lastName)
    {
        var user = new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
        };
        db.Users.Update(user);
        db.SaveChanges();
        return user;
    }

    public Session Login(AppDbContext db, string email, string password)
    {
        var user = db.Users.FirstOrDefault(u => u.Email == email);
        if (user == null)
        {
            throw new QueryException(ErrorBuilder.New().SetMessage("User not found").Build());
        }
        if (user.Password == null)
        {
            throw new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage("User does not have a password, please login via social login")
                    .SetCode(ErrorCode.RequestError.ToString())
                    .Build()
            );
        }

        //verify password
        if (PasswordUtils.VerifyPassword(password, user.Password) == false)
        {
            throw new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage("Password is incorrect")
                    .SetCode(ErrorCode.RequestError.ToString())
                    .Build()
            );
        }
        return HashUtils.GenerateJWT(user);
    }

    //TODO: add logout JWT to DB
    public Session Logout(AppDbContext db, string token)
    {
        var session = db.Sessions.FirstOrDefault(s => s.Token == token);
        if (session == null)
        {
            throw new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage("Session not found")
                    .SetCode(ErrorCode.RequestError.ToString())
                    .Build()
            );
        }
        db.Sessions.Remove(session);
        db.SaveChanges();
        return session;
    }

    public async Task<Session> SocialLogin(AppDbContext db, string accessToken)
    {
        //get user
        Console.WriteLine("In social login");
        var handler = new JwtSecurityTokenHandler();
        var auth0User = handler.ReadJwtToken(accessToken);

        if (auth0User.Subject == null)
        {
            throw new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage("User not registered via social app:auth0")
                    .SetCode(ErrorCode.RequestError.ToString())
                    .Build()
            );
        }


        var user = db.Users.FirstOrDefault(u => u.Auth0Id == auth0User.Subject);

        if (user == null)
        {
            //register user
            var client = new RestClient(System.Environment.GetEnvironmentVariable("AUTH0_DOMAIN")!);
            var request = new RestRequest("/userinfo", Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessToken);
            var response = await client.ExecuteAsync(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var json = JObject.Parse(response.Content!);
                if (json.ContainsKey("sub"))
                {
                    var auth0Id = json["sub"]!.ToString();
                    var email = json["email"]?.ToString();
                    var firstName = json["given_name"]?.ToString();
                    var lastName = json["family_name"]?.ToString();

                    var newUser = new User
                    {
                        Email = email,
                        FirstName = firstName,
                        LastName = lastName,
                        Auth0Id = auth0Id
                    };
                    db.Users.Add(newUser);
                    db.SaveChanges();

                    // Create a new JWT
                    return HashUtils.GenerateJWT(newUser);
                }
            }

            throw new QueryException(
                ErrorBuilder
                    .New()
                    .SetMessage("Error getting user info from Auth0")
                    .SetCode(ErrorCode.RequestError.ToString())
                    .Build()
            );
        }
        else
        {
            //create a new session
            Console.WriteLine("User Id is: " + user.Id);
            return HashUtils.GenerateJWT(user);

        }
    }
}
