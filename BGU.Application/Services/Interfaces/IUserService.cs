using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using Microsoft.AspNetCore.Identity.Data;
using ResetPasswordRequest = BGU.Application.Contracts.User.ResetPasswordRequest;

namespace BGU.Application.Services.Interfaces;

public interface IUserService
{
    public Task<AuthResponse> SignInAsync(AppUserSignInDto appUserDto);
    public Task<AuthResponse> ResetPasswordAsync(string userId, string newPassword, CancellationToken cp);
    public Task<bool> CheckPasswordAsync(string userId, string password, CancellationToken cp);
    public Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken ct);
    public Task<bool> AddEmailAsync(string userId, AddEmailRequest request, CancellationToken cp);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken ct);
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
}