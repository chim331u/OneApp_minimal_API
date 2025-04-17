using fc_minimalApi.Models;
using Microsoft.EntityFrameworkCore;

namespace fc_minimalApi.AppContext
{ 
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
    {

        // // Default schema for the database context
        // private const string DefaultSchema = "fc_minimalApi";

        // DbSet to represent the collection of books in our database
        public DbSet<FilesDetail> FilesDetail { get; set; }

        // Constructor to configure the database context

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        }

    }
}