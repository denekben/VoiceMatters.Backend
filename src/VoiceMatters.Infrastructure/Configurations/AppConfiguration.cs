using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Infrastructure.Data;

namespace VoiceMatters.Infrastructure.Configurations
{
    public class AppConfiguration : IEntityTypeConfiguration<Role>, IEntityTypeConfiguration<AppUser>,
        IEntityTypeConfiguration<AppUserSignedPetition>, IEntityTypeConfiguration<PetitionTag>, IEntityTypeConfiguration<Image>,
        IEntityTypeConfiguration<News>, IEntityTypeConfiguration<Petition>, IEntityTypeConfiguration<Statistic>,
        IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasKey(r => r.Id);

            builder.ToTable("Roles");

            builder.HasData(Seed.Roles);
        }

        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.HasKey(u => u.Id);

            builder
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Users");
        }

        public void Configure(EntityTypeBuilder<AppUserSignedPetition> builder)
        {
            builder.HasKey(usp => new { usp.PetitionId, usp.SignerId });

            builder
                .HasOne(usp => usp.Signer)
                .WithMany(u => u.PetitionsSignedByUser)
                .HasForeignKey(usp => usp.SignerId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(usp => usp.Petition)
                .WithMany(p => p.SignedUsers)
                .HasForeignKey(usp => usp.PetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("AppUserSignedPetitions");
        }
        public void Configure(EntityTypeBuilder<PetitionTag> builder)
        {
            builder.HasKey(pt => pt.Id);

            builder
                .HasOne(pt => pt.Petition)
                .WithMany(p => p.PetitionTags)
                .HasForeignKey(pt => pt.PetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PetitionTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("PostTags");
        }

        public void Configure(EntityTypeBuilder<Image> builder)
        {
            builder.HasKey(i => i.Id);

            builder
                .HasOne(i => i.Petition)
                .WithMany(p => p.Images)
                .HasForeignKey(i => i.PetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Images");
        }
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.HasKey(n => n.Id);

            builder
                .HasOne(n => n.Petition)
                .WithOne(p => p.News)
                .HasForeignKey<News>(n => n.PetitionId);

            builder.ToTable("News");
        }
        public void Configure(EntityTypeBuilder<Petition> builder)
        {
            builder.HasKey(p => p.Id);

            builder
                .HasOne(p => p.Creator)
                .WithMany(u => u.PetitionsCreatedByUser)
                .HasForeignKey(p => p.CreatorId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.ToTable("Petitions");
        }
        public void Configure(EntityTypeBuilder<Statistic> builder)
        {
            builder.HasKey(s => s.Id);

            builder.HasData(Seed.Statistic);

            builder.ToTable("Statistics");
        }
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.HasKey(t => t.Id);

            builder.ToTable("Tags");
        }
    }
}
