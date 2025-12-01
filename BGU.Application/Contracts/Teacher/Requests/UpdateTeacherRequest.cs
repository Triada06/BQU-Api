using BGU.Core.Entities;

namespace BGU.Application.Contracts.Teacher.Requests;

public sealed record UpdateTeacherRequest(string Name, string Surname, string Email, string  DepartmentId);