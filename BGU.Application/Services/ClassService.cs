using BGU.Application.Contracts.Class;
using BGU.Application.Contracts.Class.Requests;
using BGU.Application.Contracts.Class.Responses;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BGU.Application.Services;

public class ClassService(IClassRepository classRepository, UserManager<AppUser> userManager) : IClassService
{
    public async Task<ClassGetAllResponse> GetAll(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ClassGetByIdResponse> GetByid(string id)
    {
        throw new NotImplementedException();
    }

    public Task<CreateClassResponse> CreateAsync(CreateClassRequest request)
    {
        throw new NotImplementedException();
    }
}