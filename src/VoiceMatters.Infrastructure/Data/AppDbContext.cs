using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace VoiceMatters.Infrastructure.Data
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Petition> Petitions { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Statistic> Statistic {get; set; }
        public DbSet<AppUserSignedPetition> AppUserSignedPetitions { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("VoiceMatters");

            var configuration = new AppConfiguration();

            modelBuilder.ApplyConfiguration<Role>(configuration);
            modelBuilder.ApplyConfiguration<AppUser>(configuration);
            modelBuilder.ApplyConfiguration<AppUserSignedPetition>(configuration);
            modelBuilder.ApplyConfiguration<PetitionTag>(configuration);
            modelBuilder.ApplyConfiguration<Image>(configuration);
            modelBuilder.ApplyConfiguration<News>(configuration);
            modelBuilder.ApplyConfiguration<Petition>(configuration);
            modelBuilder.ApplyConfiguration<Statistic>(configuration);
            modelBuilder.ApplyConfiguration<Tag>(configuration);


            base.OnModelCreating(modelBuilder);
        }
    }
}
