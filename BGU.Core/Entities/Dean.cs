namespace BGU.Core.Entities;

public class Dean : BaseEntity
{
    public string PhoneNumber { get; set; }
    public string AppUserId { get; set; }
    public AppUser AppUser { get; set; }

    public string FacultyId { get; set; }
    public Faculty Faculty { get; set; }

    public string RoleName { get; set; }
}