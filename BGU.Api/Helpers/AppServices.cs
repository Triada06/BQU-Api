using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Api.Helpers;

public static class AppServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IExcelCrudService, ExcelCrudService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IDeanService, DeanService>();
        services.AddScoped<IRoomService, RoomService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<ITaughtSubjectRepository, TaughtSubjectRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IDeanRepository, DeanRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        return services;
    }
}