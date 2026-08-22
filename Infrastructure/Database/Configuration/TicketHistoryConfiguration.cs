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

            //RelationshipBuilderBase between ticket and ticketHistory 1 --- *
            builder.HasOne(t => t.Ticket).WithMany(t => t.TicketHistories).HasForeignKey(t => t.TicketId).OnDelete(DeleteBehavior.NoAction);

            //RelationshipBuilderBase between ticket and employee 1 --- * 
            builder.HasOne(e => e.ToEmployee).WithMany(e => e.TicketHistories).HasForeignKey(k => k.ToEmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.FromEmployee).WithMany().HasForeignKey(k => k.FromEmployeeId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(e => e.ActionByEmployee).WithMany().HasForeignKey(k => k.ActionByEmployeeId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
