using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(ApiEndPoints.User.SignIn)]
    public async Task<IActionResult> SignIn([FromBody] AppUserSignInDto request)
    {
        var res = await userService.SignInAsync(request);
        if (!res.IsSucceeded)
        {
            return BadRequest(res);
        }
        return Ok(res);
    }
    
}