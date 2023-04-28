using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

public class FilmSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FilmId { get; set; }
}
