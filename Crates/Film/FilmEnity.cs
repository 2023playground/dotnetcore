using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using HotChocolate.Authorization;

#pragma warning disable CS8618

[Index(nameof(FilmCode), IsUnique = true)]
// [Authorize]
public class Film
{
    public int Id { get; set; }
    public int FilmCode { get; set; }
    public string FilmUrl { get; set; }
    public string FilmName { get; set; }
    public string MediaFileName { get; set; }
    public bool HasSessions { get; set; }
    public bool IsActivate { get; set; }

    [GraphQLIgnore]
    public List<User>? UserList { get; } = new();
}
