using System.ComponentModel.DataAnnotations.Schema;

namespace BGU.Core.Entities;

public class Subject : BaseEntity
{
    public int CreditsNumber { get; set; }


    public required string Name { get; set; }
    public string TeacherCode { get; set; }

    public string DepartmentId { get; set; }
    public Department Department { get; set; }
    
}