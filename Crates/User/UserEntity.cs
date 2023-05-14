#pragma warning disable CS8618
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

[Index(nameof(Email), IsUnique = true)]
// [Authorize]
public class User
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Password { get; set; }
    public string? Auth0Id { get; set; }

    [GraphQLIgnore]
    public List<Film>? FilmList { get; } = new();
}
