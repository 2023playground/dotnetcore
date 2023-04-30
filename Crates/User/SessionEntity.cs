#pragma warning disable CS8618
public class Session
{
    public int Id { get; set; }
    public User User { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string Token { get; set; }
}
