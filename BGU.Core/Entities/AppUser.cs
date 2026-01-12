using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BGU.Core.Entities;

public class AppUser : IdentityUser, IBaseEntity
{
    [MaxLength(15)] public required string Name { get; set; }
    [MaxLength(15)] public required string Surname { get; set; }
    [MaxLength(15)] public required string MiddleName { get; set; }
    public required char Gender { get; set; }
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}