using System.Security.Claims;
using BGU.Api.Helpers;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ResetPasswordRequest = BGU.Application.Contracts.User.ResetPasswordRequest;

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
    public async Task<IActionResult> ResetPassword([FromBody] ChangePasswordDto changePasswordDto, CancellationToken cp)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
        {
            return Unauthorized();
        }

        var response = await userService.ResetPasswordAsync(userId, changePasswordDto.NewPassword, cp);
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

    [HttpPost(ApiEndPoints.Auth.ForgotPassword)]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cp)
    {
        await userService.ForgotPasswordAsync(request, cp);
        return Ok(); // always return Ok to not leak whether email exists
    }

    [HttpPost(ApiEndPoints.Auth.ResetPassword)]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cp)
    {
        var success = await userService.ResetPasswordAsync(request, cp);
        return success ? Ok() : BadRequest("Invalid or expired token.");
    }


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
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token,
        CancellationToken cp)
    {
        var success = await userService.ConfirmEmailAsync(userId, token, cp);
        return success ? Ok() : BadRequest("Invalid or expired token.");
    }

    [Authorize(Roles = "Teacher, Dean")]
    [HttpGet(ApiEndPoints.User.GetAll)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var res = await userService.GetAllAsync(search, page, pageSize);
        return Ok(res);
    }
}