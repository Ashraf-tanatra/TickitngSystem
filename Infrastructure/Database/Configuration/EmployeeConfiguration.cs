using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    internal class EmployeeConfiguration
        : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(e => e.CreatedAt)
                   .HasColumnType("datetime2");

            builder.Property(e => e.UpdatedAt)
                   .HasColumnType("datetime2");

            // First Name
            builder.Property(e => e.FName)
                   .HasMaxLength(50)
                   .IsRequired();

            // Last Name
            builder.Property(e => e.LName)
                   .HasMaxLength(50)
                   .IsRequired();

            // Phone
            builder.Property(e => e.Phone)
                   .HasMaxLength(10)
                   .IsRequired();

            // Gender
            builder.Property(e => e.Gender)
                   .HasColumnType("char(1)")
                   .IsRequired();

            // Soft Delete
            builder.Property(e => e.IsDeleted)
                   .IsRequired();

            builder.Property(e => e.DeletedAt)
                   .HasColumnType("date");

            // Employee -> Assigned Tickets
            builder.HasMany(e => e.Tickets)
                   .WithOne(t => t.Employee)
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(e => e.Tickets)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(e => e.ManagedProjects)
                   .WithOne(p => p.ProjectManager)
                   .HasForeignKey(p => p.ProjectManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(e => e.ManagedProjects)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(e => e.ProjectEmployees)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
