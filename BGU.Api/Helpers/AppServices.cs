using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Api.Helpers;

public static class AppServices
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        //services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IExcelService, ExcelService>();
        services.AddScoped<IExcelCrudService, ExcelCrudService>();
        services.AddScoped<ITeacherService, TeacherService>();
        services.AddScoped<IDeanService, DeanService>();
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<ITaughtSubjectService, TaughtSubjectService>();
        services.AddScoped<IGroupService, GroupService>();
        services.AddScoped<ISyllabusService, SyllabusService>();
        services.AddScoped<IClassTimeService, ClassTimeService>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<ISpecializationService, SpecializationService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IColloquiumService, ColloquiumService>();
        services.AddScoped<ISeminarService, SeminarService>();
        services.AddScoped<IIndependentWorkService, IndependentWorkService>();

        //repos
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<IClassRepository, ClassRepository>();
        services.AddScoped<ITaughtSubjectRepository, TaughtSubjectRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IDeanRepository, DeanRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ITaughtSubjectRepository, TaughtSubjectRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<ISyllabusRepository, SyllabusRepository>();
        services.AddScoped<IAdmissionYearRepository, AdmissionYearRepository>();
        services.AddScoped<IClassTimeRepository, ClassTimeRepository>();
        services.AddScoped<ISubjectRepository, SubjectRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<ISpecializationRepository, SpecializationRepository>();
        services.AddScoped<IAttendanceRepository, AttendanceRepository>();
        services.AddScoped<IColloquiumRepository, ColloquiumRepository>();
        services.AddScoped<ISeminarRepository, SeminarRepository>();
        services.AddScoped<IIndependentWorkRepository, IndependentWorkRepository>();
        return services;
    }
}