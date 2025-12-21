using BGU.Application.Contracts.Dean;
using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Application.Dtos.Dean;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class DeanService(IDeanRepository deanRepository, UserManager<AppUser> userManager) : IDeanService
{
    public async Task<DeanProfileResponse> GetProfile(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new DeanProfileResponse(null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var dean = (await deanRepository.FindAsync(x => x.AppUserId == user.Id,
            i => i.Include(s => s.Faculty))).FirstOrDefault();
        if (dean is null)
        {
            return new DeanProfileResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var dto = new DeanProfileDto(dean.AppUser.Name, dean.AppUser.Surname, dean.Id, dean.Faculty.Name,
            dean.AppUser.Email!, dean.PhoneNumber, dean.AppUser.Pin, dean.AppUser.BornDate);
        return new DeanProfileResponse(
            dto,
            ResponseMessages.Success,
            true,
            (int)StatusCode.Ok
        );
    }
}