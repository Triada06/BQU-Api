using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[Authorize(Roles = "Student,Teacher,Dean")]
[ApiController]
public class UserController(IUserService userService, RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpPost(ApiEndPoints.AppUser.SignIn)]
    public async Task<IActionResult> SignIn([FromBody] AppUserSignInDto request)
    {
        var res = await userService.SignInAsync(request);
        return Ok(res);
    }
    
    [AllowAnonymous]
    [HttpPost("api/addroles")]
    public async Task<IActionResult> CreateRole()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            if (!await roleManager.RoleExistsAsync(role.ToString()))
                await roleManager.CreateAsync(new IdentityRole(role.ToString()));
        }

        return Ok();
    }
}