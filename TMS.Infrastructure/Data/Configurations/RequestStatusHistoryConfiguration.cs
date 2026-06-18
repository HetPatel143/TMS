using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestStatusHistoryConfiguration : IEntityTypeConfiguration<RequestStatusHistory>
    {
        public void Configure(EntityTypeBuilder<RequestStatusHistory> builder)
        {
            builder.ToTable("RequestStatusHistories");
            builder.HasKey(sh => sh.StatusHistoryId);
            builder.Property(sh => sh.StatusHistoryId).UseIdentityColumn();

            builder.Property(sh => sh.OldStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sh => sh.NewStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(sh => sh.ChangeReason)
                .HasMaxLength(255);

            builder.Property(sh => sh.ChangedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(sh => sh.Request)
                .WithMany(r => r.StatusHistories)
                .HasForeignKey(sh => sh.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sh => sh.ChangedByUser)
                .WithMany(u => u.StatusHistoryChanges)
                .HasForeignKey(sh => sh.ChangedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
