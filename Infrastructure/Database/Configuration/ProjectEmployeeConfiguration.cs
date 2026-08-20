using Domain.Entities;
using Domain.Enum;
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


            // Project
            builder.HasOne(pe => pe.Project)
                   .WithMany(p => p.ProjectEmployees)
                   .HasForeignKey(pe => pe.ProjectId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Employee
            builder.HasOne(pe => pe.Employee)
                   .WithMany(e => e.ProjectEmployees)
                   .HasForeignKey(pe => pe.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Role inside the project
            builder.Property(pe => pe.Role)
                   .HasConversion(
                       role => role.ToString(),
                       role => Enum.Parse<ProjectRole>(role))
                   .IsRequired();


            builder.ToTable("ProjectEmployees");
        }
    }
}