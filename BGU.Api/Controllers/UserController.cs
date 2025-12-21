using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Dean;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BGU.Api.Controllers;

[ApiController]
public class UserController(IUserService userService, UserManager<AppUser> userManager) : ControllerBase
{
    //todo: fin should be unique, fix that
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
        // return new ObjectResult(res);
    }

    [Authorize(Roles = "Teacher")]
    [HttpPost(ApiEndPoints.User.SignUp)]
    public async Task<IActionResult> SignUp([FromBody] AppUserSignUpDto request)
    {
        var res = await userService.SignUpAsync(request);
        return Ok(res);
    }

    [Authorize(Roles = "Dean")]
    [HttpPost(ApiEndPoints.User.DeanSignUp)]
    public async Task<IActionResult> DeanSignUp([FromBody] DeanRegisterDto request)
    {
        var res = await userService.SignUpDeanAsync(request);
        return Ok(res);
    }

    // [AllowAnonymous]
    // [HttpDelete(ApiEndPoints.User.Delete)]
    //  public async Task<IActionResult> Delete([FromRoute] string id)
    // {
    //     var user = await userManager.FindByIdAsync(id);  
    //     await userManager.DeleteAsync(user);
    //     return Ok();
    // }
}