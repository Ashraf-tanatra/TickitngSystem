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
            builder.HasKey(x => x.TicketId);

            builder.Property(x => x.TicketId)
                   .ValueGeneratedOnAdd();

            builder.Property(x => x.TicketTitle)
                   .HasColumnType("varchar")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(x => x.Priority)
                   .HasConversion(
                       x => x.ToString(),
                       x => (TicketPriority)Enum.Parse(typeof(TicketPriority), x));

            builder.Property(x => x.ticketStatus)
                   .HasConversion(
                       x => x.ToString(),
                       x => (TicketStatus)Enum.Parse(typeof(TicketStatus), x));

            builder.Property(x => x.DueTo)
                   .HasColumnType("Date");

            builder.Property(x => x.CreatedTime)
                   .HasColumnType("Date");

            builder.Property(x => x.Description)
                   .HasColumnType("varchar")
                   .HasMaxLength(255);

            builder.HasOne(x => x.Employee)
                   .WithMany(e => e.Tickets)
                   .HasForeignKey(x => x.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Project)
                   .WithMany(x => x.ProjectTickets)
                   .HasForeignKey(e => e.ProjectId)
                   .IsRequired();

            builder.ToTable("Tickets");
        }
    }
}