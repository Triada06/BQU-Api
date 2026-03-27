using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BGU.Application.Services;

public class UserService(
    IEmailSender<AppUser> emailSender,
    UserManager<AppUser> userManager,
    IConfiguration config) : IUserService
{
    public async Task<AuthResponse> SignInAsync(AppUserSignInDto deanUserDto)
    {
        var user = await userManager.FindByNameAsync(deanUserDto.Username);
        if (user is null)
        {
            return new AuthResponse(null, null, false, StatusCode.BadRequest, ["Invalid login credentials"]);
        }

        var result = await userManager.CheckPasswordAsync(user, deanUserDto.Password);

        return !result
            ? new AuthResponse(null, null, false, StatusCode.BadRequest, ["Invalid login credentials "])
            : new AuthResponse(await GenerateJwtToken(user), DateTime.UtcNow.AddDays(7), true, StatusCode.Ok, null);
    }

    public async Task<AuthResponse> ResetPasswordAsync(string userId, string newPassword, CancellationToken cp)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new AuthResponse(null, null, false, StatusCode.BadRequest, ["User not found"]);
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var res = await userManager.ResetPasswordAsync(user, token, newPassword);

        if (!res.Succeeded)
        {
            return new AuthResponse(null, null, false, StatusCode.BadRequest,
                res.Errors.Select(e => e.Description).ToArray());
        }

        return new AuthResponse(await GenerateJwtToken(user), DateTime.UtcNow.AddDays(7), true, StatusCode.Ok, null);
    }

    public async Task<bool> CheckPasswordAsync(string userId, string password, CancellationToken cp)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return false;
        }

        var result = await userManager.CheckPasswordAsync(user, password);
        return result;
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

        var result = await userManager.ConfirmEmailAsync(user, decodedToken);
        return result.Succeeded;
    }

    public async Task<bool> AddEmailAsync(string userId, AddEmailRequest request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
            return false;

        // sets email + marks EmailConfirmed = false internally
        var result = await userManager.SetEmailAsync(user, request.Email);
        if (!result.Succeeded)
            return false;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        // encode because token contains special chars
        var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var confirmationLink = $"http://localhost:3000/confirm-email?userId={userId}&token={encodedToken}";
        
        await emailSender.SendConfirmationLinkAsync(user, request.Email, 
            confirmationLink);

        return true;
    }


    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id)
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));


        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = creds,
            Issuer = config["Jwt:Issuer"],
            Audience = config["Jwt:Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwt = tokenHandler.WriteToken(token);

        return jwt;
    }
}