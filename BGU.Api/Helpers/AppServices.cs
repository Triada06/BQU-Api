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

        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}