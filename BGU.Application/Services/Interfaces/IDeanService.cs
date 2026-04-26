using BGU.Application.Contracts.Dean;

namespace BGU.Application.Services.Interfaces;

public interface IDeanService
{
    Task<DeanProfileResponse> GetProfile(string userId);
}