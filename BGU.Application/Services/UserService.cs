using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Helpers.Exceptions;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Core.Enums;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BGU.Application.Services;

public class UserService(
    UserManager<AppUser> userManager,
    IConfiguration config,
    IUserRepository userRepository,
    IStudentRepository studentRepository)
    : IUserService
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

    public async Task<AuthResponse> SignUpAsync(AppUserSignUpDto appUser)
    {
        var template = appUser.UserName.Trim();
        var isExists = await userRepository.AnyAsync(u =>
            u.UserName != null && u.UserName.Trim() == template);
        if (isExists)
        {
            return new AuthResponse(null, null, false, ["User already exists, try sign in."]);
        }

        var user = new AppUser //TODO:needs to be changed
        {
            UserName = appUser.UserName,
            Email = appUser.Email,
            Name = appUser.Name,
            Surname = appUser.Surname,
            MiddleName = appUser.MiddleName,
            Pin = appUser.PinCode,
            Gender = appUser.Gender,
            BornDate = DateTime.UtcNow,
        };
        await userManager.CreateAsync(user, appUser.PassWord);
        await userManager.AddToRoleAsync(user, nameof(Roles.Student));
        var student = new Student
        {
            AppUserId = user.Id,
            StudentAcademicInfo = new StudentAcademicInfo()
        };
        await studentRepository.CreateAsync(student);
        var token = await GenerateJwtToken(user);
        return new AuthResponse(token, ExpireTime: DateTime.UtcNow.AddDays(7), true, null);
    }

    public Task<bool> DeleteAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<AppUserDto?> GetById(string id,
        Func<IQueryable<AppUser>, IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<AppUserDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null)
    {
        throw new NotImplementedException();
    }

    public async Task<GetMeAppUserResponse> GetMe(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new GetMeAppUserResponse(null, ResponseMessages.Unauthorized, true, (int)StatusCode.Unauthorized);
        }

        var students =
            await studentRepository.FindAsync(s => s.AppUserId == userId,
                s => s
                    .Include(m => m.StudentAcademicInfo)
                    .ThenInclude(a => a.AdmissionYear)
                    .Include(m => m.StudentAcademicInfo)
                    .ThenInclude(a => a.Faculty)
                    .Include(m => m.StudentAcademicInfo)
                    .ThenInclude(m => m.Specialization),
                tracking: false);
        var student = students.FirstOrDefault();
        if (student is null)
        {
            return new GetMeAppUserResponse(null, ResponseMessages.NotFound, true, (int)StatusCode.NotFound);
        }


        var dto = new UserProfileDto(student.Id, user.Name, user.Surname, student.StudentAcademicInfo.Gpa,
            student.StudentAcademicInfo.AdmissionYear.FirstYear, student.StudentAcademicInfo.Faculty.Name,
            student.StudentAcademicInfo.Specialization.Name, user.Email!, user.BornDate);

        return new GetMeAppUserResponse(dto, ResponseMessages.Success, true, (int)StatusCode.Ok);
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