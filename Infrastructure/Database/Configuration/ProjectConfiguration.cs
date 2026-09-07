using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class ProjectConfiguration
        : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.CreatedAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.UpdatedAt)
                   .HasColumnType("datetime2");

            builder.Property(x => x.ProjectName)
                   .HasColumnType("varchar")
                   .HasMaxLength(125)
                   .IsRequired();

            builder.Property(x => x.ProjectDescription)
                   .HasColumnType("varchar")
                   .HasMaxLength(255);

            builder.Property(x => x.ProjectStatus)
                   .HasConversion(
                       x => x.ToString(),
                       x => (ProjectStatus)Enum.Parse(
                           typeof(ProjectStatus), x));

            builder.HasOne(x => x.ProjectManager)
                   .WithMany(e => e.ManagedProjects)
                   .HasForeignKey(x => x.ProjectManagerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(x => x.ProjectTickets)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.Navigation(x => x.ProjectEmployees)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.ToTable("Projects");
        }
    }
}
