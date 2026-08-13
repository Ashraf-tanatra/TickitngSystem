using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.ProjectName).HasColumnType("varchar").HasMaxLength(125);
            builder.Property(x => x.ProjectDescription).HasColumnType("varchar").HasMaxLength(255);

            builder.HasMany(x => x.Employees).WithMany(x => x.Projects).UsingEntity(j => j.ToTable("ProjectEmployees"));

            builder.ToTable("Projects");

        }
    }
}
