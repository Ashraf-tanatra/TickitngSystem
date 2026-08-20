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
            builder.HasKey(t => t.TicketId);

            builder.Property(t => t.TicketId)
                   .ValueGeneratedOnAdd();


            // Ticket Title
            builder.Property(t => t.TicketTitle)
                   .HasColumnType("varchar")
                   .HasMaxLength(100)
                   .IsRequired();


            // Due Date
            builder.Property(t => t.DueTo)
                   .HasColumnType("datetime");


            // Created Time
            builder.Property(t => t.CreatedTime)
                   .HasColumnType("datetime")
                   .IsRequired();


            // Ticket Status
            builder.Property(t => t.TicketStatus)
                   .HasConversion(
                       status => status.ToString(),
                       status => Enum.Parse<TicketStatus>(status))
                   .IsRequired();


            // Ticket Priority
            builder.Property(t => t.Priority)
                   .HasConversion(
                       priority => priority.ToString(),
                       priority => Enum.Parse<TicketPriority>(priority))
                   .IsRequired();


            // Description
            builder.Property(t => t.Description)
                   .HasColumnType("varchar")
                   .HasMaxLength(255);


            // Current Assigned Employee
            builder.HasOne(t => t.Employee)
                   .WithMany(e => e.Tickets)
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Employee Who Created The Ticket
            builder.HasOne(t => t.TicketCreatedBy)
                   .WithMany()
                   .HasForeignKey(t => t.TicketCreatedById)
                   .OnDelete(DeleteBehavior.Restrict);


            // Project
            builder.HasOne(t => t.Project)
                   .WithMany(p => p.ProjectTickets)
                   .HasForeignKey(t => t.ProjectId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);


            // Ticket History
            builder.HasMany(t => t.TicketHistories)
                   .WithOne(h => h.Ticket)
                   .HasForeignKey(h => h.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.ToTable("Tickets");
        }
    }
}