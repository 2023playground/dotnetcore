using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

[Index(nameof(FilmId), IsUnique = true)]
public class Film
{
    [Key]
    public int Id { get; set; }
    public int FilmId { get; set; }
    public string FilmUrl { get; set; }
    public string FilmName { get; set; }
    public string MediaFileName { get; set; }
    public bool HasSessions { get; set; }
}
