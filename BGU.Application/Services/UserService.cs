using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Dean;
using BGU.Application.Helpers.Exceptions;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BGU.Application.Services;

public class UserService(
    UserManager<AppUser> userManager,
    IConfiguration config): IUserService
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