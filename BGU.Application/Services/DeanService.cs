using BGU.Application.Contracts.Dean;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class DeanService(IDeanRepository deanRepository) : IDeanService
{
    public async Task<DeanProfileResponse> GetProfile(string userId)
    {
        throw new NotImplementedException();
    }
}