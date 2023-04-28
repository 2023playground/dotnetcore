using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

[Index(nameof(FilmCode), IsUnique = true)]
public class Film
{
    public int Id { get; set; }
    public int FilmCode { get; set; }
    public string FilmUrl { get; set; }
    public string FilmName { get; set; }
    public string MediaFileName { get; set; }
    public bool HasSessions { get; set; }
    public bool IsActivate { get; set; }
    public List<User>? UserList { get; } = new();
}
