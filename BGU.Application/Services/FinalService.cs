using BGU.Application.Common;
using BGU.Application.Dtos.Exams;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class FinalService(
    IFinalRepository finalRepository,
    ITeacherRepository teacherRepository,
    UserManager<AppUser> userManager) : IFinalService
{
    public async Task<ApiResult<PagedResponse<GetFinalDto>>> GetAllAsync(int page, int pageSize, string? search)
    {
        var data = await finalRepository.GetAllAsync(
            search is not null
                ? x =>
                    search.ToLower().Trim().Contains(x.Student.AppUser.Name.ToLower().Trim(),
                        StringComparison.CurrentCultureIgnoreCase) ||
                    search.ToLower().Trim().Contains(x.Student.AppUser.Surname.ToLower().Trim(),
                        StringComparison.CurrentCultureIgnoreCase) ||
                    search.ToLower().Trim().Contains(x.Student.AppUser.MiddleName.ToLower().Trim(),
                        StringComparison.CurrentCultureIgnoreCase) ||
                    search.ToLower().Trim().Contains(x.TaughtSubject.Code.ToLower().Trim(),
                        StringComparison.CurrentCultureIgnoreCase) ||
                    search.ToLower().Trim().Contains(x.TaughtSubject.Subject.Name.ToLower().Trim(),
                        StringComparison.CurrentCultureIgnoreCase)
                : null,
            page, pageSize, false,
            include: x => x
                .Include(g => g.TaughtSubject)
                .ThenInclude(ts => ts.Group)
                .Include(e => e.Student)
                .ThenInclude(st => st.AppUser));

        var returnData = data.Items.Select(x =>
            new GetFinalDto(x.Id, x.TaughtSubject.Group.Code, x.StudentId, x.Student.AppUser.Name,
                x.TaughtSubject.Code,
                x.IsConfirmed, x.Date?.ToString("dddd, MMM dd"), x.Grade, x.IsAllowed)).ToList();

        return new ApiResult<PagedResponse<GetFinalDto>>
        {
            Data = new PagedResponse<GetFinalDto>
            {
                Items = returnData,
                Page = data.Page,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount
            },
            Message = ResponseMessages.Success,
            IsSucceeded = true,
            StatusCode = 200
        };
    }

    public async Task<ApiResult<bool>> SetExamDateAsync(SetExamDto setExamDto)
    {
        var exam = await finalRepository.GetByIdAsync(setExamDto.Id, tracking: true);
        if (exam is null)
        {
            return ApiResult<bool>.NotFound();
        }

        if (!exam.IsAllowed)
        {
            return ApiResult<bool>.BadRequest(false, "This student is not allowed to take an exam");
        }

        exam.Date = setExamDto.Date;
        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult<bool>.SystemError("Failed to update exam");
        }

        return ApiResult<bool>.Success(true);
    }

    public async Task<ApiResult<string>> CreateAsync(CreateExamDto createExamDto)
    {
        if (await finalRepository.AnyAsync(x =>
                x.StudentId == createExamDto.StudentId && x.TaughtSubjectId == createExamDto.SubjectId))
        {
            return ApiResult<string>.BadRequest("An exam with provided student and subject already exists");
        }

        var exam = new Exam
        {
            Date = createExamDto.Date,
            IsConfirmed = false,
            StudentId = createExamDto.StudentId,
            TaughtSubjectId = createExamDto.SubjectId,
        };

        if (!await finalRepository.CreateAsync(exam))
        {
            return ApiResult<string>.SystemError("Failed to create exam");
        }

        return ApiResult<string>.Success(exam.Id);
    }

    public async Task<ApiResult<UpdateExamResponse>> UpdateAsync(UpdateExamRequest request)
    {
        var exam = await finalRepository.GetByIdAsync(request.Id, tracking: true);
        if (exam is null)
        {
            return ApiResult<UpdateExamResponse>.NotFound($"An exam with an Id of {request.Id} was not found");
        }

        exam.Date = request.Dto.Date;
        exam.Grade = request.Dto.Grade;
        exam.TaughtSubjectId = request.Dto.TaughtSubjectId;
        exam.StudentId = request.Dto.StudentId;
        exam.IsAllowed = request.Dto.IsAllowed;

        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult<UpdateExamResponse>.SystemError();
        }

        return ApiResult<UpdateExamResponse>.Success(new UpdateExamResponse(exam.Id, exam.StudentId,
            exam.TaughtSubjectId, exam.Date, exam.Grade, exam.IsAllowed));
    }

    public async Task<ApiResult<ExamsToGrade>> GetAllByTeachAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ApiResult<ExamsToGrade>.NotFound($"User with an Id of {userId} not found");
        }

        var teacher = (await teacherRepository.FindAsync(x => x.AppUserId == userId, tracking: false)).First();

        if (teacher is null)
        {
            return ApiResult<ExamsToGrade>.NotFound($"Teacher not found");
        }

        var exams = await finalRepository.FindAsync(
            x => x.Date != null && x.TaughtSubject.TeacherId == teacher.Id,
            x => x
                .Include(e => e.TaughtSubject)
                .ThenInclude(e => e.Subject)
                .Include(e => e.Student)
                .ThenInclude(st => st.AppUser)
                .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Group),
            tracking: false);

        if (exams.Count == 0)
        {
            return new ApiResult<ExamsToGrade>
            {
                Data = new ExamsToGrade([]),
                Message = "Success",
                IsSucceeded = true,
                StatusCode = 200
            };
        }

        var returnData = exams
            .Select(x => new ExamToGrade(x!.Id, x.Student.Id, $"{x.Student.AppUser.Name} {x.Student.AppUser.Surname}",
                x.TaughtSubjectId,
                x.TaughtSubject.Subject.Name, x.TaughtSubject.Code,
                x.TaughtSubject.GroupId, x.TaughtSubject.Group.Code, x.Grade,
                x.Date?.ToString("dddd, MMM dd")));

        return ApiResult<ExamsToGrade>.Success(new ExamsToGrade(returnData));
    }

    public async Task<ApiResult<string>> GradeAsync(string userId, string finalId, int grade)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ApiResult<string>.NotFound($"User with an Id of {userId} not found");
        }

        var teacher = (await teacherRepository.FindAsync(x => x.AppUserId == userId, tracking: false)).First();

        if (teacher is null)
        {
            return ApiResult<string>.NotFound($"Teacher not found");
        }

        var exam = await finalRepository.GetByIdAsync(finalId, tracking: false);
        if (exam is null)
        {
            return ApiResult<string>.NotFound($"Exam with an Id of {finalId} not found");
        }

        exam.Grade = grade;

        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult<string>.SystemError();
        }

        return ApiResult<string>.Success(exam.Id);
    }

    public async Task<ApiResult<bool>> ConfirmAsync(string finalId)
    {
        var exam = await finalRepository.GetByIdAsync(finalId, tracking: false);
        if (exam is null)
        {
            return ApiResult<bool>.NotFound($"Exam with an Id of {finalId} not found");
        }

        exam.IsConfirmed = true;

        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult<bool>.SystemError();
        }

        return ApiResult<bool>.Success(true);
    }
}