using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestAttachmentConfiguration : IEntityTypeConfiguration<RequestAttachment>
    {
        public void Configure(EntityTypeBuilder<RequestAttachment> builder)
        {
            builder.ToTable("RequestAttachments");
            builder.HasKey(a => a.AttachmentId);
            builder.Property(a => a.AttachmentId).UseIdentityColumn();

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(a => a.FileType)
                .HasMaxLength(100);

            builder.Property(a => a.UploadedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(a => a.Request)
                .WithMany(r => r.Attachments)
                .HasForeignKey(a => a.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(a => a.UploadedByUser)
                .WithMany(u => u.UploadedAttachments)
                .HasForeignKey(a => a.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
