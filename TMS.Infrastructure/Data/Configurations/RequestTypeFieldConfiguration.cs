using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class RequestTypeFieldConfiguration : IEntityTypeConfiguration<RequestTypeField>
    {
        public void Configure(EntityTypeBuilder<RequestTypeField> builder)
        {
            builder.ToTable("RequestTypeFields");
            builder.HasKey(f => f.FieldId);
            builder.Property(f => f.FieldId).UseIdentityColumn();

            builder.Property(f => f.FieldName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.FieldLabel)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.FieldType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(f => f.IsRequired)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(f => f.DisplayOrder)
                .IsRequired();

            builder.Property(f => f.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(f => f.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(f => f.RequestType)
                .WithMany(rt => rt.Fields)
                .HasForeignKey(f => f.RequestTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed fields for all request types
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                // Laptop Request fields
                new RequestTypeField { FieldId = 1, RequestTypeId = 1, FieldName = "laptop_type", FieldLabel = "Laptop Type", FieldType = "Dropdown", IsRequired = true, DisplayOrder = 1, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 2, RequestTypeId = 1, FieldName = "specification", FieldLabel = "Specification/Model", FieldType = "Text", IsRequired = true, DisplayOrder = 2, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 3, RequestTypeId = 1, FieldName = "reason", FieldLabel = "Reason for Request", FieldType = "Text", IsRequired = true, DisplayOrder = 3, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 4, RequestTypeId = 1, FieldName = "needed_by", FieldLabel = "Needed By Date", FieldType = "Date", IsRequired = false, DisplayOrder = 4, IsActive = true, CreatedAt = seedDate },

                // Software Access Request fields
                new RequestTypeField { FieldId = 5, RequestTypeId = 2, FieldName = "software_name", FieldLabel = "Software Name", FieldType = "Text", IsRequired = true, DisplayOrder = 1, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 6, RequestTypeId = 2, FieldName = "access_type", FieldLabel = "Access Type", FieldType = "Dropdown", IsRequired = true, DisplayOrder = 2, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 7, RequestTypeId = 2, FieldName = "justification", FieldLabel = "Business Justification", FieldType = "Text", IsRequired = true, DisplayOrder = 3, IsActive = true, CreatedAt = seedDate },

                // Reimbursement Request fields
                new RequestTypeField { FieldId = 8, RequestTypeId = 3, FieldName = "expense_type", FieldLabel = "Expense Type", FieldType = "Dropdown", IsRequired = true, DisplayOrder = 1, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 9, RequestTypeId = 3, FieldName = "amount", FieldLabel = "Amount (₹)", FieldType = "Number", IsRequired = true, DisplayOrder = 2, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 10, RequestTypeId = 3, FieldName = "expense_date", FieldLabel = "Expense Date", FieldType = "Date", IsRequired = true, DisplayOrder = 3, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 11, RequestTypeId = 3, FieldName = "description", FieldLabel = "Description", FieldType = "Text", IsRequired = true, DisplayOrder = 4, IsActive = true, CreatedAt = seedDate },

                // Leave Request fields
                new RequestTypeField { FieldId = 12, RequestTypeId = 4, FieldName = "leave_type", FieldLabel = "Leave Type", FieldType = "Dropdown", IsRequired = true, DisplayOrder = 1, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 13, RequestTypeId = 4, FieldName = "from_date", FieldLabel = "From Date", FieldType = "Date", IsRequired = true, DisplayOrder = 2, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 14, RequestTypeId = 4, FieldName = "to_date", FieldLabel = "To Date", FieldType = "Date", IsRequired = true, DisplayOrder = 3, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 15, RequestTypeId = 4, FieldName = "reason", FieldLabel = "Reason", FieldType = "Text", IsRequired = false, DisplayOrder = 4, IsActive = true, CreatedAt = seedDate },

                // WFH Request fields
                new RequestTypeField { FieldId = 16, RequestTypeId = 5, FieldName = "wfh_date", FieldLabel = "WFH Date", FieldType = "Date", IsRequired = true, DisplayOrder = 1, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 17, RequestTypeId = 5, FieldName = "duration", FieldLabel = "Duration (Days)", FieldType = "Number", IsRequired = true, DisplayOrder = 2, IsActive = true, CreatedAt = seedDate },
                new RequestTypeField { FieldId = 18, RequestTypeId = 5, FieldName = "reason", FieldLabel = "Reason", FieldType = "Text", IsRequired = true, DisplayOrder = 3, IsActive = true, CreatedAt = seedDate }
            );
        }
    }
}
