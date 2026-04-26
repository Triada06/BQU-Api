using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Contracts.ClassTime.Responses;

namespace BGU.Application.Services.Interfaces;

public interface IClassTimeService
{
    public Task<CreateClassTimeResponse> CreateAsync( CreateClassTimeRequest request);
}