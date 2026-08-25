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


            // Email Verification Code
            builder.Property(x => x.VerificationCode)
                   .HasColumnType("VARCHAR")
                   .HasMaxLength(6);

            // Email Verification Code Expiration
            builder.Property(x => x.VerificationCodeExpiresAt)
                   .HasColumnType("DATETIME");

            // Email Verified
            builder.Property(x => x.IsEmailVerified)
                   .IsRequired();


            builder.ToTable("Accounts");
        }
    }
}