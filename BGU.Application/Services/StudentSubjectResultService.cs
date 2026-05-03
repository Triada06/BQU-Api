using BGU.Application.Common;
using BGU.Application.Dtos.StudentSubjectResult;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Application.Services;

public class StudentSubjectResultService(IStudentSubjectResultRepository studentSubjectResultRepository)
    : IStudentSubjectResultService
{
    public async Task<bool> CreateAsync(SubjectResultCreateDto dto)
    {
        var isExist = await studentSubjectResultRepository.AnyAsync(x =>
            x.StudentId == dto.StudentId && x.TaughtSubjectId == dto.TaughtSubjectId);

        if (isExist)
        {
            return false;
        }

        var studentSubjectResult = new StudentSubjectResult
        {
            StudentId = dto.StudentId,
            TaughtSubjectId = dto.TaughtSubjectId,
            // FinalGrade = dto.Grade,
            IsFinalized = dto.IsFinalised
        };

        var res = await studentSubjectResultRepository.CreateAsync(studentSubjectResult);
        return res;
    }

    public async Task<ApiResult<StudentSubjectResult>> UpdateAsync(string subjectResultId, SubjectResultUpdateDto dto)
    {
        var studentResult = await studentSubjectResultRepository.GetByIdAsync(subjectResultId, tracking: true);
        if (studentResult is null)
        {
            return ApiResult<StudentSubjectResult>.BadRequest();
        }

        studentResult.StudentId = dto.StudentId;
        studentResult.TaughtSubjectId = dto.TaughtSubjectId;
        // studentResult.FinalGrade = dto.Grade;
        studentResult.IsFinalized = dto.IsFinalised;

        var res = await studentSubjectResultRepository.UpdateAsync(studentResult);

        return new ApiResult<StudentSubjectResult>
        {
            Data = res ? studentResult : null,
            Message = res ? "Success" : "Failed",
            IsSucceeded = res,
            StatusCode = res ? 200 : 500
        };
    }
}