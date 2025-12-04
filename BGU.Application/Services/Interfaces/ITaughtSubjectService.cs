using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ITaughtSubjectService
{
    Task<DeleteTaughtSubjectResponse> DeleteAsync(string id);
    Task<UpdateTaughtSubjectResponse> UpdateAsync(string id, UpdateTaughtSubjectRequest taughtSubject);
    Task<GetAllTaughtSubjectResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
    Task<GetByIdTaughtSubjectResponse> GetByIdAsync(string id, bool tracking = false);
}