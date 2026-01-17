using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
    //TODO: Manage grades teacher + 
    //TODO: Add a column to seminar "Topic" + ( awaits migration ) 
    //TODO: Get Current Taught Subject Students + 
    //TODO: Remove IsSucceeded, ResponseMessage ( will be discussed again ) 
    //TODO: Update the Formula for GPA + 
}