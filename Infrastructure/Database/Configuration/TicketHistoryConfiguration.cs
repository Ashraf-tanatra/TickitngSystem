using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
    {
        public void Configure(EntityTypeBuilder<TicketHistory> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            // Ticket 1----* > TicketHistory
            builder.HasOne(h => h.Ticket)
                   .WithMany(t => t.TicketHistories)
                   .HasForeignKey(h => h.TicketId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Employee 1----* > TicketHistory who did the action
            builder.HasOne(h => h.ActionByEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.ActionByEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Employee 1----* > TicketHistory who is assigned to
            builder.HasOne(h => h.FromEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.FromEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);

            // Employee 1----* > TicketHistory who is assigned to
            builder.HasOne(h => h.ToEmployee)
                   .WithMany()
                   .HasForeignKey(h => h.ToEmployeeId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("TicketHistories");
        }
    }
}
