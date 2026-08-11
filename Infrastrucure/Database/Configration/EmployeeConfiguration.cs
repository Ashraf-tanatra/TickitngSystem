using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastrucure.Database.Configration

{

    internal class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.FName)
                   .HasMaxLength(100);

            builder.Property(e => e.LName)
                   .HasMaxLength(100);

            builder.Property(e => e.Phone)
                   .HasMaxLength(20);

            builder.Property(e => e.Gender)
                   .HasMaxLength(20);
        }
    }
}
