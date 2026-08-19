using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FName)
                   .HasMaxLength(50);

            builder.Property(e => e.LName)
                   .HasMaxLength(50);

            builder.Property(e => e.Phone)
                   .HasMaxLength(10);

            builder.Property(x => x.Gender)
                   .HasConversion(
                       x => x.ToString(),
                       x => (Gender)Enum.Parse(
                           typeof(Gender), x));
        }
    }
}