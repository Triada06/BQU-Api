using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;

namespace BGU.Application.Services.Interfaces;

public interface IUserService
{
    public Task<AuthResponse> SignInAsync(AppUserSignInDto appUserDto);
}