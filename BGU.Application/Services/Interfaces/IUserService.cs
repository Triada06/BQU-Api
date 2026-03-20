using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;

namespace BGU.Application.Services.Interfaces;

public interface IUserService
{
    public Task<AuthResponse> SignInAsync(AppUserSignInDto appUserDto);
    public Task<AuthResponse> ResetPasswordAsync(string userId, string newPassword, CancellationToken cp);
    public Task<bool> CheckPasswordAsync(string userId, string password, CancellationToken cp);
}