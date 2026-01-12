using BGU.Api.Helpers;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
[Authorize(Roles = "Dean")]
public class SpecializationController(ISpecializationService specializationService) : ControllerBase
{
    [HttpGet(ApiEndPoints.Specialization.GetAll)]
    public async Task<IActionResult> GetAll(int page = 1, int pageCount = 10)
    {
        var res = await specializationService.GetAllAsync(page, pageCount);
        return new ObjectResult(res);
    }
}