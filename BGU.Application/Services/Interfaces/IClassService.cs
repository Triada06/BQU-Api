using BGU.Application.Contracts.Class;
using BGU.Application.Contracts.Class.Requests;
using BGU.Application.Contracts.Class.Responses;
using BGU.Application.Dtos.Class;

namespace BGU.Application.Services.Interfaces;

public interface IClassService
{
    Task<ClassGetAllResponse> GetAll(string userId);
    Task<ClassGetByIdResponse> GetByid(string userId);
    Task<CreateClassResponse> CreateAsync(CreateClassRequest request);
}