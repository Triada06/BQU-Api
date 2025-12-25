using BGU.Application.Contracts.Department;

namespace BGU.Application.Services.Interfaces;

public interface IDepartmentService
{
    Task<GetAllDepartmentsResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
}