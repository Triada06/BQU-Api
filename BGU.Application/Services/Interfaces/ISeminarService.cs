using BGU.Application.Contracts.Seminars.Requests;
using BGU.Application.Contracts.Seminars.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ISeminarService
{
    Task<CreateSeminarResponse> CreateAsync(CreateSeminarRequest seminar);
    Task<DeleteSeminarResponse> DeleteAsync(string id);
}