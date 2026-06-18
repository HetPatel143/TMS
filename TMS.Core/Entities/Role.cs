namespace TMS.Core.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<User> Users { get; set; } = new List<User>();
        public virtual ICollection<ApprovalWorkflow> ApprovalWorkflows { get; set; } = new List<ApprovalWorkflow>();
        public virtual ICollection<RequestApproval> RequestApprovals { get; set; } = new List<RequestApproval>();
    }
}
