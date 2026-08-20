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


            // Is Deleted
            builder.Property(e => e.IsDeleted)
                   .IsRequired();


            // Employee -> ProjectEmployees
            builder.HasMany(e => e.ProjectEmployees)
                   .WithOne(pe => pe.Employee)
                   .HasForeignKey(pe => pe.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Employee -> Assigned Tickets
            builder.HasMany(e => e.Tickets)
                   .WithOne(t => t.Employee)
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.ToTable("Employees");
        }
    }
}