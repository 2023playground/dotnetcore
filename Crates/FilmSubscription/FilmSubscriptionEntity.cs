using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

[PrimaryKey(nameof(UserId), nameof(FilmId))]
public class FilmSubscription
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FilmId { get; set; }
}
