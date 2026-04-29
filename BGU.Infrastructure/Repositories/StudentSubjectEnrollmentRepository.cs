using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class StudentSubjectEnrollmentRepository(AppDbContext context)
    : BaseRepository<StudentSubjectEnrollment>(context), IStudentSubjectEnrollmentRepository;