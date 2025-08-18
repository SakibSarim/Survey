using Microsoft.EntityFrameworkCore;
using TsrmWebApi.Models.PresentationModels;


namespace TsrmWebApi.Models.DataModels
{
    public class SurveyDbContext : DbContext
    {
        public SurveyDbContext(DbContextOptions<SurveyDbContext> options) : base(options)
        {
        }

        // Define DbSets for your entities
        public DbSet<UserInfo> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Example of configuring a User entity
            modelBuilder.Entity<UserInfo>()
                .HasKey(u => u.UserId);

            modelBuilder.Entity<UserInfo>()
                .Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(50);

            // Add additional configurations here
        }
    }
}
