namespace BGU.Core.Entities;

public class Department : BaseEntity
{
    public required string Name { get; set; }
    public string FacultyId { get; set; }
    public Faculty Faculty { get; set; }
}