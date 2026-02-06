using BGU.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BGU.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<AppUser>(options)
{
    public DbSet<AcademicPerformance> AcademicPerformances { get; set; }
    public DbSet<AdmissionYear> AdmissionYears { get; set; }
    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Class> Classes { get; set; }
    public DbSet<ClassTime> ClassTimes { get; set; }
    public DbSet<Colloquiums> Colloquiums { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<IndependentWork> IndependentWorks { get; set; }
    public DbSet<Specialization> Specializations { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<StudentAcademicInfo> StudentAcademicInfos { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<TaughtSubject> TaughtSubjects { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<TeacherAcademicInfo> TeacherAcademicInfos { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<Seminar> Seminars { get; set; }
    public DbSet<Dean> Deans { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Syllabus> Syllabus { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AppUser>(b =>
        {
            b.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(256);

            b.Property(u => u.NormalizedUserName)
                .IsRequired()
                .HasMaxLength(256);

            b.HasIndex(u => u.NormalizedUserName).IsUnique();
        });

        builder.Entity<Room>()
            .Property(x => x.Name)
            .HasMaxLength(20);
        builder.Entity<Syllabus>()
            .Property(x => x.Name)
            .HasMaxLength(100);

        builder.Entity<Faculty>()
            .HasOne(f => f.Dean)
            .WithOne(d => d.Faculty)
            .HasForeignKey<Dean>(d => d.FacultyId);

        builder.Entity<Seminar>().Property(x => x.Topic).HasMaxLength(50);

        builder.Entity<IndependentWork>()
            .ToTable(t =>
                t.HasCheckConstraint(
                    "CK_Number_Range",
                    "\"Number\" >= 0 AND \"Number\" <= 10"
                ));

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType.IsEnum)
                {
                    var converterType = typeof(EnumToStringConverter<>).MakeGenericType(property.ClrType);
                    var converter = Activator.CreateInstance(converterType);
                    property.SetValueConverter((ValueConverter)converter);
                }
            }
        }

        base.OnModelCreating(builder);
    }
}