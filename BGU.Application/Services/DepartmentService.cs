using BGU.Application.Contracts.Department;
using BGU.Application.Dtos.Department;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class DepartmentService(IDepartmentRepository departmentRepository) : IDepartmentService
{
    public async Task<GetAllDepartmentsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var departments = (await departmentRepository.GetAllAsync(page, pageSize, tracking)).ToList();
        return new GetAllDepartmentsResponse(departments.Count != 0
            ? departments.Select(x => new GetDepartmentDto(x.Id, x.Name, x.FacultyId))
            : [], StatusCode.Ok, true, ResponseMessages.Success);
    }
}