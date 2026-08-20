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
            // Primary Key
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                   .ValueGeneratedOnAdd();

            // Project Name
            builder.Property(p => p.ProjectName)
                   .HasColumnType("varchar")
                   .HasMaxLength(125)
                   .IsRequired();

            // Project Description
            builder.Property(p => p.ProjectDescription)
                   .HasColumnType("varchar")
                   .HasMaxLength(255);

            // Project Manager
            builder.HasOne(p => p.ProjectManager)
                   .WithMany()
                   .HasForeignKey(p => p.ProjectManagerId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Project -> Tickets
            builder.HasMany(p => p.ProjectTickets)
                   .WithOne(t => t.Project)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Projects");
        }
    }
}