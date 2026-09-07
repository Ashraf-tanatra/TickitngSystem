using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Database.Configuration
{
    public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachments>
    {
        public void Configure(EntityTypeBuilder<TicketAttachments> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(a => a.Id).ValueGeneratedOnAdd();

            builder.Property(a => a.CreatedAt)
                .HasColumnType("datetime2");

            builder.Property(a => a.UpdatedAt)
                .HasColumnType("datetime2");

            builder.Property(u => u.URL)
                .HasColumnType("VARCHAR")
                .HasMaxLength(255)
                .IsRequired();

            builder.HasOne(t => t.Ticket)
                .WithMany(a => a.AttachmentURL)
                .HasForeignKey(a => a.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Attachments");
        }
    }
}
