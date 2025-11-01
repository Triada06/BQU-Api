namespace BGU.Core.Entities;

public class Specialization : BaseEntity
{
    public string Name { get; set; }
    public string FacultyId { get; set; }
    public Faculty Faculty { get; set; }
}