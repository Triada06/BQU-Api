using BGU.Application.Dtos.AppUser;

namespace BGU.Application.Services.Interfaces;

public interface IUserService 
{
    public Task<string> SignInAsync(AppUserSignInDto appUserDto);
}