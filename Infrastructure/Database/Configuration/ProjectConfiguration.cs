using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class ProjectConfiguration
        : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.ProjectName)
                   .HasColumnType("varchar")
                   .HasMaxLength(125)
                   .IsRequired();

            builder.Property(x => x.ProjectDescription)
                   .HasColumnType("varchar")
                   .HasMaxLength(255);

            builder.HasOne(x => x.ProjectManager)
                   .WithMany()
                   .HasForeignKey(x => x.ProjectManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Projects");
        }
    }
}