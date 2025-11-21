using BGU.Application.Contracts.Teacher.Responses;

namespace BGU.Application.Services.Interfaces;

public interface ITeacherService
{
    Task<TeacherProfileResponse>  GetProfile(string userId);
}