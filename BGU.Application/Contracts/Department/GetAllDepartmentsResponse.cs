using BGU.Application.Dtos.Department;
using BGU.Infrastructure.Constants;

namespace BGU.Application.Contracts.Department;

public sealed record GetAllDepartmentsResponse(
    IEnumerable<GetDepartmentDto> DepartmentDto,
    StatusCode StatusCode,
    bool IsSucceeded,
    string ResponseMessage);