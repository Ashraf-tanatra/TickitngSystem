using Microsoft.EntityFrameworkCore;
using Domain.Entities;
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

            builder.Property(a => a.CreatedAt)
                   .HasColumnType("datetime2");

            builder.Property(a => a.UpdatedAt)
                   .HasColumnType("datetime2");


            // Email
            builder.Property(a => a.Email)
                   .HasColumnType("varchar")
                   .HasMaxLength(255)
                   .IsRequired();

            builder.HasIndex(a => a.Email)
                   .IsUnique(); // delete this check this in program (program it)


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

            builder.Property(a => a.IsDeleted)
                   .IsRequired();

            builder.Property(a => a.IsAnonymized)
                   .IsRequired();

            builder.Property(a => a.DeletedAt)
                   .HasColumnType("datetime2");


            // Email Verification Code
            builder.Property(x => x.VerificationCode)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(6);

            // Email Verification Code Expiration
            builder.Property(x => x.VerificationCodeExpiresAt)
                   .HasColumnType("DATETIME");

            builder.Property(x => x.PasswordResetCode)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(6);

            builder.Property(x => x.PasswordResetCodeExpiresAt)
                   .HasColumnType("DATETIME");

            // Email Verified
            builder.Property(x => x.IsEmailVerified)
                   .IsRequired();


            builder.ToTable("Accounts");
        }
    }
}
