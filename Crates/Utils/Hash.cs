public class HashUtils
{
    public static Session GetNewSession(User user)
    {
        var expiry = DateTime.Now.AddDays(1).ToUniversalTime();
        var token = Convert.ToBase64String(
            System.Text.Encoding.UTF8.GetBytes($"{user.Id}:{expiry.ToString()}")
        );

        var session = new Session
        {
            UserId = user.Id,
            ExpiryDate = expiry,
            Token = token
        };

        return session;
    }
}
