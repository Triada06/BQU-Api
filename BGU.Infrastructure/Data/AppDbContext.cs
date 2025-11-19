using BGU.Core.Entities;
using BGU.Core.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    public DbSet<AdmissionYear> AdmissionYears { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassTime> ClassTimes { get; set; }
    public DbSet<Colloquiums> Colloquiums { get; set; }
    public DbSet<Decree> Decrees { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<IndependentWork> IndependentWorks { get; set; }
    public DbSet<LectureHall> LectureHalls { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentAcademicInfo> StudentAcademicInfos { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TaughtSubject> TaughtSubjects { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TeacherAcademicInfo> TeacherAcademicInfos { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Seminar> Seminars { get; set; }
    public DbSet<ClassSession> ClassSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>()
            .Property(x => x.BornDate)
            .HasColumnType("date");
        base.OnModelCreating(builder);
    }
}