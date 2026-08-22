using Domain.Entities;
using Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            // Primary Key
            builder.HasKey(x => x.TicketId);

            builder.Property(x => x.TicketId)
                   .ValueGeneratedOnAdd();


            // Title
            builder.Property(x => x.TicketTitle)
                   .HasColumnType("varchar")
                   .HasMaxLength(20)
                   .IsRequired();


            // Priority
            builder.Property(x => x.Priority)
                   .HasConversion(
                       x => x.ToString(),
                       x => (TicketPriority)Enum.Parse(
                           typeof(TicketPriority), x));


            // Status
            builder.Property(x => x.TicketStatus)
                   .HasConversion(
                       x => x.ToString(),
                       x => (TicketStatus)Enum.Parse(
                           typeof(TicketStatus), x));


            // Due Date
            builder.Property(x => x.DueTo)
                   .HasColumnType("date");


            // Created Time
            builder.Property(x => x.CreatedTime)
                   .HasColumnType("datetime");


            // Description
            builder.Property(x => x.Description)
                   .HasColumnType("varchar")
                   .HasMaxLength(2500);


            // Assigned Employee
            builder.HasOne(x => x.Employee)
                   .WithMany(e => e.Tickets)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Ticket Created By
            builder.HasOne(x => x.TicketCreatedBy)
                   .WithMany()
                   .HasForeignKey(x => x.TicketCreatedById)
                   .OnDelete(DeleteBehavior.Restrict);


            // Project
            builder.HasOne(x => x.Project)
                   .WithMany(p => p.ProjectTickets)
                   .HasForeignKey(x => x.ProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            builder.ToTable("Tickets");
        }
    }
}