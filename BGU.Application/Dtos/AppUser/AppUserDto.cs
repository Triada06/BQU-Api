namespace BGU.Application.Dtos.AppUser;

public class AppUserDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string MiddleName { get; set; }
    public required string Pin { get; set; }
    public required char Gender { get; set; }
    public required DateTime BornDate { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}