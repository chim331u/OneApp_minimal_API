using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OneApp_minimalApi.Models;
using OneApp_minimalApi.Models.Identity;

// ReSharper disable All

namespace OneApp_minimalApi.AppContext
{ 
    /// <summary>
    /// Represents the database context for the application.
    /// </summary>
    public class ApplicationContext(DbContextOptions<ApplicationContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        // // Default schema for the database context
        // private const string DefaultSchema = "fc_minimalApi";

        /// <summary>
        /// Gets or sets the DbSet representing the collection of file details in the database.
        /// </summary>
        public DbSet<FilesDetail> FilesDetail { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the collection of configurations in the database.
        /// </summary>
        public DbSet<Configs> Configuration { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the collection of NasSettings in the database.
        /// </summary>
        public DbSet<Settings> DDSettings { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet representing the collection of DD_Threads in the database.
        /// </summary>
        public DbSet<DD_Threads> DDThreads { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet representing the collection of DD_LinkEd2k in the database.
        /// </summary>
        public DbSet<DD_LinkEd2k> DDLinkEd2 { get; set; }
        
        /// <summary>
        /// Gets or sets the DbSet representing the collection of Docker configurations in the database.
        /// </summary>
        public DbSet<DockerConfig> DockerConfig { get; set; }

        /// <summary>
        /// Gets or sets the DbSet representing the collection of deployment details in the database.
        /// </summary>
        public DbSet<DeployDetail> DeployDetails { get; set; }
        
        public  DbSet<TokenInfo> TokenInfo { get; set; }
        
        /// <summary>
        /// Configures the model and relationships for the database context.
        /// </summary>
        /// <param name="modelBuilder">The builder used to construct the model for the database context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.Entity<DockerConfig>().ToTable("DD_ProjectDeployerConfig")
                .HasMany(c => c.DeployDetails);

            modelBuilder.Entity<DeployDetail>().ToTable("DD_DeployDetails");
            
            modelBuilder.Entity<Settings>().ToTable("DDSettings");
            
            // Apply configurations from the current assembly.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            // // Apply configurations from the current assembly again (duplicate call, consider removing if unnecessary).
            // modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }
    }
}