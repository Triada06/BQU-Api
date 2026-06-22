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
    IStudentSubjectResultRepository studentSubjectResultRepository,
    AppDbContext context) : IFinalService
{
   public async Task<ApiResult<PagedResponse<GetFinalDto>>> GetAllAsync(int page, int pageSize, string? search,
    string? groupId)
{
    var cleanSearch = search?.ToLower().Trim();

    var data = await finalRepository.GetAllPaginatedAsync(
        cleanSearch is not null
            ? x => ((x.Student.AppUser.Name.ToLower().Trim().Contains(cleanSearch) ||
                     x.Student.AppUser.Surname.ToLower().Trim().Contains(cleanSearch) ||
                     x.Student.AppUser.MiddleName.ToLower().Trim().Contains(cleanSearch) ||
                     x.TaughtSubject.Code.ToLower().Trim().Contains(cleanSearch) ||
                     x.TaughtSubject.Subject.Name.ToLower().Trim().Contains(cleanSearch)) && x.IsActual)
            : x => x.IsActual,
        page, pageSize, false,
        include: x => x
            .Include(g => g.TaughtSubject)
            .ThenInclude(ts => ts.Group)
            .Include(e => e.Student)
            .ThenInclude(st => st.AppUser)
            .Include(g => g.TaughtSubject)
            .ThenInclude(ts => ts.Subject),
        filterBy: groupId is not null ? x => x.TaughtSubject.GroupId == groupId : null);

    var taughtSubjectIds = data.Items.Select(x => x.TaughtSubjectId).Distinct().ToList();
    var studentIds = data.Items.Select(x => x.StudentId).Distinct().ToList();

    var gradesBeforeExamMap = await context.StudentSubjectResults.AsNoTracking()
        .Where(x => taughtSubjectIds.Contains(x.TaughtSubjectId) && studentIds.Contains(x.StudentId))
        .Select(x => new { x.TaughtSubjectId, x.StudentId, x.GradeBeforeExam })
        .ToListAsync();

    var gradeLookup = gradesBeforeExamMap
        .ToDictionary(x => (x.TaughtSubjectId, x.StudentId), x => x.GradeBeforeExam);

    var returnData = data.Items.Select(x =>
        new GetFinalDto(x.Id, x.TaughtSubject.Group.Code, x.StudentId,
            x.Student.AppUser.Name + " " + x.Student.AppUser.Surname,
            x.TaughtSubject.Subject.Name,
            x.IsConfirmed, x.Date?.ToString("yyyy MMMM dd"), x.Grade,
            gradeLookup.GetValueOrDefault((x.TaughtSubjectId, x.StudentId)),
            x.IsAllowed)).ToList();

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
    public async Task<ApiResult<PagedResponse<FailedFinalExamDto>>> GetAllFailedAsync(int page, int pageSize, string? search,
        string? groupId)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var failedResults = context.StudentSubjectResults
            .AsNoTracking()
            .Where(result =>
                result.IsFinalized &&
                (result.GradeBeforeExam < 34 || result.ExamGrade < 17) &&
                context.Exams.Any(exam =>
                    exam.StudentId == result.StudentId &&
                    exam.TaughtSubjectId == result.TaughtSubjectId &&
                    exam.IsActual));

        var cleanGroupId = groupId?.Trim();
        if (!string.IsNullOrWhiteSpace(cleanGroupId))
        {
            failedResults = failedResults.Where(result => result.TaughtSubject.GroupId == cleanGroupId);
        }

        var cleanSearch = search?.Trim().ToLower();
        if (!string.IsNullOrWhiteSpace(cleanSearch))
        {
            failedResults = failedResults.Where(result =>
                result.Student.AppUser.Name.ToLower().Contains(cleanSearch) ||
                result.Student.AppUser.Surname.ToLower().Contains(cleanSearch) ||
                (result.Student.AppUser.MiddleName ?? string.Empty).ToLower().Contains(cleanSearch) ||
                (result.Student.AppUser.UserName ?? string.Empty).ToLower().Contains(cleanSearch) ||
                result.TaughtSubject.Code.ToLower().Contains(cleanSearch) ||
                result.TaughtSubject.Subject.Name.ToLower().Contains(cleanSearch) ||
                result.TaughtSubject.Group.Code.ToLower().Contains(cleanSearch));
        }

        var totalCount = await failedResults.CountAsync();

        var items = await failedResults
            .OrderBy(result => result.TaughtSubject.Group.Code)
            .ThenBy(result => result.Student.AppUser.Surname)
            .ThenBy(result => result.Student.AppUser.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(result => new FailedFinalExamDto(
                context.Exams
                    .Where(exam =>
                        exam.StudentId == result.StudentId &&
                        exam.TaughtSubjectId == result.TaughtSubjectId &&
                        exam.IsActual)
                    .OrderByDescending(exam => exam.CreatedAt)
                    .Select(exam => exam.Id)
                    .FirstOrDefault() ?? string.Empty,
                result.StudentId,
                (result.Student.AppUser.Name + " " + result.Student.AppUser.Surname + " " +
                 (result.Student.AppUser.MiddleName ?? string.Empty)).Trim(),
                result.TaughtSubject.GroupId,
                result.TaughtSubject.Group.Code,
                result.TaughtSubjectId,
                result.TaughtSubject.SubjectId,
                result.TaughtSubject.Code,
                result.TaughtSubject.Subject.Name,
                result.GradeBeforeExam,
                result.ExamGrade,
                result.FinalGrade,
                context.Exams
                    .Where(exam =>
                        exam.StudentId == result.StudentId &&
                        exam.TaughtSubjectId == result.TaughtSubjectId &&
                        exam.IsActual)
                    .OrderByDescending(exam => exam.CreatedAt)
                    .Select(exam => exam.Date)
                    .FirstOrDefault(),
                context.Exams
                    .Where(exam =>
                        exam.StudentId == result.StudentId &&
                        exam.TaughtSubjectId == result.TaughtSubjectId &&
                        exam.IsActual)
                    .OrderByDescending(exam => exam.CreatedAt)
                    .Select(exam => (int?)exam.Grade)
                    .FirstOrDefault()))
            .ToListAsync();

        return ApiResult<PagedResponse<FailedFinalExamDto>>.Success(new PagedResponse<FailedFinalExamDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        });
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
                x.IsConfirmed, x.Date?.ToString("yyyy MMMM dd"),x.Grade, 0, x.IsAllowed)).ToList();
        // todo:  this shit needs to be fixed immediatly, why tf do we return 0 as a grade before exam if we dont even need it 
        
        
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
        return await transactionService.ExecuteAsync(async () =>
        {
            var oldExams = await finalRepository.FindAsync(x =>
                x.StudentId == createExamDto.StudentId && x.TaughtSubjectId == createExamDto.SubjectId, tracking: true);


            // this is if the student failed an exam and a new one is being created 
            if (oldExams.Count != 0)
            {
                var studentExamResults = await studentSubjectResultRepository.FindAsync(x =>
                    x.StudentId == createExamDto.StudentId && x.TaughtSubjectId == createExamDto.SubjectId);

                if (studentExamResults.Count == 0)
                {
                    return ApiResult<string>.BadRequest(
                        "An exam with provided student and subject already exists and a history record not found");
                }

                var result = studentExamResults[0];
                if (result is { GradeBeforeExam: >= 34, ExamGrade: >= 17 })
                {
                    return ApiResult<string>.BadRequest("An exam with provided student and subject already exists");
                }

                var newestExam = oldExams.OrderByDescending(x => x.CreatedAt).First();
                newestExam.IsActual = false;

                var updateRes = await finalRepository.UpdateAsync(newestExam);
                if (!updateRes)
                {
                    return ApiResult<string>.SystemError("Error while updating the old exam");
                }

                result.IsFinalized = false;
                var updateResultResponse = await studentSubjectResultRepository.UpdateAsync(result);

                if (!updateResultResponse)
                {
                    return ApiResult<string>.SystemError("Error while updating the old exam");
                }
            }

            var exam = new Exam
            {
                Date = createExamDto.Date,
                IsConfirmed = false,
                StudentId = createExamDto.StudentId,
                TaughtSubjectId = createExamDto.SubjectId,
                IsAllowed = true,
                Grade = -1
            };

            if (!await finalRepository.CreateAsync(exam))
            {
                return ApiResult<string>.SystemError("Failed to create exam");
            }

            return ApiResult<string>.Success(exam.Id);
        }, response => response.IsSucceeded && response.StatusCode == 200);
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
                    ExamGrade = exam.Grade is -1 ? 0 : exam.Grade,
                    IsFinalized = exam.IsConfirmed
                };

                result.UpdateFinalStats();

                if (!await studentSubjectResultRepository.CreateAsync(result))
                {
                    return ApiResult<UpdateExamResponse>.SystemError("Failed to create student result");
                }
            }
            else
            {
                var result = results[0];
                result.ExamGrade = exam.Grade is -1 ? 0 : exam.Grade;
                result.IsFinalized = exam.IsConfirmed;

                result.UpdateFinalStats();

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
                return ApiResult.Success(
                    "Exam was confirmed but it's grade wasn't finalized since no matched data found");
            }

            var result = results[0];

            result.ExamGrade = exam.Grade;
            result.IsFinalized = true;
            result.UpdateFinalStats();

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
                result.UpdateFinalStats();

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
