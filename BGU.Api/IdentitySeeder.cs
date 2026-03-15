namespace BGU.Api;

using Core.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class IdentitySeeder
{
    public static async Task SeedDeansFromEnvAsync(
        AppDbContext db,
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager,
        CancellationToken ct = default)
    {
        await EnsureRoleAsync(roleManager, "Dean");
        await EnsureRoleAsync(roleManager, "Student");
        await EnsureRoleAsync(roleManager, "Teacher");

        // Вытаскиваем деканов по префиксам SEED_DEAN1_ и SEED_DEAN2_
        var prefixes = new[] { "SEED_DEAN1_", "SEED_DEAN2_", "SEED_DEAN_3" };

        foreach (var p in prefixes)
        {
            var username = GetEnv(p + "USERNAME");
            var password = GetEnv(p + "PASSWORD");
            var facultyName = GetEnv(p + "FACULTY");
            var name = GetEnv(p + "NAME");
            var surname = GetEnv(p + "SURNAME");
            var middleName = GetEnv(p + "MIDDLENAME");

            var faculty = await db.Faculties.SingleOrDefaultAsync(f => f.Name == facultyName, ct);
            if (faculty == null)
                throw new Exception($"Faculty '{facultyName}' not found (env: {p}FACULTY). Run DbSeeder first.");

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = username,
                    Name = name,
                    Surname = surname,
                    MiddleName = middleName,
                };

                var res = await userManager.CreateAsync(user, password);
                if (!res.Succeeded)
                    throw new Exception(string.Join("; ", res.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(user, "Dean"))
            {
                var res = await userManager.AddToRoleAsync(user, "Dean");
                if (!res.Succeeded)
                    throw new Exception(string.Join("; ", res.Errors.Select(e => e.Description)));
            }

            var deanExists = await db.Deans.AnyAsync(d => d.AppUserId == user.Id && d.FacultyId == faculty.Id, ct);
            if (!deanExists)
            {
                db.Deans.Add(new Dean
                {
                    AppUserId = user.Id,
                    FacultyId = faculty.Id,
                    RoleName = "Dekan",
                    CreatedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync(ct);
            }
        }
    }

    private static string GetEnv(string key)
        => Environment.GetEnvironmentVariable(key)
           ?? throw new Exception($"Missing env var: {key}");

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName)) return;

        var res = await roleManager.CreateAsync(new IdentityRole(roleName));
        if (!res.Succeeded)
            throw new Exception($"Failed to create role '{roleName}': " +
                                string.Join("; ", res.Errors.Select(e => e.Description)));
    }
}