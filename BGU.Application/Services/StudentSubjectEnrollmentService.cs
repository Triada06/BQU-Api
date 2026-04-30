using BGU.Application.Common;
using BGU.Application.Common.HelperServices.Interfaces;
using BGU.Application.Dtos.StudentEnrollment;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class StudentSubjectEnrollmentService(
    IStudentSubjectEnrollmentRepository repo,
    IAcademicHelper academicHelper,
    ITaughtSubjectRepository taughtSubjectRepository,
    IStudentRepository studentRepository)
    : IStudentSubjectEnrollmentService
{
    public async Task<ApiResult<string>> CreateAsync(CreateStudentSubjectEnrollmentDto dto)
    {
        var taughtSubject =
            await taughtSubjectRepository.GetByIdAsync(dto.TaughtSubjectId, include: i => i.Include(x => x.Classes),
                tracking:
                true);

        if (taughtSubject is null)
        {
            return ApiResult<string>.BadRequest($"Taught subject with an Id of {dto.TaughtSubjectId} not found");
        }

        var student = await studentRepository.GetByIdAsync(dto.StudentId, tracking: true);

        if (student is null)
        {
            return ApiResult<string>.BadRequest($"Student with an Id of {dto.StudentId} not found");
        }


        var academicStuffResponse =
            await academicHelper.CreateAcademicRequirementsAsync(taughtSubject.Classes.ToList(), student,
                dto.TaughtSubjectId);

        if (!academicStuffResponse)
        {
            return ApiResult<string>.SystemError("An error occured while creating academic requirements");
        }

        var attempt = dto.Attempt ?? 1;

        var entity = new StudentSubjectEnrollment
        {
            StudentId = dto.StudentId,
            TaughtSubjectId = dto.TaughtSubjectId,
            Attempt = attempt
        };

        var res = await repo.CreateAsync(entity);

        return res
            ? ApiResult<string>.Success(entity.Id)
            : ApiResult<string>.SystemError("An error occured while deleting the enrollment");
    }

    public async Task<ApiResult<PagedResponse<GetEnrollmentDto>>> GetAllAsync(int page, int pageSize)
    {
        var data = await repo.GetAllPaginatedAsync(null, page, pageSize, include: x =>
            x.Include(e => e.Student).ThenInclude(st => st.AppUser)
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Subject)
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Group));

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
                item.TaughtSubject.Subject.Name, item.TaughtSubjectId, item.TaughtSubject.Code,
                item.TaughtSubject.Group.Code);
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
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Subject)
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Group));

        if (enrollment == null) return ApiResult<GetEnrollmentDto>.BadRequest("Student enrollment not found");

        var studentFullName = enrollment.Student.AppUser.Name + " " +
                              enrollment.Student.AppUser.Surname + " " +
                              enrollment.Student.AppUser.MiddleName;

        var dto = new GetEnrollmentDto(enrollment.Id, enrollment.StudentId, studentFullName,
            enrollment.TaughtSubject.Subject.Name, enrollment.TaughtSubjectId, enrollment.TaughtSubject.Code,
            enrollment.TaughtSubject.Group.Code);


        return ApiResult<GetEnrollmentDto>.Success(dto);
    }

    public async Task<ApiResult> UpdateAsync(string id, UpdateStudentSubjectEnrollmentDto dto)
    {
        var enrollment = await repo.GetByIdAsync(id, tracking: true);

        if (enrollment == null) return ApiResult.BadRequest("Student enrollment not found");

        enrollment.Attempt = dto.Attempt;

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