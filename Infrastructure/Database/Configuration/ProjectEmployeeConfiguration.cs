using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class ProjectEmployeeConfiguration
        : IEntityTypeConfiguration<ProjectEmployee>
    {
        public void Configure(EntityTypeBuilder<ProjectEmployee> builder)
        {
            // Composite Primary Key
            builder.HasKey(pe => new
            {
                pe.ProjectId,
                pe.EmployeeId
            });

            // Project -> ProjectEmployees
            builder.HasOne(pe => pe.Project)
                   .WithMany(p => p.ProjectEmployees)
                   .HasForeignKey(pe => pe.ProjectId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Employee -> ProjectEmployees
            builder.HasOne(pe => pe.Employee)
                   .WithMany(e => e.ProjectEmployees)
                   .HasForeignKey(pe => pe.EmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Role
            builder.Property(pe => pe.Role)
                   .HasMaxLength(20)
                   .IsRequired();

            builder.ToTable("ProjectEmployees");
        }
    }
}