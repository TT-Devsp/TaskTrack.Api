using Microsoft.EntityFrameworkCore;
using System.Reflection;
using TaskTrack.Infrastructure;
using TaskTrack.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await ApplyMigrationsWithRetryAsync(app);

app.Run();

static async Task ApplyMigrationsWithRetryAsync(WebApplication app)
{
    const int maxRetries = 10;
    var delay = TimeSpan.FromSeconds(5);

    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("StartupMigration");

    for (var attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso.");
            return;
        }
        catch (Exception ex)
        {
            if (attempt == maxRetries)
            {
                throw new InvalidOperationException(
                    $"Nao foi possivel aplicar migrations apos {maxRetries} tentativas.",
                    ex);
            }

            logger.LogWarning(
                ex,
                "Falha ao aplicar migrations (tentativa {Attempt}/{MaxRetries}). Nova tentativa em {DelaySeconds}s.",
                attempt,
                maxRetries,
                delay.TotalSeconds);

            await Task.Delay(delay);
        }
    }
}
