using BGU.Application.Contracts.Syllabus.Requests;
using BGU.Application.Contracts.Syllabus.Responses;
using BGU.Application.Services.Interfaces;

namespace BGU.Application.Services;

public class SyllabusService : ISyllabusService
{
    public Task<CreateSyllabusResponse> CreateAsync(CreateSyllabusRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<UpdateSyllabusResponse> UpdateAsync(UpdateSyllabusRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<DeleteSyllabusResponse> DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<GetByIdSyllabusResponse> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }
}