using System.Reflection;
using fc_minimalApi.AppContext;
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
        }
    }
}