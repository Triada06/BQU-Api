using BGU.Application.Contracts.Student.Responses;
using BGU.Application.Contracts.Teacher.Responses;
using BGU.Application.Dtos.Teacher;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Constants;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BGU.Application.Services;

public class TeacherService(UserManager<AppUser> userManager, ITeacherRepository teacherRepository) : ITeacherService
{
    public async Task<TeacherProfileResponse> GetProfile(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return new TeacherProfileResponse(null, null,
                ResponseMessages.Unauthorized, false,
                (int)StatusCode.Unauthorized);
        }

        var teacher = (await teacherRepository.FindAsync(
            s => s.AppUserId == userId,
            s => s.Include(st => st.TeacherAcademicInfo)
                .ThenInclude(ai => ai.Department)
                .ThenInclude(g => g.Faculty)
                .ThenInclude(ts => ts.Specializations)
        )).FirstOrDefault();

        if (teacher == null)
        {
            return new TeacherProfileResponse(
                null,
                null,
                ResponseMessages.NotFound,
                false,
                (int)StatusCode.NotFound
            );
        }

        var teacherSpecialization = teacher.TeacherAcademicInfo.Department.Faculty.Specializations
            .Where(t => t.FacultyId == teacher.TeacherAcademicInfo.Department.FacultyId).Select(x => x.Name)
            .FirstOrDefault();
        if (teacherSpecialization == null)
        {
            return new TeacherProfileResponse(
                null,
                null,
                "Teacher Specialization not found",
                false,
                (int)StatusCode.NotFound);
        }

        var teacherPersonalInfoDto = new TeacherPersonalInfoDto(user.Email!, user.BornDate);
        var teacherAcademicInfoDto = new TeacherAcademicInfoDto(user.Name, user.Surname, teacher.Id,
            teacher.TeacherAcademicInfo.Department.Faculty.Name, teacher.TeacherAcademicInfo.Department.Faculty.Name,
            teacherSpecialization);
        return new TeacherProfileResponse(teacherPersonalInfoDto, teacherAcademicInfoDto, ResponseMessages.Success,
            true, (int)StatusCode.Ok);
    }
}