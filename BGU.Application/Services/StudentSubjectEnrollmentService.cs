using BGU.Application.Common;
using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class StudentSubjectEnrollmentService(IStudentSubjectEnrollmentRepository repo)
    : IStudentSubjectEnrollmentService
{
    public async Task<ApiResult<string>> CreateAsync(CreateStudentSubjectEnrollmentDto dto)
    {
        var entity = new StudentSubjectEnrollment
        {
            StudentId = dto.StudentId,
            TaughtSubjectId = dto.TaughtSubjectId,
            Attempt = dto.Attempt ?? 1
        };

        var res = await repo.CreateAsync(entity);

        return !res
            ? ApiResult<string>.SystemError("An error occured while deleting the enrollment")
            : ApiResult<string>.Success(entity.Id);
    }

    public async Task<ApiResult<PagedResponse<GetEnrollmentDto>>> GetAllAsync(int page, int pageSize)
    {
        var data = await repo.GetAllPaginatedAsync(null, page, pageSize, include: x =>
            x.Include(e => e.Student).ThenInclude(st => st.AppUser)
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Subject));

        if (!data.Items.Any())
        {
            return new ApiResult<PagedResponse<GetEnrollmentDto>>
            {
                Data = new PagedResponse<GetEnrollmentDto>
                {
                    Items = [],
                    Page = data.Page,
                    PageSize = data.PageSize,
                    TotalCount = data.TotalCount
                },
                Message = "Success",
                IsSucceeded = true,
                StatusCode = 200
            };
        }

        List<GetEnrollmentDto> returnData = [];

        foreach (var item in data.Items)
        {
            var studentFullName = item.Student.AppUser.Name + " " +
                                  item.Student.AppUser.Surname + " " +
                                  item.Student.AppUser.MiddleName;

            var dto = new GetEnrollmentDto(item.Id, item.StudentId, studentFullName,
                item.TaughtSubject.Subject.Name, item.TaughtSubjectId, item.TaughtSubject.Code);
            returnData.Add(dto);
        }

        return new ApiResult<PagedResponse<GetEnrollmentDto>>
        {
            Data = new PagedResponse<GetEnrollmentDto>
            {
                Items = returnData,
                Page = data.Page,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount
            },

            Message = "Success",
            IsSucceeded = true,
            StatusCode = 200
        };
    }

    public async Task<ApiResult<GetEnrollmentDto>> GetByIdAsync(string id)
    {
        var enrollment = await repo.GetByIdAsync(id, x =>
                x.Include(e => e.Student).ThenInclude(st => st.AppUser)
                    .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Subject),
            tracking: true);

        if (enrollment == null) return ApiResult<GetEnrollmentDto>.BadRequest("Student enrollment not found");

        var studentFullName = enrollment.Student.AppUser.Name + " " +
                              enrollment.Student.AppUser.Surname + " " +
                              enrollment.Student.AppUser.MiddleName;

        var dto = new GetEnrollmentDto(enrollment.Id, enrollment.StudentId, studentFullName,
            enrollment.TaughtSubject.Subject.Name, enrollment.TaughtSubjectId, enrollment.TaughtSubject.Code);


        return ApiResult<GetEnrollmentDto>.Success(dto);
    }

    public async Task<ApiResult> UpdateAsync(string id, UpdateStudentSubjectEnrollmentDto dto)
    {
        var enrollment = await repo.GetByIdAsync(id, tracking: true);

        if (enrollment == null) return ApiResult.BadRequest("Student enrollment not found");

        enrollment.Attempt = dto.Attempt;
        enrollment.TaughtSubjectId = dto.TaughtSubjectId;
        enrollment.StudentId = dto.StudentId;

        var res = await repo.UpdateAsync(enrollment);

        return !res ? ApiResult.SystemError("An error occured while updating the enrollment") : ApiResult.Success();
    }

    public async Task<ApiResult> DeleteAsync(string id)
    {
        var entity = await repo.GetByIdAsync(id, tracking: true);

        if (entity == null) return ApiResult.BadRequest("Student enrollment not found");

        var res = await repo.DeleteAsync(entity);

        return !res ? ApiResult.SystemError("An error occured while deleting the enrollment") : ApiResult.Success();
    }
}