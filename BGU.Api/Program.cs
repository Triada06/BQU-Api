using System.Text;
using BGU.Api;
using BGU.Api.Filters;
using BGU.Api.Helpers;
using BGU.Application.Common;
using BGU.Core.Entities;
using BGU.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using DotNetEnv;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

Env.Load();

builder.Services.Configure<UrlOptions>(configuration.GetRequiredSection("Urls"));

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
builder.Services.AddAppServices(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ResponseStatusCodeFilter>();
    options.Filters.Add<ValidationFilter>(); 
});
builder.Services.AddProblemDetails();

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
                "http://localhost:3001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
        // .AllowCredentials() // включай ТОЛЬКО если используешь куки/credentials
    );
});

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;

    var db = sp.GetRequiredService<AppDbContext>();
    var userManager = sp.GetRequiredService<UserManager<AppUser>>();
    var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();

    await db.Database.MigrateAsync();

    await DbSeeder.SeedAsync(db);
    await IdentitySeeder.SeedDeansFromEnvAsync(db, userManager, roleManager);
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