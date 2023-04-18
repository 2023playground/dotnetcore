public class UserMutation : Mutation
{
    public User Register(
        AppDbContext db,
        string email,
        string firstName,
        string lastName,
        string? auth0Id,
        string? password
    )
    {
        // hash password
        string? hash = null;
        if (password != null)
        {
            hash = PasswordUtils.HashPassword(password);
        }

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Auth0Id = auth0Id,
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
            throw new Exception("User not found");
        }
        if (user.Password == null)
        {
            throw new Exception("User does not have a password, please login via social login");
        }

        //verify password
        if (PasswordUtils.VerifyPassword(password, user.Password) == false)
        {
            throw new Exception("Password is incorrect");
        }

        var session = new Session
        {
            UserId = user.Id,
            ExpiryDate = DateTime.Now.AddDays(1),
            Token = Guid.NewGuid().ToString()
        };
        db.Sessions.Add(session);
        db.SaveChanges();
        return session;
    }

    public Session Logout(AppDbContext db, string token)
    {
        var session = db.Sessions.FirstOrDefault(s => s.Token == token);
        if (session == null)
        {
            throw new Exception("Session not found");
        }
        db.Sessions.Remove(session);
        db.SaveChanges();
        return session;
    }
}
