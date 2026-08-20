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


            // Project -> ProjectEmployees
            builder.HasMany(p => p.ProjectEmployees)
                   .WithOne(pe => pe.Project)
                   .HasForeignKey(pe => pe.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Project -> Tickets
            builder.HasMany(p => p.ProjectTickets)
                   .WithOne(t => t.Project)
                   .HasForeignKey(t => t.ProjectId)
                   .OnDelete(DeleteBehavior.Restrict);




            builder.ToTable("Projects");
        }
    }
}