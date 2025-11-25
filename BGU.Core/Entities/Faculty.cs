namespace BGU.Core.Entities;

public class Faculty : BaseEntity
{
    public required string Name { get; set; }
    public Dean Dean { get; set; }
    public ICollection<Department> Departments { get; set; }
    public ICollection<Specialization> Specializations { get; set; }
}