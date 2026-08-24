using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketAttachmentConfiguration
        : IEntityTypeConfiguration<TicketAttachments>
    {
        public void Configure(
            EntityTypeBuilder<TicketAttachments> builder)
        {
            // Primary Key
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                   .ValueGeneratedOnAdd();


            // URL
            builder.Property(a => a.URL)
                   .HasColumnType("varchar")
                   .HasMaxLength(255)
                   .IsRequired();


            // Ticket -> Attachments
            builder.HasOne(a => a.Ticket)
                   .WithMany(t => t.TicketAttachments)
                   .HasForeignKey(a => a.TicketId)
                   .OnDelete(DeleteBehavior.Cascade);


            builder.ToTable("Attachments");
        }
    }
}