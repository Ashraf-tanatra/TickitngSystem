using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    internal class AccountConfiguration
        : IEntityTypeConfiguration<Account>
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
                   .IsUnique();

            builder.Property(a => a.PasswordHash)
                   .IsRequired()
                   .HasMaxLength(500);
        }
    }
}