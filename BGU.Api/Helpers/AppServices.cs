using BGU.Application.Common;
using BGU.Application.Common.HelperServices;
using BGU.Application.Common.HelperServices.Interfaces;
using BGU.Application.Services;
using BGU.Application.Services.Interfaces;
using BGU.Core.Entities;
using BGU.Infrastructure.Repositories;
using BGU.Infrastructure.Repositories.Interfaces;
using BGU.Infrastructure.Workers;
using Microsoft.AspNetCore.Identity;

namespace BGU.Api.Helpers;

public static class AppServices {
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration) {
        //services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IExcelService, ExcelService>();
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
        services.AddScoped<IStudentSubjectEnrollmentService, StudentSubjectEnrollmentService>();
        services.AddScoped<IFinalService, FinalService>();
        services.AddScoped<IStudentSubjectResultService, StudentSubjectResultService>();
        services.AddScoped<IAcademicHelper, AcademicHelper>();
        services.AddScoped<ITranscriptService, TranscriptService>();
        services.AddScoped<INotificationService, NotificationService>();
        
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
        services.AddScoped<IStudentSubjectEnrollmentRepository, StudentSubjectEnrollmentRepository>();
        services.AddScoped<IFinalRepository, FinalRepository>();
        services.AddScoped<IStudentSubjectResultRepository, StudentSubjectResultRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddSingleton<IEmailSender<AppUser>, MailKitEmailSender>();
        
        
        services.AddHostedService<FinalBackGroundService>();
        services.AddHostedService<NotificationCleanerBackgroundService>();
        return services;
    }
}