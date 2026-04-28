using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddScoped<ISolicitacoesRepository, SolicitacoesRepository>();
        services.AddScoped<ISolicitacoesService, SolicitacoesService>();
        services.AddScoped<IPlanejamentosRepository, PlanejamentosRepository>();
        services.AddScoped<IPlanejamentosService, PlanejamentosService>();
        services.AddScoped<IAprovacoesRepository, AprovacoesRepository>();
        services.AddScoped<IAprovacoesService, AprovacoesService>();
        services.AddScoped<IExecucoesRepository, ExecucoesRepository>();
        services.AddScoped<IExecucoesService, ExecucoesService>();

        return services;
    }
}
