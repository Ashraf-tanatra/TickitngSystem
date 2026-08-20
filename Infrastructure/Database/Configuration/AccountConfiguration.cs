using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class AccountConfiguration
        : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();


            // Email
            builder.Property(a => a.Email)
                   .HasColumnType("varchar")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.HasIndex(a => a.Email)
                   .IsUnique();


            // Password
            builder.Property(a => a.PasswordHash)
                   .HasColumnType("varchar")
                   .HasMaxLength(500)
                   .IsRequired();


            // Employee
            builder.HasOne(a => a.Employee)
                   .WithOne(e => e.Account)
                   .HasForeignKey<Account>(a => a.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Is Deleted
            builder.Property(a => a.IsDeleted)
                   .IsRequired();


            // Deleted At
            builder.Property(a => a.DeletedAt)
                   .HasColumnType("datetime");


            // Email Confirmed
            builder.Property(a => a.EmailConfirmed)
                   .IsRequired();


            // Verification Code
            builder.Property(a => a.VerificationCode)
                   .HasColumnType("varchar")
                   .HasMaxLength(6);


            // Verification Code Expiration
            builder.Property(a => a.VerificationCodeExpiresAt)
                   .HasColumnType("datetime");


            builder.ToTable("Accounts");
        }
    }
}