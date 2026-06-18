namespace TMS.Core.Entities
{
    public class ApprovalWorkflow
    {
        public int WorkflowId { get; set; }
        public int RequestTypeId { get; set; }
        public int ApprovalOrder { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual RequestType RequestType { get; set; } = null!;
        public virtual Role Role { get; set; } = null!;
    }
}
