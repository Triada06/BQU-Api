using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using BGU.Infrastructure.Repositories.Interfaces;

namespace BGU.Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : BaseRepository<AppUser>(context), IUserRepository;