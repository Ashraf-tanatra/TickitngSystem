using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketHistoryConfiguration
        : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(
            EntityTypeBuilder<TicketHistory> builder)
        {
            // =====================================================
            // Primary Key
            // =====================================================

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                   .ValueGeneratedOnAdd();


            // =====================================================
            // Ticket
            // =====================================================

            builder.HasOne(h => h.Ticket)
                   .WithMany(t => t.TicketHistories)
                   .HasForeignKey(h => h.TicketId)
                   .OnDelete(DeleteBehavior.NoAction);


            // =====================================================
            // Employee Who Performed The Action
            // =====================================================

            builder.HasOne(h => h.ActionByEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.ActionByEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);


            // =====================================================
            // Previous Assigned Employee
            // =====================================================

            builder.HasOne(h => h.FromEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.FromEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);


            // =====================================================
            // New Assigned Employee
            // =====================================================

            builder.HasOne(h => h.ToEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.ToEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);


            builder.ToTable("TicketHistories");
        }
    }
}