using BGU.Application.Contracts.Group.Requests;
using BGU.Application.Contracts.Group.Responses;
using BGU.Application.Dtos.Class;
using BGU.Application.Dtos.Group;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class GroupService(IGroupRepository groupRepository, IAdmissionYearRepository admissionYearRepository)
    : IGroupService
{
    public async Task<GetAllGroupsResponse> GetAllAsync(int page, int pageSize, bool tracking = false)
    {
        var groups =
            (await groupRepository.GetAllAsync(page, pageSize, tracking,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear)
            ))
            .Select(x => new GroupDto(
                x.Id,
                x.Code,
                x.Specialization.Name,
                x.EducationLanguage.ToString(),
                x.EducationLevel.ToString(),
                DateTime.Now.Year - x.AdmissionYear.FirstYear,
                x.Students.Count)
            );
        return new GetAllGroupsResponse(groups, StatusCode.Ok, true, ResponseMessages.Success);
    }

    public async Task<GetByIdGroupsResponse> GetByIdAsync(string id, bool tracking = false)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear), tracking
            );

        if (group is null)
        {
            return new GetByIdGroupsResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return new GetByIdGroupsResponse(
            new GroupDto(
                group.Id,
                group.Code,
                group.Specialization.Name,
                group.EducationLanguage.ToString(),
                group.EducationLevel.ToString(),
                DateTime.Now.Year - group.AdmissionYear.FirstYear,
                group.Students.Count),
            StatusCode.Ok, true,
            ResponseMessages.Success);
    }


    public async Task<GroupScheduleResponse> GetSchedule(string id)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(g => g.TaughtSubjects)
                        .ThenInclude(ts => ts.Subject)
                        .Include(st => st.TaughtSubjects)
                        .ThenInclude(ts => ts.Teacher)
                        .ThenInclude(t => t.AppUser)
                        .Include(st => st.TaughtSubjects)
                        .ThenInclude(ts => ts.Classes)
                        .ThenInclude(c => c.ClassTime)
            );


        if (group == null)
        {
            return new GroupScheduleResponse(
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var todayDate = DateTime.Today;
        int diff = (7 + (todayDate.DayOfWeek - DayOfWeek.Monday)) % 7;
        var weekStart = todayDate.AddDays(-diff);
        var classesThisWeek = group.TaughtSubjects
            .SelectMany(ts => ts.Classes)
            .Where(c => (int)c.ClassTime.DaysOfTheWeek is >= 1 and <= 5)
            .Select(c =>
            {
                var classDate = weekStart.AddDays((int)c.ClassTime.DaysOfTheWeek - 1);
                var classDateTime = classDate.Add(c.ClassTime.Start);

                return new TodaysClassesDto(
                    c.Id,
                    c.TaughtSubject.Subject.Name,
                    c.ClassType.ToString(),
                    c.TaughtSubject.Teacher.AppUser.Name + " " + c.TaughtSubject.Teacher.AppUser.Surname,
                    c.ClassTime.Start,
                    c.ClassTime.End, new DateTimeOffset(classDateTime), c.Room,
                    c.TaughtSubject.Code
                );
            })
            .DistinctBy(x => new { x.Name, x.ClassType, x.Professor, x.Start })
            .OrderBy(x => x.Start)
            .ToList();
        return new GroupScheduleResponse(
            new GroupScheduleDto(DateTime.Now.ToString("dddd, MMM dd"), classesThisWeek),
            "Found", true, 200);
    }

    public async Task<DeleteGroupsResponse> DeleteAsync(string id)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
            );

        if (group is null)
        {
            return new DeleteGroupsResponse(StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        return await groupRepository.DeleteAsync(group)
            ? new DeleteGroupsResponse(StatusCode.Ok, true, ResponseMessages.Success)
            : new DeleteGroupsResponse(StatusCode.InternalServerError, false, ResponseMessages.Failed);
    }

    public async Task<UpdateGroupsResponse> UpdateAsync(string id, UpdateGroupRequest request)
    {
        var group =
            await groupRepository.GetByIdAsync(id,
                include: x =>
                    x.Include(e => e.Specialization)
                        .Include(e => e.Students)
                        .Include(e => e.AdmissionYear)
            );

        if (group is null)
        {
            return new UpdateGroupsResponse(null, StatusCode.NotFound, false, ResponseMessages.NotFound);
        }

        group.SpecializationId = request.SpecializationId;
        group.Code = request.GroupCode;
        group.AdmissionYear.FirstYear = request.Year;
        group.AdmissionYear.SecondYear += 1;

        return await groupRepository.UpdateAsync(group)
            ? new UpdateGroupsResponse(group.Id, StatusCode.Ok, true, ResponseMessages.Success)
            : new UpdateGroupsResponse(null, StatusCode.InternalServerError, false, ResponseMessages.Failed);
    }

    public async Task<CreateGroupsResponse> CreateAsync(CreateGroupRequest request)
    {
        var code = request.GroupCode.Trim();
        if (string.IsNullOrWhiteSpace(code))
            return new CreateGroupsResponse(null, StatusCode.BadRequest, false, "GroupCode is required");

        var normalized = code.ToUpper(); // or ToLower()

        var exists = await groupRepository.AnyAsync(g => g.Code.Trim().ToUpper() == normalized);

        if (exists)
            return new CreateGroupsResponse(null, StatusCode.Conflict, false, ResponseMessages.Failed);
        var admissionYear = GetAdmissionYear(request.Year);
        if (!await admissionYearRepository.CreateAsync(admissionYear))
        {
            return new CreateGroupsResponse(null, StatusCode.Conflict, false, "Failed to initialize  admission year");
        }

        // IMPORTANT: store normalized/trimmed form consistently
        var group = new Group
        {
            Code = code,
            AdmissionYearId = admissionYear.Id,
            EducationLanguage = request.EducationLanguage,
            EducationLevel = request.EducationLevel,
            SpecializationId = request.SpecializationId,
        };


        if (!await groupRepository.CreateAsync(group))
        {
            return new CreateGroupsResponse(null, StatusCode.InternalServerError, false, "Failed to create the group");
        }

        return new CreateGroupsResponse(group.Id, StatusCode.Ok, true, ResponseMessages.Success);
    }

    private static AdmissionYear GetAdmissionYear(int year) //3, today is 2025
    {
        var firstYear = DateTime.Now.Month >= 9 ? DateTime.Now.Year - year + 1 : DateTime.Now.Year - year; //2023
        var secondYear = DateTime.Now.Month <= 9 ? DateTime.Now.Year - year + 1 : DateTime.Now.Year - year + 2; //2024
        return new AdmissionYear
        {
            FirstYear = firstYear,
            SecondYear = secondYear
        };
    }
}