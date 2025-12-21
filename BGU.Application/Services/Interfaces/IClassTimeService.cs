using BGU.Application.Contracts.ClassTime.Requests;
using BGU.Application.Contracts.ClassTime.Responses;
using BGU.Core.Entities;
using BGU.Core.Enums;

namespace BGU.Application.Services.Interfaces;

public interface IClassTimeService
{
    public Task<CreateClassTimeResponse> CreateAsync( CreateClassTimeRequest request);
}