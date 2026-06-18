using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestFieldValueConfiguration : IEntityTypeConfiguration<RequestFieldValue>
    {
        public void Configure(EntityTypeBuilder<RequestFieldValue> builder)
        {
            builder.ToTable("RequestFieldValues");
            builder.HasKey(fv => fv.FieldValueId);
            builder.Property(fv => fv.FieldValueId).UseIdentityColumn();

            builder.Property(fv => fv.FieldValue)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(fv => fv.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(fv => fv.Request)
                .WithMany(r => r.FieldValues)
                .HasForeignKey(fv => fv.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(fv => fv.Field)
                .WithMany(f => f.FieldValues)
                .HasForeignKey(fv => fv.FieldId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
