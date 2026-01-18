using System.Security.Claims;
using System.Text;
using BGU.Api;
using BGU.Api.Filters;
using BGU.Api.Helpers;
using BGU.Application.Dtos.AppUser;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;


// Configure OpenAPI with JWT authentication
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "BGU API",
            Version = "v1",
            Description = "BGU Application API with JWT Authentication"
        };

        // Add JWT Bearer authentication scheme
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Enter your JWT token in the format: Bearer {your token}"
            }
        };

        // Apply security requirement globally
        document.SecurityRequirements = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            }
        };

        return Task.CompletedTask;
    });
});
// builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DbConStr"))
);

// Add Data Protection services (required for Identity token providers)
builder.Services.AddDataProtection();
builder.Services.AddAppServices();
builder.Services.AddAuthorization();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResponseStatusCodeFilter>();
    options.Filters.Add<ValidationFilter>(); // Add this
});
builder.Services.AddProblemDetails();
builder.Services.AddValidatorsFromAssemblyContaining<AppUserCreateValidator>();

builder.Services.AddIdentityCore<AppUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.SignIn.RequireConfirmedEmail = false;

        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(o =>
{
    o.User.RequireUniqueEmail = false;
    o.SignIn.RequireConfirmedEmail = false;
});

var jwtSettings = configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"])
            ),
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", p => p
        .WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        // .AllowCredentials() // включай ТОЛЬКО если используешь куки/credentials
    );
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
    Console.WriteLine("=== SEED START ===");
    await db.Database.MigrateAsync();

    foreach (var role in new[] { "Dean", "Student", "Teacher" })
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));    
    
    const string roomName = "Otaq 317";
    
    var room = await db.Rooms.SingleOrDefaultAsync(f => f.Name == roomName);
    if (room == null)
    {
        room = new Room {
            Name = roomName,
            Capacity = 20,
            CreatedAt = DateTime.UtcNow
        };
        db.Rooms.Add(room);
        await db.SaveChangesAsync();
    }
    
    const string facultyName = "Tetbiqi Riyaziyyat";
    
    var faculty = await db.Faculties.SingleOrDefaultAsync(f => f.Name == facultyName);
    if (faculty == null)
    {
        faculty = new Faculty {
            Name = facultyName,
            CreatedAt = DateTime.UtcNow
        };
        db.Faculties.Add(faculty);
        await db.SaveChangesAsync();
    }

    
    const string specializationName = "Komputer Elmleri";
    
    var specialization = await db.Specializations.SingleOrDefaultAsync(f => f.Name == specializationName);
    if (specialization == null)
    {
        specialization = new Specialization {
            Name = specializationName,
            FacultyId = faculty.Id,
            CreatedAt = DateTime.UtcNow
        };
        db.Specializations.Add(specialization);
        await db.SaveChangesAsync();
    }
    
    const string departmentName = "Riyazi Kibernetika";
    
    var department = await db.Departments.SingleOrDefaultAsync(f => f.Name == departmentName);
    if (department == null)
    {
        department = new Department {
            Name = departmentName,
            FacultyId = faculty.Id,
            CreatedAt = DateTime.UtcNow
        };
        db.Departments.Add(department);
        await db.SaveChangesAsync();
    }
    
    const string username = "7KW323K";
    const string password = "Atilla123";
    
    var user = await userManager.FindByNameAsync(username);
    if (user == null)
    {
        user = new AppUser {
            UserName = username,
            Name = "Resad",
            Surname = "Mehdiev",
            MiddleName = "Ali",
            Gender = 'M',
        };

        var res = await userManager.CreateAsync(user, password);
        if (!res.Succeeded) throw new Exception(string.Join("; ", res.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Dean");

        db.Deans.Add(new Dean
        {
            AppUserId = user.Id,
            FacultyId = faculty.Id,
            RoleName = "Dekan",
            CreatedAt = DateTime.UtcNow
        });

        await db.SaveChangesAsync();
    }
    Console.WriteLine("=== SEED END ===");
}

app.UseStaticFiles();
app.UseRouting();

app.UseCors("DevCors");

// app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.BluePlanet;
        options.Title = "BGU API Documentation";
        options.Authentication = new ScalarAuthenticationOptions
        {
            PreferredSecurityScheme = "Bearer"
        };
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/healthcheck", () => Results.Text("WHOAMI"));

app.Run();
