using System.Reflection;
using fc_minimalApi.AppContext;
using fc_minimalApi.Exceptions;
using fc_minimalApi.Interfaces;
using fc_minimalApi.Models;
using fc_minimalApi.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace fc_minimalApi.Extensions
{
    public static class ServiceExtensions
    {
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

            //Register services
            builder.Services.AddScoped<IFilesDetailService, FilesDetailService>();
            builder.Services.AddScoped<IConfigsService, ConfigsService>();
            builder.Services.AddScoped<IUtilityServices, UtilityServices>();
            builder.Services.AddScoped<IHangFireJobService, HangFireJobService>();
            builder.Services.AddScoped<IMachineLearningService, MachineLearningService>();
            builder.Services.AddScoped<IDDService, DDService>();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
            builder.Services.AddProblemDetails();
        }
    }
}