using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
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

    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpPut(ApiEndPoints.User.ChangePassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto, CancellationToken cp)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await userService.ResetPasswordAsync(userId, resetPasswordDto.NewPassword, cp);
        return Ok(response);
    }

    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpPost(ApiEndPoints.User.CheckPassword)]
    public async Task<IActionResult> CheckPassword([FromBody] CheckPasswordDto checkPasswordRequest,
        CancellationToken cp)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await userService.CheckPasswordAsync(userId, checkPasswordRequest.Password, cp);
        return response ? Ok() : BadRequest("Invalid password");
    }
    
    // [HttpPost(ApiEndPoints.Auth.ForgotPassword)]
    // public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request,
    //     CancellationToken cp)
    // {
    //     var response = await userService.ForgotPasswordAsync(request, cp);
    //     return response ? Ok() : BadRequest("Invalid Email");
    // }
    
    // [Authorize(Roles = "Student, Teacher, Dean")]
    // [HttpPost(ApiEndPoints.Auth.ResetPassword)]
    // public async Task<IActionResult> ConfirmEmail([FromBody] ResetPasswordRequest addEmailRequest,
    //     CancellationToken cp)
    // {
    //     var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //     if (userId == null)
    //     {
    //         return Unauthorized();
    //     }
    //
    //     var response = await userService.ConfirmEmailAsync(userId, addEmailRequest.Email, cp);
    //     return response ? Ok() : BadRequest("Invalid password");
    // }
    
       
    [Authorize(Roles = "Student, Teacher, Dean")]
    [HttpPost(ApiEndPoints.User.AddEmail)]
    public async Task<IActionResult> AddEmail([FromBody] AddEmailRequest addEmailRequest,
        CancellationToken cp)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await userService.AddEmailAsync(userId, addEmailRequest, cp);
        return response ? Ok() : BadRequest("Failed to add email");
    }
    
    [HttpGet(ApiEndPoints.User.ConfirmEmail)]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token, CancellationToken cp)
    {
        var success = await userService.ConfirmEmailAsync(userId, token, cp);
        return success ? Ok() : BadRequest("Invalid or expired token.");
    }
}