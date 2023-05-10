public class HashUtils
{
    public static Session GetNewSession(User user)
    {
        var expiry = DateTime.Now.AddDays(1).ToUniversalTime();
        Console.WriteLine("New Session expiry: " + expiry);
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

    // Breakdown token, analysis if it is expired or not
    public static bool IsExpired(String theToken)
    {
        Console.WriteLine("in is expired");
        var decodedToken = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(theToken));
        Console.WriteLine("decoded token: " + decodedToken);
        var tokenTime = decodedToken.Substring(decodedToken.IndexOf(":") + 1);
        Console.WriteLine("token time: " + tokenTime);
        var theExpiry = DateTime.Parse(tokenTime);
        Console.WriteLine("expiry: " + theExpiry);

        return theExpiry < DateTime.Now.ToUniversalTime();
    }
}
