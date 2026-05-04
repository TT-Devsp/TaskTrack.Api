using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskTrack.Application.Interfaces;
using TaskTrack.Application.Services;
using TaskTrack.Domain.Interfaces;
using TaskTrack.Infrastructure.Identity;
using TaskTrack.Infrastructure.Persistence;
using TaskTrack.Infrastructure.Repositories;

namespace TaskTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        ConfigureAuthentication(services, configuration);

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ISolicitacoesRepository, SolicitacoesRepository>();
        services.AddScoped<ISolicitacoesService, SolicitacoesService>();
        services.AddScoped<IPlanejamentosRepository, PlanejamentosRepository>();
        services.AddScoped<IPlanejamentosService, PlanejamentosService>();
        services.AddScoped<IAprovacoesRepository, AprovacoesRepository>();
        services.AddScoped<IAprovacoesService, AprovacoesService>();
        services.AddScoped<IExecucoesRepository, ExecucoesRepository>();
        services.AddScoped<IExecucoesService, ExecucoesService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();

        return services;
    }

    private static void ConfigureAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
        var issuer = configuration["Jwt:Issuer"] ?? "TaskTrack.Api";
        var audience = configuration["Jwt:Audience"] ?? "TaskTrack.Client";

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}
