using System.Linq.Expressions;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BGU.Infrastructure.Repositories;

public class StudentRepository(AppDbContext context) : BaseRepository<Student>(context), IStudentRepository
{
  
}