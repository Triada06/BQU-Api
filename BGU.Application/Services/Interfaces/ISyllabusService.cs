using BGU.Application.Contracts.Syllabus.Requests;
using BGU.Application.Contracts.Syllabus.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ISyllabusService
{
    Task<CreateSyllabusResponse> CreateAsync(CreateSyllabusRequest request);
    Task<UpdateSyllabusResponse> UpdateAsync(UpdateSyllabusRequest request);
    Task<DeleteSyllabusResponse> DeleteAsync(string id);
    Task<GetByIdSyllabusResponse> GetByIdAsync(string id);
}