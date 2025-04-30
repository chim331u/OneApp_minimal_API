using System.Reflection;
using OneApp_minimalApi.Models;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.AppContext;
using OneApp_minimalApi.Exceptions;
using OneApp_minimalApi.Interfaces;
using OneApp_minimalApi.Services;

namespace OneApp_minimalApi.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring application services.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adds application-specific services to the specified <see cref="IHostApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IHostApplicationBuilder"/> to configure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="builder"/> or its configuration is null.</exception>
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (builder.Configuration == null) throw new ArgumentNullException(nameof(builder.Configuration));

            // Adding the database context
            builder.Services.AddDbContext<ApplicationContext>(configure =>
            {
                configure.UseSqlite(builder.Configuration.GetConnectionString("sqliteConnection"));
            });

            // Adding validators from the current assembly
            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            // Register services
            builder.Services.AddScoped<IFilesDetailService, FilesDetailService>();
            builder.Services.AddScoped<IConfigsService, ConfigsService>();
            builder.Services.AddScoped<IUtilityServices, UtilityServices>();
            builder.Services.AddScoped<IHangFireJobService, HangFireJobService>();
            builder.Services.AddScoped<IMachineLearningService, MachineLearningService>();
            builder.Services.AddScoped<IDDService, DDService>();
            builder.Services.AddScoped<IDockerConfigsService, DockerConfigsService>();
            builder.Services.AddScoped<IDeployDetailService, DeployDetailService>();
            builder.Services.AddScoped<IDockerCommandService, DockerCommandService>();

            // Register exception handler
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            // Add problem details for standardized error responses
            builder.Services.AddProblemDetails();
        }
    }
}