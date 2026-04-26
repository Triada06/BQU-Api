using BGU.Application.Common;
using BGU.Application.Dtos.StudentSubjectResult;
using BGU.Core.Entities;

namespace BGU.Application.Services.Interfaces;

public interface IStudentSubjectResultService
{
    Task<bool> CreateAsync(SubjectResultCreateDto dto);
    Task<ApiResult<StudentSubjectResult>> UpdateAsync(string subjectResultId, SubjectResultUpdateDto dto);
}