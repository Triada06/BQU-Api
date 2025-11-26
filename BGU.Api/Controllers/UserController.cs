using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Dean;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BGU.Api.Controllers;

// [Authorize(Roles = "Student,Teacher,Dean")]
[ApiController]
public class UserController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost(ApiEndPoints.User.SignIn)]
    public async Task<IActionResult> SignIn([FromBody] AppUserSignInDto request)
    {
        var res = await userService.SignInAsync(request);
        return Ok(res);
    }

    [HttpPost(ApiEndPoints.User.SignUp)]
    public async Task<IActionResult> SignUp([FromBody] AppUserSignUpDto request)
    {
        var res = await userService.SignUpAsync(request);
        return Ok(res);
    }

    // [Authorize(Roles = "Teacher")]
    [HttpPost(ApiEndPoints.User.DeanSignUp)]
    public async Task<IActionResult> DeanSignUp([FromBody] DeanRegisterDto request)
    {
        var res = await userService.SignUpDeanAsync(request);
        return Ok(res);
    }
    //TODO: fix the signup, it can send a null request but shouldnt, it shouldnt accept repeated FIN codes, it accpets non-unique emails yet returns Ok
    //TODO: on sing in entering wrong data returns 500 instead 404 or 400`
}