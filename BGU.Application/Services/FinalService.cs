using System.Linq.Expressions;
using BGU.Application.Common;
using BGU.Application.Dtos.Exams;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class FinalService(
    IFinalRepository finalRepository,
    ITeacherRepository teacherRepository,
    UserManager<AppUser> userManager,
    IStudentRepository studentRepository,
    ITransactionService transactionService,
    IStudentSubjectResultRepository studentSubjectResultRepository) : IFinalService
{
    public async Task<ApiResult<PagedResponse<GetFinalDto>>> GetAllAsync(int page, int pageSize, string? search,
        string? groupId)
    {
        var cleanSearch = search?.ToLower().Trim();

        var data = await finalRepository.GetAllPaginatedAsync(
            cleanSearch is not null
                ? x => (x.Student.AppUser.Name.ToLower().Trim().Contains(cleanSearch) ||
                        x.Student.AppUser.Surname.ToLower().Trim().Contains(cleanSearch) ||
                        x.Student.AppUser.MiddleName.ToLower().Trim().Contains(cleanSearch) ||
                        x.TaughtSubject.Code.ToLower().Trim().Contains(cleanSearch) ||
                        x.TaughtSubject.Subject.Name.ToLower().Trim().Contains(cleanSearch))
                : null,
            page, pageSize, false,
            include: x => x
                .Include(g => g.TaughtSubject)
                .ThenInclude(ts => ts.Group)
                .Include(e => e.Student)
                .ThenInclude(st => st.AppUser),
            filterBy: groupId is not null ? x => x.TaughtSubject.GroupId == groupId : null);

        var returnData = data.Items.Select(x =>
            new GetFinalDto(x.Id, x.TaughtSubject.Group.Code, x.StudentId, x.Student.AppUser.Name,
                x.TaughtSubject.Code,
                x.IsConfirmed, x.Date?.ToString("yyyy MMMM dd"), x.Grade, x.IsAllowed)).ToList();

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

    public async Task<ApiResult<IEnumerable<GetFinalDto>>> GetAllToConfirmAsync()
    {
        var data = await finalRepository.FindAsync(x => x.IsAllowed && x.Grade != -1 && !x.IsConfirmed, tracking: false,
            include: x => x
            .Include(g => g.TaughtSubject)
            .ThenInclude(ts => ts.Group)
            .Include(e => e.Student)
            .ThenInclude(st => st.AppUser));
        
        if (data.Count == 0)
        {
            return ApiResult<IEnumerable<GetFinalDto>>.Success([]);
        }

        var returnData = data.Select(x =>
            new GetFinalDto(x.Id, x.TaughtSubject.Group.Code, x.StudentId, x.Student.AppUser.Name,
                x.TaughtSubject.Code,
                x.IsConfirmed, x.Date?.ToString("yyyy MMMM dd"), x.Grade, x.IsAllowed)).ToList();

        return ApiResult<IEnumerable<GetFinalDto>>.Success(returnData);
    }

    public async Task<ApiResult> SetExamDateAsync(SetExamDto setExamDto)
    {
        var exam = await finalRepository.GetByIdAsync(setExamDto.Id, tracking: true);
        if (exam is null)
        {
            return ApiResult.NotFound();
        }

        if (!exam.IsAllowed)
        {
            return ApiResult.BadRequest("This student is not allowed to take an exam");
        }

        exam.Date = setExamDto.Date;
        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult.SystemError("Failed to update exam");
        }

        return ApiResult.Success();
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
        return await transactionService.ExecuteAsync(async () =>
        {
        var exam = await finalRepository.GetByIdAsync(request.Id, tracking: true);
        if (exam is null)
        {
            return ApiResult<UpdateExamResponse>.NotFound($"An exam with an Id of {request.Id} was not found");
        }

        var oldGrade = exam.Grade;
        exam.Date = request.Dto.Date;
        exam.Grade = request.Dto.Grade;
        exam.TaughtSubjectId = request.Dto.TaughtSubjectId;
        exam.StudentId = request.Dto.StudentId;
        exam.IsAllowed = request.Dto.IsAllowed;

        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult<UpdateExamResponse>.SystemError();
        }


        var results = await studentSubjectResultRepository.FindAsync(x =>
            x.StudentId == exam.StudentId && x.TaughtSubjectId == exam.TaughtSubjectId);

        if (results.Count == 0)
        {
            // this code should never be executed since by this time the object already must be created
            var result = new StudentSubjectResult
            {
                StudentId = exam.StudentId,
                TaughtSubjectId = exam.TaughtSubjectId,
                ExamGrade = exam.Grade,
                IsFinalized = exam.IsConfirmed
            };

            result.UpdateFinalGrade();

            if (!await studentSubjectResultRepository.CreateAsync(result))
            {
                return ApiResult<UpdateExamResponse>.SystemError("Failed to create student result");
            }
        }
        else
        {
            var result = results[0];
            result.ExamGrade = exam.Grade;
            result.IsFinalized = exam.IsConfirmed;

            result.UpdateFinalGrade();

            if (!await studentSubjectResultRepository.UpdateAsync(result))
            {
                return ApiResult<UpdateExamResponse>.SystemError("Failed to update student result");
            }
        }

        return ApiResult<UpdateExamResponse>.Success(new UpdateExamResponse(exam.Id, exam.StudentId,
            exam.TaughtSubjectId, exam.Date, exam.Grade, exam.IsAllowed));
        }, response => response.IsSucceeded && response.StatusCode == 200);
    }

    public async Task<ApiResult<ExamsToGrade>> GetAllByTeachAsync(string userId, bool forGrade)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ApiResult<ExamsToGrade>.NotFound($"User with an Id of {userId} not found");
        }

        var teachers = await teacherRepository.FindAsync(x => x.AppUserId == userId, tracking: false);

        if (teachers.Count == 0)
        {
            return ApiResult<ExamsToGrade>.NotFound($"Teacher not found");
        }

        List<Exam> exams;

        if (forGrade)
        {
            exams = await finalRepository.FindAsync(
                x => x.Date != null && x.TaughtSubject.TeacherId == teachers[0].Id && x.Grade == -1,
                x => x
                    .Include(e => e.TaughtSubject)
                    .ThenInclude(e => e.Subject)
                    .Include(e => e.Student)
                    .ThenInclude(st => st.AppUser)
                    .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Group),
                tracking: false);
        }
        else
        {
            exams = await finalRepository.FindAsync(
                x => x.Date != null && x.TaughtSubject.TeacherId == teachers[0].Id,
                x => x
                    .Include(e => e.TaughtSubject)
                    .ThenInclude(e => e.Subject)
                    .Include(e => e.Student)
                    .ThenInclude(st => st.AppUser)
                    .Include(e => e.TaughtSubject).ThenInclude(ts => ts.Group),
                tracking: false);
        }


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
            .Select(x => new ExamToGrade(x.Id, x.Student.Id, $"{x.Student.AppUser.Name} {x.Student.AppUser.Surname}",
                x.TaughtSubjectId,
                x.TaughtSubject.Subject.Name, x.TaughtSubject.Code,
                x.TaughtSubject.GroupId, x.TaughtSubject.Group.Code, x.Grade,
                x.Date?.ToString("yyyy MMMM dd") ?? "Data was not set"));

        return ApiResult<ExamsToGrade>.Success(new ExamsToGrade(returnData));
    }

    public async Task<ApiResult<string>> GradeAsync(string userId, string finalId, int grade)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return ApiResult<string>.NotFound($"User with an Id of {userId} not found");
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

    public async Task<ApiResult> ConfirmAsync(string finalId)
    {
        return await transactionService.ExecuteAsync(async () =>
        {
        var exam = await finalRepository.GetByIdAsync(finalId, tracking: true);
        if (exam is null)
        {
            return ApiResult.NotFound($"Exam with an Id of {finalId} not found");
        }

        exam.IsConfirmed = true;

        if (!await finalRepository.UpdateAsync(exam))
        {
            return ApiResult.SystemError();
        }

        var results = await studentSubjectResultRepository.FindAsync(
            x => x.StudentId == exam.StudentId && x.TaughtSubjectId == exam.TaughtSubjectId, tracking: true);

        if (results.Count == 0)
        {
            return ApiResult.Success("Exam was confirmed but it's grade wasn't finalized since no matched data found");
        }

        var result = results[0];

        result.ExamGrade = exam.Grade;
        result.IsFinalized = true;
        result.UpdateFinalGrade();

        if (!await studentSubjectResultRepository.UpdateAsync(result))
        {
            return ApiResult.Success("Exam was confirmed but it's grade wasn't finalized");
        }

        return ApiResult.Success();
        }, response => response.IsSucceeded && response.StatusCode == 200);
    }

    public async Task<ApiResult> BulkConfirmAsync(BulkConfirmFinalsRequest? request)
    {
        if (request?.Ids is null)
        {
            return ApiResult.BadRequest("At least one exam id is required");
        }

        var ids = request.Ids
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.Ordinal)
            .ToList();

        if (ids.Count == 0)
        {
            return ApiResult.BadRequest("At least one exam id is required");
        }

        return await transactionService.ExecuteAsync(async () =>
        {
            var exams = await finalRepository.FindAsync(x => ids.Contains(x.Id), tracking: true);
            if (exams.Count != ids.Count)
            {
                var foundIds = exams.Select(x => x.Id).ToHashSet(StringComparer.Ordinal);
                var missingIds = ids.Where(x => !foundIds.Contains(x));

                return ApiResult.BadRequest($"Exams not found: {string.Join(", ", missingIds)}");
            }

            var notFinalizedCount = 0;

            foreach (var exam in exams)
            {
                exam.IsConfirmed = true;

                if (!await finalRepository.UpdateAsync(exam))
                {
                    return ApiResult.SystemError($"Failed to confirm exam with an Id of {exam.Id}");
                }

                var results = await studentSubjectResultRepository.FindAsync(
                    x => x.StudentId == exam.StudentId && x.TaughtSubjectId == exam.TaughtSubjectId,
                    tracking: true);

                if (results.Count == 0)
                {
                    notFinalizedCount++;
                    continue;
                }

                var result = results[0];
                result.ExamGrade = exam.Grade;
                result.IsFinalized = true;
                result.UpdateFinalGrade();

                if (!await studentSubjectResultRepository.UpdateAsync(result))
                {
                    return ApiResult.SystemError(
                        $"Failed to finalize student result for exam with an Id of {exam.Id}");
                }
            }

            return notFinalizedCount == 0
                ? ApiResult.Success($"Confirmed {exams.Count} exams")
                : ApiResult.Success(
                    $"Confirmed {exams.Count} exams, but {notFinalizedCount} grade results were not finalized because no matched data was found");
        }, response => response.IsSucceeded && response.StatusCode == 200);
    }

    public async Task<ApiResult> SetGroupExamDateAsync(SetGroupExamDto setExamDto)
    {
        return await transactionService.ExecuteAsync(async () =>
        {
        var students = await studentRepository.GetAllAsync(x => x.GroupId == setExamDto.GroupId,
            include: x => x.Include(st => st.Finals), tracking: true);

        if (students.Count == 0)
        {
            return ApiResult.NotFound($"Students in group not found");
        }

        var exams = students
            .SelectMany(x => x.Finals)
            .Where(x => x.TaughtSubjectId == setExamDto.TaughtSubjectId)
            .ToList();

        if (exams.Count == 0)
        {
            return ApiResult.NotFound("Exams do not exist");
        }

        foreach (var exam in exams.Where(exam => exam.IsAllowed))
        {
            exam.Date = setExamDto.Date;
        }


        if (await finalRepository.BulkUpdateAsync(exams) <= 0)
        {
            return ApiResult.SystemError("Failed to update exam");
        }

        return ApiResult.Success();
        }, response => response.IsSucceeded && response.StatusCode == 200);
    }
}
