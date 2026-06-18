using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TMS.Core.Entities;

namespace TMS.Infrastructure.Data.Configurations
{
    public class ApprovalWorkflowConfiguration : IEntityTypeConfiguration<ApprovalWorkflow>
    {
        public void Configure(EntityTypeBuilder<ApprovalWorkflow> builder)
        {
            builder.ToTable("ApprovalWorkflows");
            builder.HasKey(aw => aw.WorkflowId);
            builder.Property(aw => aw.WorkflowId).UseIdentityColumn();

            builder.Property(aw => aw.ApprovalOrder)
                .IsRequired();

            builder.Property(aw => aw.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(aw => aw.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(aw => aw.RequestType)
                .WithMany(rt => rt.Workflows)
                .HasForeignKey(aw => aw.RequestTypeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(aw => aw.Role)
                .WithMany(r => r.ApprovalWorkflows)
                .HasForeignKey(aw => aw.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed approval workflows
            // Roles: 1=Employee, 2=Manager, 3=Finance, 4=IT, 5=HR, 6=Admin
            var seedDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            builder.HasData(
                // Laptop Request: Manager → IT
                new ApprovalWorkflow { WorkflowId = 1, RequestTypeId = 1, ApprovalOrder = 1, RoleId = 2, IsActive = true, CreatedAt = seedDate },
                new ApprovalWorkflow { WorkflowId = 2, RequestTypeId = 1, ApprovalOrder = 2, RoleId = 4, IsActive = true, CreatedAt = seedDate },

                // Software Access Request: Manager → IT
                new ApprovalWorkflow { WorkflowId = 3, RequestTypeId = 2, ApprovalOrder = 1, RoleId = 2, IsActive = true, CreatedAt = seedDate },
                new ApprovalWorkflow { WorkflowId = 4, RequestTypeId = 2, ApprovalOrder = 2, RoleId = 4, IsActive = true, CreatedAt = seedDate },

                // Reimbursement Request: Manager → Finance
                new ApprovalWorkflow { WorkflowId = 5, RequestTypeId = 3, ApprovalOrder = 1, RoleId = 2, IsActive = true, CreatedAt = seedDate },
                new ApprovalWorkflow { WorkflowId = 6, RequestTypeId = 3, ApprovalOrder = 2, RoleId = 3, IsActive = true, CreatedAt = seedDate },

                // Leave Request: Manager → HR
                new ApprovalWorkflow { WorkflowId = 7, RequestTypeId = 4, ApprovalOrder = 1, RoleId = 2, IsActive = true, CreatedAt = seedDate },
                new ApprovalWorkflow { WorkflowId = 8, RequestTypeId = 4, ApprovalOrder = 2, RoleId = 5, IsActive = true, CreatedAt = seedDate },

                // WFH Request: Manager
                new ApprovalWorkflow { WorkflowId = 9, RequestTypeId = 5, ApprovalOrder = 1, RoleId = 2, IsActive = true, CreatedAt = seedDate }
            );
        }
    }
}
