using BGU.Application.Common;
using BGU.Application.Contracts.TaughtSubjects.Requests;
using BGU.Application.Contracts.TaughtSubjects.Responses;
using BGU.Application.Dtos.Student;
using BGU.Application.Dtos.TaughtSubject;
using BGU.Application.Dtos.TaughtSubject.Requests;
using BGU.Application.Dtos.TaughtSubject.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ITaughtSubjectService
{
    Task<DeleteTaughtSubjectResponse> DeleteAsync(string id);
    Task<UpdateTaughtSubjectResponse> UpdateAsync(string id, UpdateTaughtSubjectRequest taughtSubject);
    Task<GetAllTaughtSubjectResponse> GetAllAsync(int page, int pageSize, bool tracking = false);
    Task<GetByIdTaughtSubjectResponse> GetByIdAsync(string id, bool tracking = false);
    Task<CreateTaughtSubjectResponse> CreateAsync(CreateTaughtSubjectRequest request);
    Task<ApiResult<GetActivitiesAndAttendances>> GetStudentsAndAttendances(string taughtSubjectId);
    Task<ApiResult<GetStudentsForSubject>> GetStudentsAsync(string taughtSubjectId);
    Task<ApiResult<GetIndependentWorksByTaughtSubjectDto>> GetIndependentWorksByTaughtSubjectIdAsync(string taughtSubjectId);
}