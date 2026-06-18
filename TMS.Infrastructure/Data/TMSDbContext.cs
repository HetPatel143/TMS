using Microsoft.EntityFrameworkCore;
using TMS.Core.Entities;
using TMS.Infrastructure.Data.Configurations;

namespace TMS.Infrastructure.Data
{
    public class TMSDbContext : DbContext
    {
        public TMSDbContext(DbContextOptions<TMSDbContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RequestType> RequestTypes { get; set; } = null!;
        public DbSet<RequestTypeField> RequestTypeFields { get; set; } = null!;
        public DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; } = null!;
        public DbSet<Request> Requests { get; set; } = null!;
        public DbSet<RequestFieldValue> RequestFieldValues { get; set; } = null!;
        public DbSet<RequestApproval> RequestApprovals { get; set; } = null!;
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; } = null!;
        public DbSet<RequestAttachment> RequestAttachments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RequestTypeConfiguration());
            modelBuilder.ApplyConfiguration(new RequestTypeFieldConfiguration());
            modelBuilder.ApplyConfiguration(new ApprovalWorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new RequestConfiguration());
            modelBuilder.ApplyConfiguration(new RequestFieldValueConfiguration());
            modelBuilder.ApplyConfiguration(new RequestApprovalConfiguration());
            modelBuilder.ApplyConfiguration(new RequestStatusHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new RequestAttachmentConfiguration());
        }
    }
}
