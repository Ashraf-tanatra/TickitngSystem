using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketHistoryConfiguration
        : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            // Primary Key
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                   .ValueGeneratedOnAdd();


            // Action
            builder.Property(h => h.Action)
                   .HasColumnType("varchar")
                   .HasMaxLength(100)
                   .IsRequired();


            // Old Value
            builder.Property(h => h.OldValue)
                   .HasColumnType("varchar")
                   .HasMaxLength(500);


            // New Value
            builder.Property(h => h.NewValue)
                   .HasColumnType("varchar")
                   .HasMaxLength(500);


            // Created Time
            builder.Property(h => h.CreatedAt)
                   .HasColumnType("datetime")
                   .IsRequired();


            // Ticket
            builder.HasOne(h => h.Ticket)
                   .WithMany(t => t.TicketHistories)
                   .HasForeignKey(h => h.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);


            // Employee who performed the action
            builder.HasOne(h => h.ActionByEmployee)
                   .WithMany(e => e.TicketHistories)
                   .HasForeignKey(h => h.ActionByEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            // Previous assigned employee
            builder.HasOne(h => h.FromEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.FromEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            // New assigned employee
            builder.HasOne(h => h.ToEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.ToEmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.ToTable("TicketHistories");
        }
    }
}