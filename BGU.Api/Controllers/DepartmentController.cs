using BGU.Api.Helpers;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class DepartmentController(IDepartmentService departmentService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Department.GetAll)]
    public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
    {
        var res = await departmentService.GetAllAsync(page, pageSize);
        return new ObjectResult(res);
    }
}