using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Helpers.Exceptions;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BGU.Application.Services;

public class UserService(UserManager<AppUser> userManager, IConfiguration config) : IUserService
{
    public async Task<string> SignInAsync(AppUserSignInDto appUserDto)
    {
        var user = await userManager.FindByEmailAsync(appUserDto.Email)
                   ?? await userManager.FindByNameAsync(appUserDto.Email);
        if (user is null)
            throw new InvalidLoginCredentialsException();

        var result = await userManager.CheckPasswordAsync(user, appUserDto.PassWord);

        if (!result)
            throw new InvalidLoginCredentialsException();
        return await GenerateJwtToken(user);
    }
    

    private async Task<string> GenerateJwtToken(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
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