public class User{

    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Auth0Id { get; set; }

    public User(string email, string firstName, string lastName, string auth0Id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        Auth0Id = auth0Id;
    }

}