using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    internal class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(a => a.Email)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.HasIndex(a => a.Email)
                   .IsUnique(); // delete this check this in program (program it)

            builder.Property(a => a.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasOne(a => a.Employee)
                   .WithOne(e => e.Account)
                   .HasForeignKey<Account>(a => a.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}