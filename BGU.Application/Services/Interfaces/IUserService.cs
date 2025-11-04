using BGU.Application.Contracts.User;
using BGU.Application.Dtos.AppUser;
using BGU.Core.Entities;
using Microsoft.EntityFrameworkCore.Query;

namespace BGU.Application.Services.Interfaces;

public interface IUserService
{
    public Task<string> SignInAsync(AppUserSignInDto appUserDto);
    public Task<AuthResponse> SignUpAsync(AppUserSignUpDto appUser);

    public Task<bool> DeleteAsync(string userId);

    public Task<AppUserDto?> GetById(string id, Func<IQueryable<AppUser>,
        IIncludableQueryable<AppUser, object>>? include = null, bool tracking = true);

    public Task<IEnumerable<AppUserDto>> GetAll(int page, string? sort, int pageSize = 5, bool tracking = true,
        string? searchText = null);

    Task<GetMeAppUserResponse> GetMe(string userId);
}